using Rustic.Source;

namespace Rustic.DataEnumGen;

internal static class EnumValueStruct
{
    public static void Generate(SrcBuilder text, in GenContext info)
    {
        using (text.Stmt("[Serializable]")
                .Stmt("[StructLayout(LayoutKind.Explicit)]")
                .Decl($"{info.Modifiers} readonly struct {info.EnumName}Value"))
        {
            using (text.InRegion("Fields"))
            {
                Fields(text, in info);
            }

            using (text.InRegion("Constructor"))
            {
                Ctor(text, in info);
                SerCtor(text, in info);
                foreach (var e in info.Members)
                {
                    Factory(text, in info, in e);
                }
            }

            using (text.InRegion("Members"))
            {
                foreach (var e in info.Members)
                {
                    IsEnum(text, in info, in e);
                }
                foreach (var e in info.DataMembers)
                {
                    TryEnum(text, in info, in e);
                }
                foreach (var e in info.DataMembers)
                {
                    ExpectEnum(text, in info, in e);
                }
            }

            using (text.InRegion("IEquatable members"))
            {
                EqualsEnum(text, in info);
                EqualsOther(text, in info);
                EqualsObj(text, in info);
                HashCode(text, in info);
                OperatorEq(text, in info);
                OperatorNeq(text, in info);
            }

            using (text.InRegion("ISerializable"))
            {
                SerGetObjData(text, in info);
            }

            OperatorEnum(text, in info);
        }
    }

    private static void Fields(SrcBuilder text, in GenContext info)
    {
        text.Stmt("[FieldOffset(0)]")
            .Stmt($"public readonly {info.EnumName} Value;");

        foreach (var e in info.DataMembers)
        {
            text.Stmt($"[FieldOffset(sizeof({info.EnumName}))]")
                .Stmt($"public readonly {e.TypeName} {e.Name}Unchecked;");
        }
    }

    private static void Ctor(SrcBuilder text, in GenContext info)
    {
        using (text.Decl($"private {info.EnumName}Value", in info, (in GenContext ctx, ref SrcBuilder.SrcColl p) =>
        {
            p.Add($"in {ctx.EnumName} value");
            foreach (var e in ctx.DataMembers)
            {
                p.Add($"in {e.TypeName} {e.NameLower}");
            }
        }))
        {
            text.Stmt("this.Value = value;");
            CtorSwitch(text, in info);
        }
    }

    private static void CtorSwitch(SrcBuilder text, in GenContext info)
    {
        text.Switch("value", in info, info.DataMembers,
            static (ctx, current) => $"{ctx.EnumName}.{current.Name}",
            static (t, ctx, current) =>
            {
                foreach (var e in ctx.DataMembers)
                {
                    if (e != current)
                    {
                        t.Stmt($"this.{e.Name}Unchecked = {e.NameLower};");
                    }
                }
                t.NL().Stmt($"this.{current.Name}Unchecked = {current.NameLower};");
            },
            static (t, ctx) =>
            {
                foreach (var e in ctx.DataMembers)
                {
                    t.Stmt($"this.{e.Name}Unchecked = default!;");
                }
            });
    }

    private static void Factory(SrcBuilder text, in GenContext info, in EnumContext current)
    {
        using (text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
            .Decl($"public static {info.EnumName}Value {current.Name}", current, (in EnumContext ctx, ref SrcBuilder.SrcColl parameters) =>
        {
            if (ctx.IsDataEnum)
            {
                parameters.Add($"in {ctx.TypeName} value");
            }
        }))
        {
            using var args = text.Call($"return new {info.EnumName}Value");
            args.Add($"{info.EnumName}.{current.Name}");
            foreach (var e in info.DataMembers)
            {
                args.Add(e != current ? "default!" : "value");
            }
        }
    }

    private static void IsEnum(SrcBuilder text, in GenContext info, in EnumContext current)
    {
        using (text.Decl($"public bool Is{current.Name}"))
        {
            text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            using (text.Decl("get"))
            {
                text.Stmt($"return this.Equals({info.EnumName}.{current.Name});");
            }
        }
    }

    private static void TryEnum(SrcBuilder text, in GenContext info, in EnumContext current)
    {
        using (text.Decl($"public bool Try{current.Name}(out {current.TypeName} value)"))
        {
            using (text.If($"this.Is{current.Name}"))
            {
                text.Stmt($"value = this.{current.Name}Unchecked;")
                    .Stmt("return true;");
            }

            text.Stmt("value = default!;")
                    .Stmt("return false;");
        }
    }

    private static void ExpectEnum(SrcBuilder text, in GenContext info, in EnumContext current)
    {
        using (text.Decl($"public {current.TypeName} Expect{current.Name}(string? message)"))
        {
            using (text.If($"this.Is{current.Name}"))
            {
                text.Stmt($"return this.{current.Name}Unchecked;");
            }
            text.Stmt("throw new InvalidOperationException(message);");
        }
    }

    private static void EqualsEnum(SrcBuilder text, in GenContext info)
    {
        text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        using (text.Decl($"public bool Equals({info.EnumName} other)"))
        {
            text.Stmt("return this.Value == other;");
        }
    }

    private static void EqualsOther(SrcBuilder text, in GenContext info)
    {
        using (text.Decl($"public bool Equals({info.EnumName}Value other)"))
        {
            using (text.If("this.Value != other.Value"))
            {
                text.Stmt("return false;");
            }

            text.Switch("this.Value", in info, info.DataMembers,
                static (ctx, current) => $"{ctx.EnumName}.{current.Name}",
                static (t, _, current) =>
                {
                    t.Stmt($"return EqualityComparer<{current.TypeName}>.Default.Equals(this.{current.Name}Unchecked, other.{current.Name}Unchecked);");
                    return true;
                });

            text.Stmt("return true;");
        }
    }

    private static void EqualsObj(SrcBuilder text, in GenContext info)
    {
        using (text.Decl("public override bool Equals(object? obj)"))
        {
            using (text.If($"obj is {info.EnumName}Value other"))
            {
                text.Stmt("return this.Equals(other);");
            }

            using (text.If($"obj is {info.EnumName} value"))
            {
                text.Stmt("return this.Equals(value);");
            }

            text.Stmt("return false;");
        }
    }

    private static void HashCode(SrcBuilder text, in GenContext info)
    {
        using (text.Decl("public override int GetHashCode()"))
        {
            text.Stmt("HashCode hash = new HashCode();")
                .Stmt("hash.Add(this.Value);");

            text.Switch("this.Value", in info, info.DataMembers,
                static (ctx, current) => $"{ctx.EnumName}.{current.Name}",
                static (t, _, current) =>
                {
                    t.Stmt($"hash.Add(this.{current.Name}Unchecked);");
                });

            text.Stmt("return hash.ToHashCode();");
        }
    }

    private static void SerCtor(SrcBuilder text, in GenContext info)
    {
        using (text.Decl($"private {info.EnumName}Value(SerializationInfo info, StreamingContext context)"))
        {
            text.Stmt($"this.Value = ({info.EnumName})info.GetValue(\"Value\", typeof({info.EnumName}))!;");
            text.Switch("this.Value", in info, info.DataMembers,
                static (ctx, current) => $"{ctx.EnumName}.{current.Name}",
                static (t, ctx, current) =>
                {
                    foreach (var e in ctx.DataMembers)
                    {
                        if (e != current)
                        {
                            t.Stmt($"this.{e.Name}Unchecked = default!;");
                        }
                    }
                    t.NL().Stmt($"this.{current.Name}Unchecked = ({current.TypeName})info.GetValue(\"{current.Name}Unchecked\", typeof({current.TypeName}))!;");
                },
                static (t, ctx) =>
                {
                    foreach (var e in ctx.DataMembers)
                    {
                        t.Stmt($"this.{e.Name}Unchecked = default!;");
                    }
                });
        }
    }

    private static void SerGetObjData(SrcBuilder text, in GenContext info)
    {
        using (text.Decl("public void GetObjectData(SerializationInfo info, StreamingContext context)"))
        {
            text.Stmt($"info.AddValue(\"Value\", this.Value, typeof({info.EnumName}));");
            text.Switch("this.Value", in info, info.DataMembers,
                static (ctx, e) => $"{ctx.EnumName}.{e.Name}",
                static (t, _, e) =>
                {
                    t.Stmt($"info.AddValue(\"{e.Name}Unchecked\", this.{e.Name}Unchecked, typeof({e.TypeName}));");
                });
        }
    }

    private static void OperatorEq(SrcBuilder text, in GenContext info)
    {
        text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        using (text.Decl($"public static bool operator ==(in {info.EnumName}Value left, in {info.EnumName}Value right)"))
        {
            text.Stmt("return left.Equals(right);");
        }
    }

    private static void OperatorNeq(SrcBuilder text, in GenContext info)
    {
        text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        using (text.Decl($"public static bool operator !=(in {info.EnumName}Value left, in {info.EnumName}Value right)"))
        {
            text.Stmt("return !left.Equals(right);");
        }
    }

    private static void OperatorEnum(SrcBuilder text, in GenContext info)
    {
        text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        using (text.Decl($"public static implicit operator {info.EnumName}(in {info.EnumName}Value value)"))
        {
            text.Stmt("return value.Value;");
        }
    }
}
