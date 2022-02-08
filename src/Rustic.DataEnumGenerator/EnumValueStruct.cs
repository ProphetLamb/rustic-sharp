namespace Rustic.DataEnumGenerator;

internal static class EnumValueStruct
{
    public static void Generate(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[Serializable]");
        builder.AppendLine("[StructLayout(LayoutKind.Explicit)]");
        builder.AppendLine($"{info.Modifiers} readonly struct {info.EnumValueName}");
        builder.BlockStart();

        builder.Region("Fields");
        Fields(ref builder, in info);
        builder.EndRegion("Fields");

        builder.Region("Constructor");
        Ctor(ref builder, in info);
        SerCtor(ref builder, in info);
        foreach (var e in info.Members)
        {
            Factory(ref builder, in info, in e);
        }
        builder.EndRegion("Constructor");

        builder.Region("Members");
        foreach (var e in info.Members)
        {
            IsEnum(ref builder, in info, in e);
        }
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                TryEnum(ref builder, in info, in e);
            }
        }
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                ExpectEnum(ref builder, in info, in e);
            }
        }
        builder.EndRegion("Members");

        builder.Region("IEquatable members");
        EqualsEnum(ref builder, in info);
        EqualsOther(ref builder, in info);
        EqualsObj(ref builder, in info);
        HashCode(ref builder, in info);
        OperatorEq(ref builder, in info);
        OperatorNeq(ref builder, in info);
        builder.EndRegion("IEquatable members");

        builder.Region("ISerializable");
        SerGetObjData(ref builder, in info);
        builder.EndRegion("ISerializable");

        OperatorEnum(ref builder, in info);
        builder.BlockEnd();
    }

    private static void Fields(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[FieldOffset(0)]");
        builder.AppendLine($"public readonly {info.EnumName} Value;");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine();
                builder.AppendLine($"[FieldOffset(sizeof({info.EnumName}))]");
                builder.AppendLine($"public readonly {e.TypeName} {e.NameUnchecked};");
            }
        }
        builder.AppendLine();
    }

    private static void Ctor(ref SrcBuilder builder, in GenInfo info)
    {
        // Declaration
        builder.AppendIndent(); builder.Append($"private {info.EnumValueName}(");

        builder.Append($"in {info.EnumName} value");
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.Append($", in {e.TypeName} {e.NameLower}");
            }
        }
        builder.Append(')'); builder.AppendLine();
        builder.BlockStart();

        builder.AppendLine("this.Value = value;");

        CtorSwitch(ref builder, in info);
        builder.BlockEnd();
    }

    private static void CtorSwitch(ref SrcBuilder builder, in GenInfo info)
    {
        builder.StartSwitchBlock("value");

        // Branches for each DataEnum
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                CtorSwitchCase(ref builder, in info, in e);
            }
        }

        // Default
        builder.CaseStart();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.CaseEnd();
        builder.BlockEnd();
    }

    private static void CtorSwitchCase(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"case {info.EnumName}.{current.Name}:"); builder.Indent();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum && e != current)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = {e.NameLower};");
            }
        }
        builder.AppendLine();
        builder.AppendLine($"this.{current.NameUnchecked} = {current.NameLower};");
        builder.AppendLine("break;"); builder.Outdent();
    }

    private static void Factory(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendIndent(); builder.Append($"public static {info.EnumValueName} {current.Name}(");
        if (current.IsDataEnum)
        {
            builder.Append($"in {current.TypeName} value");
        }
        builder.Append(')'); builder.AppendLine();

        builder.BlockStart();
        builder.AppendIndent(); builder.Append($"return new {info.EnumValueName}({info.EnumName}.{current.Name}");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.Append(e != current ? ", default!" : ", value");
            }
        }

        builder.Append(");"); builder.AppendLine();
        builder.BlockEnd();
    }

    private static void IsEnum(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public bool Is{current.Name}");
        builder.BlockStart();

        builder.AppendLine("get");
        builder.BlockStart();

        builder.Return($"this.Equals({info.EnumName}.{current.Name})");
        builder.BlockEnd();
        builder.BlockEnd();
    }

    private static void TryEnum(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public bool Try{current.Name}(out {current.TypeName} value)");
        builder.BlockStart();

        builder.StartIfBlock($"this.Is{current.Name}");
        builder.AppendLine($"value = this.{current.NameUnchecked};");
        builder.Return("true");
        builder.BlockEnd();

        builder.AppendLine("value = default!;");
        builder.Return("false");
        builder.BlockEnd();
    }

    private static void ExpectEnum(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public {current.TypeName} Expect{current.Name}(string? message)");
        builder.BlockStart();

        builder.StartIfBlock($"this.Is{current.Name}");
        builder.Return($"this.{current.NameUnchecked}");
        builder.BlockEnd();

        builder.AppendLine("throw new InvalidOperationException(message);");
        builder.BlockEnd();
    }

    private static void EqualsEnum(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public bool Equals({info.EnumName} other)");
        builder.BlockStart();

        builder.Return("this.Value == other");
        builder.BlockEnd();
    }

    private static void EqualsOther(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"public bool Equals({info.EnumValueName} other)");
        builder.BlockStart();

        builder.StartIfBlock("this.Value != other.Value");
        builder.Return("false");
        builder.BlockEnd();

        builder.StartSwitchBlock("this.Value");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.CaseStart($"{info.EnumName}.{e.Name}");
                builder.Return($"EqualityComparer<{e.TypeName}>.Default.Equals(this.{e.NameUnchecked}, other.{e.NameUnchecked})");
                builder.CaseEnd();
            }
        }
        builder.BlockEnd();

        builder.Return("true");
        builder.BlockEnd();
    }

    private static void EqualsObj(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("public override bool Equals(object? obj)");
        builder.BlockStart();

        builder.StartIfBlock($"obj is {info.EnumValueName} other");
        builder.Return("this.Equals(other)");
        builder.BlockEnd();

        builder.StartIfBlock($"obj is {info.EnumName} value");
        builder.Return("this.Equals(value)");
        builder.BlockEnd();

        builder.Return("false");
        builder.BlockEnd();
    }

    private static void HashCode(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("public override int GetHashCode()");
        builder.BlockStart();

        builder.AppendLine("HashCode hash = new HashCode();");
        builder.AppendLine("hash.Add(this.Value);");
        builder.AppendLine();

        builder.StartSwitchBlock("this.Value");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.CaseStart($"{info.EnumName}.{e.Name}"); ;
                builder.AppendLine($"hash.Add(this.{e.NameUnchecked});");
                builder.CaseEnd();
            }
        }
        builder.BlockEnd();

        builder.Return("hash.ToHashCode()");
        builder.BlockEnd();
    }

    private static void SerCtor(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"private {info.EnumValueName}(SerializationInfo info, StreamingContext context)");
        builder.BlockStart();

        builder.AppendLine($"this.Value = ({info.EnumName})info.GetValue(\"Value\", typeof({info.EnumName}))!;");

        builder.StartSwitchBlock("Value");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                SerCtorSwitchCase(ref builder, in info, in e);
            }
        }

        // Default
        builder.CaseStart();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.CaseEnd();
        builder.BlockEnd();
        builder.BlockEnd();
    }

    private static void SerCtorSwitchCase(ref SrcBuilder builder, in GenInfo info, in EnumDeclInfo current)
    {
        builder.CaseStart($"{info.EnumName}.{current.Name}");
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum && e != current)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.AppendLine();

        builder.AppendLine($"this.{current.NameUnchecked} = ({current.TypeName})info.GetValue(\"{current.NameUnchecked}\", typeof({current.TypeName}))!;");
        builder.CaseEnd();
    }

    private static void SerGetObjData(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("public void GetObjectData(SerializationInfo info, StreamingContext context)");
        builder.BlockStart();

        builder.AppendLine($"info.AddValue(\"Value\", this.Value, typeof({info.EnumName}));");

        builder.StartSwitchBlock("Value");
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.CaseStart($"{info.EnumName}.{e.Name}");
                builder.AppendLine($"info.AddValue(\"{e.NameUnchecked}\", this.{e.NameUnchecked}, typeof({e.TypeName}));");
                builder.CaseEnd();
            }
        }
        builder.BlockEnd();
        builder.BlockEnd();
    }

    private static void OperatorEq(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static bool operator ==(in {info.EnumValueName} left, in {info.EnumValueName} right)");
        builder.BlockStart();

        builder.Return("left.Equals(right)");
        builder.BlockEnd();
    }

    private static void OperatorNeq(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static bool operator !=(in {info.EnumValueName} left, in {info.EnumValueName} right)");
        builder.BlockStart();

        builder.Return("!left.Equals(right)");
        builder.BlockEnd();
    }

    private static void OperatorEnum(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static implicit operator {info.EnumName}(in {info.EnumValueName} value)");
        builder.BlockStart();

        builder.Return("value.Value");
        builder.BlockEnd();
    }
}
