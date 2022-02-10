using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Rustic.Source;

namespace Rustic.DataEnumGen;

[Generator(LanguageNames.CSharp)]
[CLSCompliant(false)]
public class DataEnumGen : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{GenInfo.DataEnumSymbol}.g.cs", SourceText.From(GenInfo.DataEnumSyntax, Encoding.UTF8)));

        var enumDecls = context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) => IsEnumDecl(s),
                static (ctx, _) => CollectTreeInfo(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        var compilationEnumDeclUnion = context.CompilationProvider.Combine(enumDecls.Collect());

        context.RegisterSourceOutput(compilationEnumDeclUnion, static (spc, source) => Generate(source.Right, spc));
    }

    private static bool IsEnumDecl(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax;
    }

    private static GenInfo? CollectTreeInfo(GeneratorSyntaxContext context)
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
        return new GenInfo(nsDecl, nestingDecls, enumDecl, members.MoveToImmutable());
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
        return GenInfo.FlagsSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static bool HasDataType(AttributeSyntax s, GeneratorSyntaxContext ctx)
    {
        return GenInfo.DataEnumSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static bool HasDescription(AttributeSyntax s, GeneratorSyntaxContext ctx)
    {
        return GenInfo.DescriptionSymbol.Equals(ctx.SemanticModel.GetTypeInfo(s).Type?.ToDisplayString());
    }

    private static void Generate(ImmutableArray<GenInfo> members, SourceProductionContext context)
    {
        if (members.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var info in members.Distinct())
        {
            SrcBuilder text = new(2048);
            GenInfo.Generate(text, in info);
            context.AddSource($"{info.EnumName}Value.g.cs", SourceText.From(text.ToString(), Encoding.UTF8));
        }
    }
}

[CLSCompliant(false)]
public readonly struct EnumDeclInfo : IEquatable<EnumDeclInfo>
{
    public readonly EnumMemberDeclarationSyntax EnumNode;
    public readonly TypeSyntax? TypeNode;
    public readonly string? Description;

    public readonly string Name;
    public readonly string NameLower;
    public readonly string? TypeName;

    public EnumDeclInfo(EnumMemberDeclarationSyntax enumNode, TypeSyntax? typeNode, string? description)
    {
        EnumNode = enumNode;
        TypeNode = typeNode;
        Description = description;

        Name = EnumNode.Identifier.Text;
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
            return (EnumNode.GetHashCode() * 397) ^ ((TypeNode?.GetHashCode()) ?? 0);
        }
    }

    public static bool operator ==(EnumDeclInfo left, EnumDeclInfo right) => left.Equals(right);

    public static bool operator !=(EnumDeclInfo left, EnumDeclInfo right) => !left.Equals(right);

    #endregion IEquality members
}
