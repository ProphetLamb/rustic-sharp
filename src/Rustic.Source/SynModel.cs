

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis;

namespace Rustic.Source;

[CLSCompliant(false)]
public static class SemCtxExtensions
{
    /// <summary>Casts the <see cref="SynModel"/> to <see cref="GeneratorSyntaxContext"/>.</summary>
    public static GeneratorSyntaxContext ToGenCtx(this SynModel ctx)
    {
        return Unsafe.As<SynModel, GeneratorSyntaxContext>(ref ctx);
    }

    public static string? GetTypeName(this SynModel model)
    {
       return model.Model.GetTypeName(model.Node);
    }

    public static string? GetTypeName<N>(this SynModel<N> model)
        where N : SyntaxNode
    {
        return model.Model.GetTypeName(model.Node);
    }

    public static SynModel GetSynModel(this Compilation comp, SyntaxNode node)
    {
        return new(node, comp.GetSemanticModel(node.SyntaxTree));
    }

    public static SynModel<N> GetSynModel<N>(this Compilation comp, N node)
        where N : SyntaxNode
    {
        return new(node, comp.GetSemanticModel(node.SyntaxTree));
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
    public bool Is<N>(out SynModel<N> model)
        where N : SyntaxNode
    {
        if (Node is N n)
        {

            model = new SynModel<N>(n, Model);
            return true;
        }

        model = default!;
        return false;
    }

    public SynModel<N> Sub<N>(N node)
        where N : SyntaxNode
    {
        return new SynModel<N>(node, Model);
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

    public SynModel<O> Sub<O>(O node)
        where O : SyntaxNode
    {
        return new SynModel<O>(node, Model);
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

    /// <summary>Casts the strong-typed <see cref="SynModel"/> to the weak-typed equivalent.</summary>
    public static implicit operator SynModel(in SynModel<N> model) => new(model.Node, model.Model);
}
