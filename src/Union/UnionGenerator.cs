using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

using Rustic.Source;

namespace Rustic;

internal readonly record struct BuildOptions(string UnionAttributeNamespace, string UnionAttributeName, string SignatureMethodName) {
    public static BuildOptions Default => new("Rustic", "UnionAttribute", "UnionOf");

    public string UnionAttributeFullName { get; } = $"{UnionAttributeNamespace}.{UnionAttributeName}";
    public bool IsSignatureName(ReadOnlySpan<char> s) {
        return SignatureMethodName.AsSpan().Equals(s, StringComparison.Ordinal);
    }

    public bool IsAttributeTypeName(ReadOnlySpan<char> s) {
        return UnionAttributeFullName.AsSpan().Equals(s, StringComparison.Ordinal);
    }
}

internal readonly record struct UnionSourceSyntax(
    TypeDeclarationSyntax Type,
    MethodDeclarationSyntax Method,
    AttributeSyntax? Attribute);

internal readonly record struct UnionDescriptor(
    string NamespaceName,
    ImmutableArray<UsingDirective> NamespaceUsings,
    string TypeName,
    string MethodName,
    ImmutableArray<string> MethodModifiers,
    ImmutableArray<UnionVariant> Variants);

internal readonly record struct UsingDirective(bool Static, bool Global, string? Alias, string Name);

internal readonly record struct UnionVariant(
    string Name,
    string FullTypeName,
    bool IsTypeManaged);

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    private BuildOptions _options;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        _options = BuildOptions.Default;
        IncrementalValuesProvider<UnionSourceSyntax> unionDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            IsPartialTypeDeclaration,
            CollectUnionDeclaration
        );
        context.RegisterImplementationSourceOutput(unionDeclarations, GenerateSyntax);
    }

    private bool IsPartialTypeDeclaration(SyntaxNode node, CancellationToken ct) {
        if (node is not TypeDeclarationSyntax typeDeclaration) {
            return false;
        }

        if (typeDeclaration.Modifiers.FirstOrDefault(SyntaxKind.PartialKeyword) is null) {
            return false;
        }

        if (GetUnionSignature(typeDeclaration.Members) is not { } signature) {
            return false;
        }
        return true;
    }

    private MethodDeclarationSyntax? GetUnionSignature(SyntaxList<MemberDeclarationSyntax> members) {
        return members.OfType<MethodDeclarationSyntax>().FirstOrDefault(
            method => method.Modifiers.FirstOrDefault(SyntaxKind.PartialKeyword) is not null
             && _options.IsSignatureName(method.Identifier.Text)
        );
    }
    private UnionSourceSyntax CollectUnionDeclaration(GeneratorSyntaxContext ctx, CancellationToken ct) {
        SynModel<TypeDeclarationSyntax> type = ctx.SemanticModel.Compilation.GetSynModel((TypeDeclarationSyntax)ctx.Node!);
        SynModel<MethodDeclarationSyntax> signature = type.Sub(GetUnionSignature(type.Node.Members)!);
        AttributeSyntax? attr = type.FindTypeAttr((s, a) => _options.IsAttributeTypeName(s.Sub(a).GetTypeName()));
        return new(type.Node, signature.Node, attr);
    }

    private void GenerateSyntax(SourceProductionContext ctx, UnionSourceSyntax src) {
        if (!AnalyzeSourceSyntax(ctx, src)) {
            return;
        }

        BaseNamespaceDeclarationSyntax ns = GetHierarchy(src.Type).Ns!;
        string nsName = ns.Name.ToString();
        ImmutableArray<UsingDirective> nsUsings = ns.Usings.Select(u => new UsingDirective(u.StaticKeyword.RawKind != 0, u.GlobalKeyword.RawKind != 0, u.Alias?.Name.Identifier.Text, u.Name.ToString())).ToImmutableArray();
        string typeName = src.Type.Identifier.Text;
        string methodName = src.Method.Identifier.Text;
        ImmutableArray<string> methodModifiers = src.Method.Modifiers.Select(m => m.Text).ToImmutableArray();
        var variants = src.Method.ParameterList.Parameters.Select(p => new UnionVariant(p.Identifier.Text, p.Type!.ToString(), p.Type!.).ToImmutableArray();
    }

    private bool AnalyzeSourceSyntax(SourceProductionContext ctx, UnionSourceSyntax src) {
        DescriptorReporter diag = new(_options, ctx);
        if (src.Attribute is null) {
            diag.MissingUnionAttribute(src.Type.GetLocation(), src.Type.Identifier.Text);
            return false;
        }

        if (!IsPartialStatic(src.Method.Modifiers)) {
            diag.MethodNotPartialStatic(src.Method.GetLocation());
        }

        if (IsPublicOrInternal(src.Method.Modifiers)) {
            diag.MethodPublicOrInternal(src.Method.GetLocation());
        }

        (BaseNamespaceDeclarationSyntax? ns, ImmutableArray<SyntaxNode> hierarchy) = GetHierarchy(src.Type);
        if (!hierarchy.IsDefaultOrEmpty) {
            BaseTypeDeclarationSyntax? parent = hierarchy.FirstOrDefault() as BaseTypeDeclarationSyntax;
            diag.NestedTypeNotAllowed(src.Type.GetLocation(), src.Type.Identifier.Text, parent?.Identifier.Text);
        }

        if (ns is null) {
            diag.NamespaceRequired(src.Type.GetLocation(), src.Type.Identifier.Text);
        }

        if (src.Type.TypeParameterList is not null && src.Type.TypeParameterList.Parameters.Count != 0) {
            diag.GenericsDisallowed(src.Type.GetLocation(), src.Type.Identifier.Text);
        }

        if (src.Method.TypeParameterList is not null && src.Method.TypeParameterList.Parameters.Count != 0) {
            diag.GenericsDisallowed(src.Method.GetLocation(), src.Type.Identifier.Text);
        }

        return !diag.HasErrors;
    }

    private bool IsPartialStatic(SyntaxTokenList tokens) {
        return tokens.Count(tkn => tkn.Kind() is SyntaxKind.PartialKeyword or SyntaxKind.StaticKeyword) == 2;
    }

    private bool IsPublicOrInternal(SyntaxTokenList tokens) {
        return tokens.Count(tkn => tkn.Kind() is SyntaxKind.PublicKeyword or SyntaxKind.InternalKeyword) != 0;
    }
    
    public static (BaseNamespaceDeclarationSyntax? Ns, ImmutableArray<SyntaxNode> Hier) GetHierarchy(CSharpSyntaxNode node) {
        ImmutableArray<SyntaxNode>.Builder? nesting = ImmutableArray.CreateBuilder<SyntaxNode>(16);
        SyntaxNode? p = node;
        while ((p = p?.Parent) is not null) {
            switch (p) {
            case BaseNamespaceDeclarationSyntax ns:
                return (ns, nesting.ToImmutable());
            case MemberDeclarationSyntax member:
                nesting.Add(member);
                break;
            }
        }

        return default;
    }
}

internal readonly struct DescriptorReporter {
    private readonly BuildOptions _options;
    private readonly SourceProductionContext _ctx;
    private readonly List<Diagnostic> _reported;

    public DescriptorReporter(BuildOptions options, SourceProductionContext ctx) {
        _options = options;
        _ctx = ctx;
        _reported = new();
    }

    public int Errors => _reported.Count(d => d.Severity >= DiagnosticSeverity.Error);
    public bool HasErrors => Errors != 0;

    private void Report(Diagnostic d) {
        _ctx.ReportDiagnostic(d);
        _reported.Add(d);
    }

    public void MissingUnionAttribute(Location loc, string typeName) {
        Report(Diagnostic.Create(MissingUnionAttributeDescriptor, loc, typeName));
    }

    private DiagnosticDescriptor MissingUnionAttributeDescriptor => new("UU3001", $"Possibly missing {_options.UnionAttributeName} on type", $"Omitting the {_options.UnionAttributeName} prevents on {{0}} the source generator from processing the type.", _options.UnionAttributeFullName, DiagnosticSeverity.Info, true);

    public void MethodNotPartialStatic(Location loc) {
        Report(Diagnostic.Create(MethodNotPartialStaticDescriptor, loc));
    }

    private DiagnosticDescriptor MethodNotPartialStaticDescriptor => new("UU1001", $"Invalid modifiers on {_options.SignatureMethodName}", $"The {_options.SignatureMethodName} must be `static` and `partial`.", _options.UnionAttributeFullName, DiagnosticSeverity.Error, true);

    public void MethodPublicOrInternal(Location loc) {
        Report(Diagnostic.Create(MethodPublicOrInternalDescriptor, loc));
    }
    private DiagnosticDescriptor MethodPublicOrInternalDescriptor => new("UU1002", $"Invalid modifiers on {_options.SignatureMethodName}", $"The {_options.SignatureMethodName} must not be `public` or `internal`.", _options.UnionAttributeFullName, DiagnosticSeverity.Error, true);

    public void NestedTypeNotAllowed(Location loc, string typeName, string? parentTypeName) {
        Report(Diagnostic.Create(NestedTypeNotAllowedDescriptor, loc, typeName, string.IsNullOrEmpty(parentTypeName) ? "<unknown type>" : parentTypeName));
    }
    private DiagnosticDescriptor NestedTypeNotAllowedDescriptor => new("UU1003", "Nested types are disallowed", "The union type {0} cannot be nested in {1}.", _options.UnionAttributeFullName, DiagnosticSeverity.Error, true);

    public void NamespaceRequired(Location loc, string typeName) {
        Report(Diagnostic.Create(NamespaceRequiredDescriptor, loc, typeName));
    }
    private DiagnosticDescriptor NamespaceRequiredDescriptor => new("UU1004", $"Namespace required", "The union type {0} must be declared inside a namespace.", _options.UnionAttributeFullName, DiagnosticSeverity.Error, true);

    public void GenericsDisallowed(Location loc, string typeName) {
        Report(Diagnostic.Create(GenericsDisallowedDescriptor, loc, typeName));
    }

    private DiagnosticDescriptor GenericsDisallowedDescriptor => new("UU1005", "Generics disallowed", "The union type `{0}` cannot have generic type parameters", _options.UnionAttributeName, DiagnosticSeverity.Error, true);
}

