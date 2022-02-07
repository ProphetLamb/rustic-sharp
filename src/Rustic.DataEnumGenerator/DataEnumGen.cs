using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Rustic.DataEnumGenerator.Extensions;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rustic.DataEnumGenerator;

[Generator]
public class DataEnumGen : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{GeneratorInfo.DataEnumSymbol}.g.cs", SourceText.From(GeneratorInfo.DataEnumSource, Encoding.UTF8)));

        var enumDecls = context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) => IsEnumDecl(s),
                static (ctx, _) => CollectFileInfo(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        var compilationEnumDeclUnion = context.CompilationProvider.Combine(enumDecls.Collect());

        context.RegisterSourceOutput(compilationEnumDeclUnion, static (spc, source) => Generate(source.Left, source.Right, spc));

    }

    private static bool IsEnumDecl(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax;
    }

    private static GeneratorInfo? CollectFileInfo(GeneratorSyntaxContext context)
    {
        if (context.Node is not EnumDeclarationSyntax enumDecl)
        {
            return default;
        }

        if (enumDecl.FindAttribute(context, static (s, ctx) => HasFlags(s, ctx)) is not null)
        {
            // Flags enum are not allowed!
            return default;
        }

        var members = ImmutableArray.CreateBuilder<EnumDeclInfo>(enumDecl.Members.Count);
        foreach (var memberSyntax in enumDecl.Members)
        {
            if (memberSyntax is EnumMemberDeclarationSyntax memberDecl)
            {
                members.Add(CollectEnumDeclInfo(context, memberDecl));
            }
        }

        var (nsDecl, nestingDecls) = enumDecl.GetHierarchy<BaseTypeDeclarationSyntax>();
        return new GeneratorInfo(nsDecl, nestingDecls, enumDecl, members.MoveToImmutable());
    }

    private static EnumDeclInfo CollectEnumDeclInfo(GeneratorSyntaxContext context, EnumMemberDeclarationSyntax memberDecl)
    {
        var dataEnumAttr = memberDecl.FindAttribute(context, static (s, ctx) => HasDataType(s, ctx));
        TypeSyntax? dataType = null;
        var typeArg = dataEnumAttr?.ArgumentList?.Arguments[0];
        if (typeArg?.Expression is TypeOfExpressionSyntax tof)
        {
            dataType = tof.Type;
        }

        var descrAttr = memberDecl.FindAttribute(context, static (s, ctx) => HasDescription(s, ctx));
        string? descr = null;
        var descrArg = descrAttr?.ArgumentList?.Arguments[0];
        if (descrArg?.Expression is LiteralExpressionSyntax literal)
        {
            descr = literal.Token.ValueText;
        }

        return new EnumDeclInfo(memberDecl, dataType, descr);
    }

    private static bool HasFlags(AttributeSyntax s, GeneratorSyntaxContext ctx)
    {
        return GeneratorInfo.FlagsSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static bool HasDataType(AttributeSyntax s, GeneratorSyntaxContext ctx)
    {
        return GeneratorInfo.DataEnumSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static bool HasDescription(AttributeSyntax s, GeneratorSyntaxContext ctx)
    {
        return GeneratorInfo.DescriptionSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static void Generate(Compilation compilation, ImmutableArray<GeneratorInfo> members, SourceProductionContext context)
    {
        if (members.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var info in members.Distinct())
        {
            var sourceText = GenerateEnumSource(compilation, info);
            context.AddSource($"{info.EnumValueName}.g.cs", sourceText);
        }
    }

    internal static SourceText GenerateEnumSource(Compilation compilation, in GeneratorInfo info)
    {
        SourceTextBuilder builder = new(stackalloc char[2048]);
        GeneratorInfo.Generate(ref builder, in info);
        return SourceText.From(builder.ToString(), Encoding.UTF8);
    }
}

public readonly struct EnumDeclInfo : IEquatable<EnumDeclInfo>
{
    public readonly EnumMemberDeclarationSyntax EnumNode;
    public readonly TypeSyntax? TypeNode;
    public readonly string? Description;

    public readonly string Name;
    public readonly string NameUnchecked;
    public readonly string NameLower;
    public readonly string? TypeName;

    public EnumDeclInfo(EnumMemberDeclarationSyntax enumNode, TypeSyntax? typeNode, string? description)
    {
        EnumNode = enumNode;
        TypeNode = typeNode;
        Description = description;

        Name = EnumNode.Identifier.Text;
        NameUnchecked = $"{Name}Unchecked";
        NameLower = Name.ToLower();
        TypeName = TypeNode?.ToString();
    }

    public bool IsDataEnum => TypeNode is not null;

    #region IEquality members

    public bool Equals(EnumDeclInfo other)
    {
        return EnumNode.Equals(other.EnumNode) && Equals(TypeNode, other.TypeNode);
    }

    public override bool Equals(object? obj)
    {
        return obj is EnumDeclInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (EnumNode.GetHashCode() * 397) ^ (TypeNode != null ? TypeNode.GetHashCode() : 0);
        }
    }

    public static bool operator ==(EnumDeclInfo left, EnumDeclInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EnumDeclInfo left, EnumDeclInfo right)
    {
        return !left.Equals(right);
    }

    #endregion IEquality members
}

public readonly struct GeneratorInfo
{
    public readonly NamespaceDeclarationSyntax Ns;
    public readonly ImmutableArray<BaseTypeDeclarationSyntax> Nesting;
    public readonly EnumDeclarationSyntax EnumDecl;
    public readonly ImmutableArray<EnumDeclInfo> Members;

    public readonly string Namespace;
    public readonly string Modifiers;
    public readonly string EnumName;
    public readonly string EnumValueName;
    public readonly string EnumExtensionsName;

    public GeneratorInfo(NamespaceDeclarationSyntax ns, ImmutableArray<BaseTypeDeclarationSyntax> nesting,
        EnumDeclarationSyntax enumDecl, ImmutableArray<EnumDeclInfo> members)
    {
        Ns = ns;
        Nesting = nesting;
        EnumDecl = enumDecl;
        Members = members;
        Namespace = Ns.Name.ToString();
        Modifiers = EnumDecl.Modifiers.ToString();
        EnumName = EnumDecl.Identifier.Text;
        EnumValueName = $"{EnumDecl.Identifier.Text}Value";
        EnumExtensionsName = $"{EnumDecl.Identifier.Text}Extensions";
    }

    public const string FlagsSymbol = "System.FlagsAttribute";

    public const string DataEnumSymbol = "Rustic.Attributes.DataEnumAttribute";

    public const string DescriptionSymbol = "System.ComponentModel.DescriptionAttribute";

    public const string DataEnumSource = @"#nullable enable
namespace Rustic.Attributes
{
    /// <summary>Allows a enum member to ship with additional data.</summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class DataEnumAttribute : System.Attribute
    {
        public DataEnumAttribute(System.Type data)
        {
            Data = data;
        }

        public System.Type Data { get; }
    }
}
#nullable restore";

    public static void Generate(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine("#nullable enable");
        builder.AppendLine("using System;");
        builder.AppendLine("using System.ComponentModel;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using System.Runtime.Serialization;");
        builder.AppendLine("using System.Runtime.InteropServices;");
        builder.AppendLine();
        builder.AppendLine($"namespace {info.Namespace}");

        builder.AppendLine('{'); builder.Indent();

        EnumExtensionsClass.Generate(ref builder, in info);

        // Only if there is any DataEnum member.
        if (info.Members.Any(m => m.IsDataEnum))
        {
            builder.AppendLine();
            EnumValueStruct.Generate(ref builder, in info);
        }

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine("#nullable restore");
    }
}

public static class EnumExtensionsClass
{
    public static void Generate(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"{info.Modifiers} static class {info.EnumExtensionsName}");
        builder.AppendLine('{'); builder.Indent();

        Name(ref builder, in info);
        Description(ref builder, in info);
        Values(ref builder, in info);

        builder.Outdent(); builder.AppendLine('}');
    }

    private static void Name(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static string Name(this {info.EnumName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"switch (value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            builder.AppendLine($"case {info.EnumName}.{e.Name}:");
            builder.Indent();
            builder.AppendLine($"return nameof({info.EnumName}.{e.Name});");
            builder.Outdent();
        }

        builder.AppendLine("default:"); builder.Indent();
        builder.AppendLine("return value.ToString();"); builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void Description(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static string? Description(this {info.EnumName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"switch (value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.Description is not null)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:");
                builder.Indent();
                builder.AppendLine($"return \"{e.Description}\";");
                builder.Outdent();
            }
        }

        builder.AppendLine("default:");
        builder.Indent();
        builder.AppendLine("return null;");
        builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void Values(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static ReadOnlySpan<{info.EnumName}> Values");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("get");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"return new {info.EnumName}[]");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            builder.AppendLine($"{info.EnumName}.{e.Name},");
        }

        builder.Outdent(); builder.AppendLine("};");

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }
}

public static class EnumValueStruct
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
