using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rustic.Source;

[CLSCompliant(false)]
public static class SyntaxExtensions
{
    public static AttributeSyntax? FindMemAttr<M>(this SynModel<M> member, Func<SynModel<M>, AttributeSyntax, bool> predicate)
        where M : MemberDeclarationSyntax
    {
        foreach (var attrListSyntax in member.Node.AttributeLists)
        {
            foreach (var attrSyntax in attrListSyntax.Attributes)
            {
                if (predicate(member, attrSyntax))
                {
                    return attrSyntax;
                }
            }
        }

        return null;
    }

    public static AttributeSyntax? FindTypeAttr<T>(this SynModel<T> type, Func<SynModel<T>, AttributeSyntax, bool> predicate)
        where T : BaseTypeDeclarationSyntax
    {
        foreach (var attrListSyntax in type.Node.AttributeLists)
        {
            foreach (var attrSyntax in attrListSyntax.Attributes)
            {
                if (predicate(type, attrSyntax))
                {
                    return attrSyntax;
                }
            }
        }

        return null;
    }

    public static (NamespaceDeclarationSyntax, ImmutableArray<P>) GetHierarchy<P>(this CSharpSyntaxNode node)
        where P : MemberDeclarationSyntax
    {
        var nesting = ImmutableArray.CreateBuilder<P>(16);
        SyntaxNode? p = node;
        while ((p = p?.Parent) is not null)
        {
            switch (p)
            {
                case P member:
                    nesting.Add(member);
                    break;
                case NamespaceDeclarationSyntax ns:
                    return (ns, nesting.ToImmutable());
                default:
                    throw new InvalidOperationException($"{p.GetType().Name} is not allowed in the hierarchy.");
            }
        }

        throw new InvalidOperationException("No namespace declaration found.");
    }

    public static IEnumerable<T> CollectSyntax<T>(this GeneratorExecutionContext ctx, Func<SyntaxNode, CancellationToken, bool> predicate, Func<GeneratorExecutionContext, SyntaxNode, CancellationToken, T> transform)
    {
        foreach (var tree in ctx.Compilation.SyntaxTrees)
        {
            CancellationToken ct = new();
            if (tree.TryGetRoot(out var root))
            {
                foreach (var node in root.ChildNodes())
                {
                    if (predicate(node, ct))
                    {
                        yield return transform(ctx, node, ct);
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            if (ct.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
