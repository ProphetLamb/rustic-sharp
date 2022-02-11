

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis;

using static Rustic.Option;

namespace Rustic.Source;

[CLSCompliant(false)]
public static class SemCtxExtensions
{
    /// <summary>Casts the <see cref="SynModel"/> to <see cref="GeneratorSyntaxContext"/>.</summary>
    public static GeneratorSyntaxContext ToGenCtx(this SynModel ctx)
    {
        return Unsafe.As<SynModel, GeneratorSyntaxContext>(ref ctx);
    }

    /// <summary>Returns the <see cref="SynModel"/> for the node in the <see cref="Compilation"/>.</summary>
    public static SynModel GetSemCtx(this Compilation comp, SyntaxNode node)
    {
        return new(node, comp.GetSemanticModel(node.SyntaxTree));
    }

    public static string? GetTypeName(this SynModel model)
    {
        return model.GetTypeInfo().Type?.ToDisplayString();
    }

    public static string? GetTypeName<N>(this SynModel<N> model)
        where N : SyntaxNode
    {
        return model.GetTypeInfo().Type?.ToDisplayString();
    }
}

/// <summary>Weak-typed <see cref="SyntaxNode"/> and the associated <see cref="SemanticModel"/>.</summary>
[CLSCompliant(false)]
public readonly struct SynModel
{
    internal SynModel(SyntaxNode node, SemanticModel model)
    {
        Node = node;
        Model = model;
    }

    /// <summary>The <see cref="SyntaxNode"/>.</summary>
    public SyntaxNode Node { get; }

    /// <summary>The <see cref="SemanticModel"/> that can be queried to obtain information about <see cref="Node"/>.</summary>
    public SemanticModel Model { get; }

    /// <summary>Casts the strong-typed <see cref="SynModel"/> to the weak-typed equivalent.</summary>
    /// <typeparam name="N">The type of the <see cref="SyntaxNode"/>.</typeparam>
    public Option<SynModel<N>> As<N>()
        where N : SyntaxNode
    {
        return Node is N n ? Some(new SynModel<N>(n, Model)) : default;
    }

    /// <inheritdoc cref="ModelExtensions.GetDeclaredSymbol"/>
    public ISymbol? GetDeclaredSymbol(CancellationToken ct = default)
    {
        return Model.GetDeclaredSymbol(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetMemberGroup"/>
    public ImmutableArray<ISymbol> GetMemberGroup(CancellationToken ct = default)
    {
        return Model.GetMemberGroup(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetSymbolInfo"/>
    public SymbolInfo GetSymbolInfo(CancellationToken ct = default)
    {
        return Model.GetSymbolInfo(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetTypeInfo"/>
    public TypeInfo GetTypeInfo(CancellationToken ct = default)
    {
        return Model.GetTypeInfo(Node, ct);
    }

    /// <summary>Casts the <see cref="GeneratorSyntaxContext"/> to <see cref="SynModel"/>.</summary>
    public static implicit operator SynModel(GeneratorSyntaxContext ctx) => Unsafe.As<GeneratorSyntaxContext, SynModel>(ref ctx);
}

/// <summary>Strong-typed <see cref="SyntaxNode"/> and the associated <see cref="SemanticModel"/>.</summary>
/// <typeparam name="N">The type of the <see cref="SyntaxNode"/>.</typeparam>
[CLSCompliant(false)]
public readonly struct SynModel<N>
    where N : SyntaxNode
{
    internal SynModel(N node, SemanticModel model)
    {
        Node = node;
        Model = model;
    }

    /// <summary>The <see cref="SyntaxNode"/>.</summary>
    public N Node { get; }

    /// <summary>The <see cref="SemanticModel"/> that can be queried to obtain information about <see cref="Node"/>.</summary>
    public SemanticModel Model { get; }

    /// <inheritdoc cref="ModelExtensions.GetDeclaredSymbol"/>
    public ISymbol? GetDeclaredSymbol(CancellationToken ct = default)
    {
        return Model.GetDeclaredSymbol(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetMemberGroup"/>
    public ImmutableArray<ISymbol> GetMemberGroup(CancellationToken ct = default)
    {
        return Model.GetMemberGroup(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetSymbolInfo"/>
    public SymbolInfo GetSymbolInfo(CancellationToken ct = default)
    {
        return Model.GetSymbolInfo(Node, ct);
    }

    /// <inheritdoc cref="ModelExtensions.GetTypeInfo"/>
    public TypeInfo GetTypeInfo(CancellationToken ct = default)
    {
        return Model.GetTypeInfo(Node, ct);
    }

    /// <summary>Casts the strong-typed <see cref="SynModel"/> to the weak-typed equivalent.</summary>
    public static implicit operator SynModel(in SynModel<N> model) => new(model.Node, model.Model);
}
