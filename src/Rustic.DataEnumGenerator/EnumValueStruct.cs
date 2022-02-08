namespace Rustic.DataEnumGenerator;

internal static class EnumValueStruct
{
    public static void Generate(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("[Serializable]");
        builder.AppendLine("[StructLayout(LayoutKind.Explicit)]");
        builder.AppendLine($"{info.Modifiers} readonly struct {info.EnumValueName}");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendDoubleLine("#region Fields");
        Fields(ref builder, in info);
        builder.AppendDoubleLine("#endregion Fields");

        builder.AppendDoubleLine("#region Constructor");
        Ctor(ref builder, in info);
        SerCtor(ref builder, in info);
        foreach (var e in info.Members)
        {
            Factory(ref builder, in info, in e);
        }
        builder.AppendDoubleLine("#endregion Constructor");

        builder.AppendDoubleLine("#region Members");
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
        builder.AppendDoubleLine("#endregion Members");

        builder.AppendDoubleLine("#region IEquatable members");
        EqualsEnum(ref builder, in info);
        EqualsOther(ref builder, in info);
        EqualsObj(ref builder, in info);
        HashCode(ref builder, in info);
        OperatorEq(ref builder, in info);
        OperatorNeq(ref builder, in info);
        builder.AppendDoubleLine("#endregion IEquatable members");

        builder.AppendDoubleLine("#region ISerializable");
        SerGetObjData(ref builder, in info);
        builder.AppendDoubleLine("#endregion ISerializable");

        OperatorEnum(ref builder, in info);

        builder.Outdent(); builder.AppendLine('}');
    }

    private static void Fields(ref SourceTextBuilder builder, in GeneratorInfo info)
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

    private static void Ctor(ref SourceTextBuilder builder, in GeneratorInfo info)
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

        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("this.Value = value;");

        CtorSwitch(ref builder, in info);

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void CtorSwitch(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("switch (value)");
        builder.AppendLine('{'); builder.Indent();

        // Branches for each DataEnum
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                CtorSwitchCase(ref builder, in info, in e);
            }
        }

        // Default
        builder.AppendLine("default:"); builder.Indent();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.AppendLine("break;"); builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');
    }

    private static void CtorSwitchCase(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
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

    private static void Factory(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendIndent(); builder.Append($"public static {info.EnumValueName} {current.Name}(");
        if (current.IsDataEnum)
        {
            builder.Append($"in {current.TypeName} value");
        }
        builder.Append(')'); builder.AppendLine();

        builder.AppendLine('{'); builder.Indent();
        builder.AppendIndent(); builder.Append($"return new {info.EnumValueName}({info.EnumName}.{current.Name}");

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.Append(e != current ? ", default!" : ", value");
            }
        }

        builder.Append(");"); builder.AppendLine();
        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void IsEnum(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public bool Is{current.Name}");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("get");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"return this.Equals({info.EnumName}.{current.Name});");

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void TryEnum(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public bool Try{current.Name}(out {current.TypeName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"if (this.Is{current.Name})");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"value = this.{current.NameUnchecked};");
        builder.AppendLine("return true;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("value = default!;");
        builder.AppendLine("return false;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void ExpectEnum(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"public {current.TypeName} Expect{current.Name}(string? message)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"if (this.Is{current.Name})");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"return this.{current.NameUnchecked};");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("throw new InvalidOperationException(message);");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void EqualsEnum(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public bool Equals({info.EnumName} other)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return this.Value == other;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void EqualsOther(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public bool Equals({info.EnumValueName} other)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("if (this.Value != other.Value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return false;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("switch (this.Value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:"); builder.Indent();
                builder.AppendLine($"return EqualityComparer<{e.TypeName}>.Default.Equals(this.{e.NameUnchecked}, other.{e.NameUnchecked});"); builder.Outdent();
            }
        }

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("return true;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void EqualsObj(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("public override bool Equals(object? obj)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"if (obj is {info.EnumValueName} other)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return this.Equals(other);");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine($"if (obj is {info.EnumName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return this.Equals(value);");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("return false;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void HashCode(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("public override int GetHashCode()");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("var hash = new HashCode();");
        builder.AppendLine("hash.Add(this.Value);");
        builder.AppendLine();

        builder.AppendLine("switch (this.Value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:"); builder.Indent();
                builder.AppendLine($"hash.Add(this.{e.NameUnchecked});");
                builder.AppendLine("break;"); builder.Outdent();
            }
        }


        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();

        builder.AppendLine("return hash.ToHashCode();");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void SerCtor(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"private {info.EnumValueName}(SerializationInfo info, StreamingContext context)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"this.Value = ({info.EnumName})info.GetValue(\"Value\", typeof({info.EnumName}))!;");

        builder.AppendLine("switch (Value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                SerCtorSwitchCase(ref builder, in info, in e);
            }
        }

        // Default
        builder.AppendLine("default:"); builder.Indent();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.AppendLine("break;"); builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void SerCtorSwitchCase(ref SourceTextBuilder builder, in GeneratorInfo info, in EnumDeclInfo current)
    {
        builder.AppendLine($"case {info.EnumName}.{current.Name}:"); builder.Indent();
        foreach (var e in info.Members)
        {
            if (e.IsDataEnum && e != current)
            {
                builder.AppendLine($"this.{e.NameUnchecked} = default!;");
            }
        }
        builder.AppendLine();

        builder.AppendLine($"this.{current.NameUnchecked} = ({current.TypeName})info.GetValue(\"{current.NameUnchecked}\", typeof({current.TypeName}))!;");
        builder.AppendLine("break;"); builder.Outdent();
    }

    private static void SerGetObjData(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("public void GetObjectData(SerializationInfo info, StreamingContext context)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"info.AddValue(\"Value\", this.Value, typeof({info.EnumName}));");

        builder.AppendLine("switch (Value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.IsDataEnum)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:"); builder.Indent();
                builder.AppendLine($"info.AddValue(\"{e.NameUnchecked}\", this.{e.NameUnchecked}, typeof({e.TypeName}));");
                builder.AppendLine("break;"); builder.Outdent();
            }
        }

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void OperatorEq(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static bool operator ==(in {info.EnumValueName} left, in {info.EnumValueName} right)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return left.Equals(right);");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void OperatorNeq(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static bool operator !=(in {info.EnumValueName} left, in {info.EnumValueName} right)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return !left.Equals(right);");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void OperatorEnum(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"public static implicit operator {info.EnumName}(in {info.EnumValueName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("return value.Value;");

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }
}
