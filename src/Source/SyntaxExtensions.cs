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
        foreach (AttributeListSyntax? attrListSyntax in member.Node.AttributeLists)
        {
            foreach (AttributeSyntax? attrSyntax in attrListSyntax.Attributes)
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
        foreach (AttributeListSyntax? attrListSyntax in type.Node.AttributeLists)
        {
            foreach (AttributeSyntax? attrSyntax in attrListSyntax.Attributes)
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
        ImmutableArray<P>.Builder? nesting = ImmutableArray.CreateBuilder<P>(16);
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

    public static string? GetTypeName(this SemanticModel model, SyntaxNode node)
    {
        // Are we a type?
        TypeInfo typeInfo = model.GetTypeInfo(node);
        if (typeInfo.Type is not null)
        {
            return typeInfo.Type.ToDisplayString();
        }

        ISymbol? decl = model.GetDeclaredSymbol(node);
        // Are we of a type?
        if (decl?.ContainingType is not null)
        {
            return decl.ContainingType.ToDisplayString();
        }
        // Do we have any symbol at all?
        return decl?.ToDisplayString();
    }

    public static IEnumerable<T> CollectSyntax<T>(this Compilation comp, Func<SyntaxNode, CancellationToken, bool> predicate, Func<Compilation, SyntaxNode, CancellationToken, T> transform)
    {
        foreach (SyntaxTree? tree in comp.SyntaxTrees)
        {
            CancellationToken ct = new();
            if (tree.TryGetRoot(out SyntaxNode? root))
            {
                Stack<SyntaxNode> stack = new(64);
                stack.Push(root);

                SyntaxNode node;
                while ((node = stack.Pop()) is not null)
                {
                    foreach (SyntaxNodeOrToken child in node.ChildNodesAndTokens())
                    {
                        if (child.IsNode)
                        {
                            stack.Push((SyntaxNode)child!);
                        }
                    }

                    if (predicate(node, ct))
                    {
                        yield return transform(comp, node, ct);
                    }
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }
    }
}
