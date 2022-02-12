using System;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Rustic.Source;

namespace Rustic.DataEnumGen;

[Generator]
[CLSCompliant(false)]
public class DataEnumGen : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static (ctx) =>
            ctx.AddSource($"{Const.DataEnumSymbol}.g.cs", SourceText.From(Const.DataEnumSyntax, Encoding.UTF8)));

        var enumDecls = context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) => s is EnumDeclarationSyntax,
                static (ctx, _) => GenContext.CollectDeclInfo(ctx))
            .Where(static (m) => m.HasValue)
            .Select(static (m, _) => m!.Value);

        context.RegisterSourceOutput(enumDecls.Collect(), static (ctx, src) => Generate(ctx, src));
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<GenContext> declsCtx)
    {
        if (declsCtx.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var ctx in declsCtx)
        {
            SrcBuilder text = new(2048);
            Generate(text, in ctx);
            context.AddSource($"{ctx.EnumDataSymbol}.g.cs", SourceText.From(text.ToString(), Encoding.UTF8));
        }
    }

    private static void Generate(SrcBuilder text, in GenContext ctx)
    {
        EnumExtensionsGen ex = new(text);
        EnumDataGen data = new(text);

        using (text.NullableEnable())
        {
            text.Stmt("using System;")
                .Stmt("using System.ComponentModel;")
                .Stmt("using System.Collections.Generic;")
                .Stmt("using System.Runtime.CompilerServices;")
                .Stmt("using System.Runtime.Serialization;")
                .Stmt("using System.Runtime.InteropServices;")
                .NL()
                .Stmt("using Rustic;")
                .NL()
                .NL();

            using (text.Decl($"namespace {ctx.Namespace}"))
            {
                ex.Generate(in ctx);

                // Only if there is any DataEnum member.
                if (ctx.GetDataMembers().MoveNext())
                {
                    text.NL();
                    data.Generate(in ctx);
                }
            }
        }
    }
}

public static class Const
{
    public const string MethodInlineAttr = "[MethodImpl(MethodImplOptions.AggressiveInlining)]";

    public const string FlagsSymbol = "System.FlagsAttribute";

    public const string DataEnumSymbol = "Rustic.DataEnumAttribute";

    public const string DescriptionSymbol = "System.ComponentModel.DescriptionAttribute";

    public const string DataEnumSyntax = @"#nullable enable
using System;

namespace Rustic
{
    /// <summary>Allows a enum member to ship with additional data.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DataEnumAttribute : Attribute
    {
        public DataEnumAttribute(Type data)
        {
            Data = data;
        }

        public Type Data { get; }
    }
}
#nullable restore
";
}

internal readonly struct EnumExtensionsGen
{
    internal readonly SrcBuilder Text;

    public EnumExtensionsGen(SrcBuilder text)
    {
        Text = text;
    }

    public void Generate(in GenContext ctx)
    {
        using (Text.Doc("summary"))
        {
            Text.Append($"Collection of extensions for the {ctx.Symbol} Enum.");
        }
        using (Text.Decl($"{ctx.Modifiers} static class {ctx.Symbol}Extensions"))
        {
            Name(in ctx);
            Description(in ctx);
            Values(in ctx);
            foreach (var mem in ctx.GetMembers())
            {
                Is(mem);
            }
        }
    }

    private void Name(in GenContext ctx)
    {
        using (Text.Doc("summary"))
        {
            Text.Append($"Returns the name of the <see cref=\"{ctx.Symbol}\" value.");
        }
        using (Text.Decl($"public static string Symbol(this {ctx.Symbol} value)"))
        {
            Text.Switch("value", ctx.GetMembers(),
                static (MemberContext ctx) => ctx.Symbol,
                static (t, ctx) =>
                {
                    t.Stmt($"return nameof({ctx.Symbol});");
                    return CaseStyle.NoBreak;
                },
                static (t, _) =>
                {
                    t.Stmt("return value.ToString();");
                    return CaseStyle.NoBreak;
                });
        }
    }

    private void Description(in GenContext ctx)
    {
        using (Text.Doc("summary"))
        {
            Text.Append($"Returns the value of the <see cref=\"System.ComponentModel.DescriptionAttribute\"/> of <see cref=\"{ctx.Symbol}\" value.");
        }
        using (Text.Decl($"public static string? Description(this {ctx.Symbol} value)"))
        {
            Text.Switch("value", ctx.GetMembers(),
                static (MemberContext ctx) => ctx.Mem.Description is null ? null : ctx.Symbol,
                static (t, ctx) =>
                {
                    t.Stmt($"return \"{ctx.Mem.Description}\";");
                    return CaseStyle.NoBreak;
                },
                static (t, _) =>
                {
                    t.Stmt("return null;");
                    return CaseStyle.NoBreak;
                });
        }
    }

    private void Values(in GenContext ctx)
    {
        using (Text.Doc("summary"))
        {
            Text.Append($"Returns the span of all possible values of the <see cref=\"{ctx.Symbol}\".");
        }
        using (Text.Decl($"public static ReadOnlySpan<{ctx.Symbol}> Values"))
        {
            Text.Stmt(Const.MethodInlineAttr);
            using (Text.Decl("get"))
            {
                using var array = Text.Coll($"return new {ctx.Symbol}[] ");
                foreach (var mem in ctx.GetMembers())
                {
                    array.Add(mem.Symbol);
                }
            }
        }
    }

    private void Is(in MemberContext ctx)
    {
        using (Text.Doc("summary"))
        {
            Text.Append($"Returns <c>true</c> if the vlaue is <cee cref=\"{ctx.Symbol}\">; otherwise, returns <c>false</c>.");
        }
        Text.Stmt(Const.MethodInlineAttr);
        using (Text.Decl($"public static bool Is{ctx.Mem.Symbol}(this {ctx.Gen.Symbol} value)"))
        {
            Text.Stmt($"return value == {ctx.Symbol};");
        }
    }
}

internal readonly struct EnumDataGen
{
    internal readonly SrcBuilder Text;

    public EnumDataGen(SrcBuilder text)
    {
        Text = text;
    }

    public void Generate(in GenContext ctx)
    {
        // No good for [Flags] enums
        if (ctx.IsFlags)
        {
            return;
        }

        using (Text.Stmt("[Serializable]")
                .Stmt("[StructLayout(LayoutKind.Explicit)]")
                .Decl($"{ctx.Modifiers} readonly struct {ctx.EnumDataSymbol}"))
        {
            using (Text.Region("Fields"))
            {
                Fields(in ctx);
            }

            using (Text.Region("Constructor"))
            {
                Ctor(in ctx);
                SerCtor(in ctx);
                foreach (var mem in ctx.GetMembers())
                {
                    Factory(mem);
                }
            }

            using (Text.Region("Members"))
            {
                foreach (var mem in ctx.GetMembers())
                {
                    IsEnum(in mem);
                }
                foreach (var mem in ctx.GetDataMembers())
                {
                    TryEnum(in mem);
                }
                foreach (var mem in ctx.GetDataMembers())
                {
                    ExpectEnum(in mem);
                }
            }

            using (Text.Region("IEquatable members"))
            {
                EqualsEnum(in ctx);
                EqualsOther(in ctx);
                EqualsObj(in ctx);
                HashCode(in ctx);
                OperatorEq(in ctx);
                OperatorNeq(in ctx);
            }

            using (Text.Region("ISerializable"))
            {
                SerGetObjData(in ctx);
            }

            OperatorEnum(in ctx);
        }
    }

    private void Fields(in GenContext ctx)
    {
        Text.Stmt("[FieldOffset(0)]")
            .Stmt($"public readonly {ctx.Symbol} Value;");

        foreach (var mem in ctx.GetDataMembers())
        {
            Text.Stmt($"[FieldOffset(sizeof({ctx.Symbol}))]")
                .Stmt($"public readonly {mem.Mem.TypeName} {mem.Mem.Symbol}Unchecked;");
        }
    }

    private void Ctor(in GenContext ctx)
    {
        using (Text.Decl($"private {ctx.EnumDataSymbol}", in ctx, (in GenContext ctx, ref SrcBuilder.SrcColl p) =>
        {
            p.Add($"in {ctx.Symbol} value");
            foreach (var mem in ctx.GetDataMembers())
            {
                p.Add($"in {mem.Mem.TypeName} {mem.Mem.NameLower}");
            }
        }))
        {
            Text.Stmt("this.Value = value;");
            CtorSwitch(in ctx);
        }
    }

    private void CtorSwitch(in GenContext ctx)
    {
        Text.Switch("value", ctx.GetDataMembers(),
            static (MemberContext ctx) => $"{ctx.Symbol}",
            static (t, ctx) =>
            {
                foreach (var cur in ctx.Gen.GetDataMembers())
                {
                    if (!cur.Eq(ctx))
                    {
                        t.Stmt($"this.{cur.Mem.Symbol}Unchecked = {cur.Mem.NameLower};");
                    }
                }
                t.NL().Stmt($"this.{ctx.Mem.Symbol}Unchecked = {ctx.Mem.NameLower};");
            },
            static (t, ctx) =>
            {
                foreach (var mem in ctx.GetEnumerator())
                {
                    t.Stmt($"this.{mem.Mem.Symbol}Unchecked = default!;");
                }
            });
    }

    private void Factory(in MemberContext ctx)
    {
        using (Text.Stmt(Const.MethodInlineAttr)
            .Decl($"public static {ctx.Gen.EnumDataSymbol} {ctx.Mem.Symbol}", ctx.Mem, (in EnumContext mem, ref SrcBuilder.SrcColl parameters) =>
        {
            if (mem.IsDataEnum)
            {
                parameters.Add($"in {mem.TypeName} value");
            }
        }))
        {
            using var args = Text.Call($"return new {ctx.Gen.EnumDataSymbol}");
            args.Add(ctx.Symbol);
            foreach (var e in ctx.Gen.GetDataMembers())
            {
                args.Add(e.Eq(ctx) ? "value" : "default!");
            }
        }
    }

    private void IsEnum(in MemberContext ctx)
    {
        using (Text.Decl($"public bool Is{ctx.Mem.Symbol}"))
        {
            Text.Stmt(Const.MethodInlineAttr);
            using (Text.Decl("get"))
            {
                Text.Stmt($"return this.Equals({ctx.Symbol});");
            }
        }
    }

    private void TryEnum(in MemberContext ctx)
    {
        using (Text.Decl($"public bool Try{ctx.Mem.Symbol}(out {ctx.Mem.TypeName} value)"))
        {
            using (Text.If($"this.Is{ctx.Mem.Symbol}"))
            {
                Text.Stmt($"value = this.{ctx.Mem.Symbol}Unchecked;")
                    .Stmt("return true;");
            }

            Text.Stmt("value = default!;")
                    .Stmt("return false;");
        }
    }

    private void ExpectEnum(in MemberContext ctx)
    {
        using (Text.Decl($"public {ctx.Mem.TypeName} Expect{ctx.Mem.Symbol}(string? message)"))
        {
            using (Text.If($"this.Is{ctx.Mem.Symbol}"))
            {
                Text.Stmt($"return this.{ctx.Mem.Symbol}Unchecked;");
            }
            Text.Stmt("throw new InvalidOperationException(message);");
        }
    }

    private void EqualsEnum(in GenContext ctx)
    {
        Text.Stmt(Const.MethodInlineAttr);
        using (Text.Decl($"public bool Equals({ctx.Symbol} other)"))
        {
            Text.Stmt("return this.Value == other;");
        }
    }

    private void EqualsOther(in GenContext ctx)
    {
        using (Text.Decl($"public bool Equals({ctx.EnumDataSymbol} other)"))
        {
            using (Text.If("this.Value != other.Value"))
            {
                Text.Stmt("return false;");
            }

            Text.Switch("this.Value", ctx.GetDataMembers(),
                static (MemberContext mem) => mem.Symbol,
                static (t, mem) =>
                {
                    t.Stmt($"return EqualityComparer<{mem.Mem.TypeName}>.Default.Equals(this.{mem.Mem.Symbol}Unchecked, other.{mem.Mem.Symbol}Unchecked);");
                    return CaseStyle.NoBreak;
                });

            Text.Stmt("return true;");
        }
    }

    private void EqualsObj(in GenContext ctx)
    {
        using (Text.Decl("public override bool Equals(object? obj)"))
        {
            using (Text.If($"obj is {ctx.EnumDataSymbol} other"))
            {
                Text.Stmt("return this.Equals(other);");
            }

            using (Text.If($"obj is {ctx.Symbol} value"))
            {
                Text.Stmt("return this.Equals(value);");
            }

            Text.Stmt("return false;");
        }
    }

    private void HashCode(in GenContext ctx)
    {
        using (Text.Decl("public override int GetHashCode()"))
        {
            Text.Stmt("HashCode hash = new HashCode();")
                .Stmt("hash.Add(this.Value);");

            Text.Switch("this.Value", ctx.GetDataMembers(),
                static (MemberContext ctx) => ctx.Symbol,
                static (t, ctx) =>
                {
                    t.Stmt($"hash.Add(this.{ctx.Mem.Symbol}Unchecked);");
                });

            Text.Stmt("return hash.ToHashCode();");
        }
    }

    private void SerCtor(in GenContext ctx)
    {
        using (Text.Decl($"private {ctx.EnumDataSymbol}(SerializationInfo info, StreamingContext context)"))
        {
            Text.Stmt($"this.Value = ({ctx.Symbol})info.GetValue(\"Value\", typeof({ctx.Symbol}))!;");
            Text.Switch("this.Value", ctx.GetDataMembers(),
                static (MemberContext ctx) => ctx.Symbol,
                static (t, ctx) =>
                {
                    foreach (var e in ctx.Gen.GetDataMembers())
                    {
                        if (!e.Eq(ctx))
                        {
                            t.Stmt($"this.{e.Mem.Symbol}Unchecked = default!;");
                        }
                    }
                    t.NL().Stmt($"this.{ctx.Mem.Symbol}Unchecked = ({ctx.Mem.TypeName})info.GetValue(\"{ctx.Mem.Symbol}Unchecked\", typeof({ctx.Mem.TypeName}))!;");
                },
                static (t, ctx) =>
                {
                    foreach (var e in ctx.GetEnumerator())
                    {
                        t.Stmt($"this.{e.Mem.Symbol}Unchecked = default!;");
                    }
                });
        }
    }

    private void SerGetObjData(in GenContext ctx)
    {
        using (Text.Decl("public void GetObjectData(SerializationInfo info, StreamingContext context)"))
        {
            Text.Stmt($"info.AddValue(\"Value\", this.Value, typeof({ctx.Symbol}));");
            Text.Switch("this.Value", ctx.GetDataMembers(),
                static (MemberContext ctx) => ctx.Symbol,
                static (t, ctx) =>
                {
                    t.Stmt($"info.AddValue(\"{ctx.Mem.Symbol}Unchecked\", this.{ctx.Mem.Symbol}Unchecked, typeof({ctx.Mem.TypeName}));");
                });
        }
    }

    private void OperatorEq(in GenContext ctx)
    {
        Text.Stmt(Const.MethodInlineAttr);
        using (Text.Decl($"public static bool operator ==(in {ctx.EnumDataSymbol} left, in {ctx.EnumDataSymbol} right)"))
        {
            Text.Stmt("return left.Equals(right);");
        }
    }

    private void OperatorNeq(in GenContext ctx)
    {
        Text.Stmt(Const.MethodInlineAttr);
        using (Text.Decl($"public static bool operator !=(in {ctx.EnumDataSymbol} left, in {ctx.EnumDataSymbol} right)"))
        {
            Text.Stmt("return !left.Equals(right);");
        }
    }

    private void OperatorEnum(in GenContext ctx)
    {
        Text.Stmt(Const.MethodInlineAttr);
        using (Text.Decl($"public static implicit operator {ctx.Symbol}(in {ctx.EnumDataSymbol} value)"))
        {
            Text.Stmt("return value.Value;");
        }
    }
}
