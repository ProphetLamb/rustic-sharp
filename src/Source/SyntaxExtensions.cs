using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rustic.Source;

/// <summary>Syntax extensions.</summary>
[CLSCompliant(false)]
public static class SyntaxExtensions {
    /// <summary>Attempts to find the a attribute attached to a <see cref="MemberDeclarationSyntax"/> node.</summary>
    /// <param name="member">The member declaration.</param>
    /// <param name="predicate">The function determining whether an attribute matches, or not.</param>
    /// <typeparam name="M">The type of the <see cref="MemberDeclarationSyntax"/>.</typeparam>
    /// <returns>The first <see cref="AttributeSyntax"/> found, if any.</returns>
    public static AttributeSyntax? FindMemAttr<M>(
        in this SynModel<M> member,
        Func<SynModel<M>, AttributeSyntax, bool> predicate)
        where M : MemberDeclarationSyntax {
        foreach (AttributeListSyntax? attrListSyntax in member.Node.AttributeLists) {
            foreach (AttributeSyntax? attrSyntax in attrListSyntax.Attributes) {
                if (predicate(member, attrSyntax)) {
                    return attrSyntax;
                }
            }
        }

        return null;
    }

    /// <summary>Attempts to find the a attribute attached to a <see cref="BaseTypeDeclarationSyntax"/> node.</summary>
    /// <param name="type">The member declaration.</param>
    /// <param name="predicate">The function determining whether an attribute matches, or not.</param>
    /// <typeparam name="T">The type of the <see cref="BaseTypeDeclarationSyntax"/>.</typeparam>
    /// <returns>The first <see cref="AttributeSyntax"/> found, if any.</returns>
    public static AttributeSyntax? FindTypeAttr<T>(
        in this SynModel<T> type,
        Func<SynModel<T>, AttributeSyntax, bool> predicate)
        where T : BaseTypeDeclarationSyntax {
        foreach (AttributeListSyntax? attrListSyntax in type.Node.AttributeLists) {
            foreach (AttributeSyntax? attrSyntax in attrListSyntax.Attributes) {
                if (predicate(type, attrSyntax)) {
                    return attrSyntax;
                }
            }
        }

        return null;
    }

    /// <summary>Determines the trace from the node to the <see cref="NamespaceDeclarationSyntax"/>, by traversing the parents of the node.</summary>
    /// <param name="node">The node</param>
    /// <typeparam name="P">The type of parents allowed in the trace. Throws if the type of any parent doesnt match.</typeparam>
    /// <exception cref="InvalidOperationException">The type of a node is not assignable to <typeparamref name="P"/>.</exception>
    public static (NamespaceDeclarationSyntax, ImmutableArray<P>) GetHierarchy<P>(this CSharpSyntaxNode node)
        where P : MemberDeclarationSyntax {
        ImmutableArray<P>.Builder? nesting = ImmutableArray.CreateBuilder<P>(16);
        SyntaxNode? p = node;
        while ((p = p?.Parent) is not null) {
            switch (p) {
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

    /// <summary>Returns the type name of the node inside the model.</summary>
    /// <param name="model">The model</param>
    /// <param name="node">The node in the scope of the model.</param>
    public static string? GetTypeName(this SemanticModel model, SyntaxNode node) {
        // Are we a type?
        TypeInfo typeInfo = model.GetTypeInfo(node);
        if (typeInfo.Type is not null) {
            return typeInfo.Type.ToDisplayString();
        }

        ISymbol? decl = model.GetDeclaredSymbol(node);
        // Are we of a type?
        if (decl?.ContainingType is not null) {
            return decl.ContainingType.ToDisplayString();
        }

        // Do we have any symbol at all?
        return decl?.ToDisplayString();
    }

    /// <summary>Traverses all roots inside the <see cref="Compilation"/>.</summary>
    /// <param name="comp">The compilation to filter.</param>
    /// <param name="predicate">The filter for relevant nodes.</param>
    /// <param name="transform">The final transformation to apply to relevant nodes.</param>
    /// <typeparam name="T">The type of elements produces by the transformation.</typeparam>
    /// <returns>All filtered and transformed nodes inside any root inside the <see cref="Compilation"/>.</returns>
    public static IEnumerable<T> CollectSyntax<T>(
        this Compilation comp,
        Func<SyntaxNode, CancellationToken, bool> predicate,
        Func<Compilation, SyntaxNode, CancellationToken, T> transform) {
        foreach (SyntaxTree? tree in comp.SyntaxTrees) {
            CancellationToken ct = new();
            if (tree.TryGetRoot(out SyntaxNode? root)) {
                Stack<SyntaxNode> stack = new(64);
                stack.Push(root);

                SyntaxNode node;
                while ((node = stack.Pop()) is not null) {
                    foreach (SyntaxNodeOrToken child in node.ChildNodesAndTokens()) {
                        if (child.IsNode) {
                            stack.Push((SyntaxNode) child!);
                        }
                    }

                    if (predicate(node, ct)) {
                        yield return transform(comp, node, ct);
                    }

                    if (ct.IsCancellationRequested) {
                        break;
                    }
                }
            }
        }
    }
}
