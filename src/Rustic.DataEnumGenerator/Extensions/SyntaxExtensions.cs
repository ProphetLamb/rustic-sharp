using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rustic.DataEnumGenerator.Extensions;

internal static class SyntaxExtensions
{
    public static AttributeSyntax? FindAttribute(this MemberDeclarationSyntax member, GeneratorSyntaxContext context, Func<AttributeSyntax, GeneratorSyntaxContext, bool> selector)
    {
        foreach (var attrListSyntax in member.AttributeLists)
        {
            foreach (var attrSyntax in attrListSyntax.Attributes)
            {
                if (selector(attrSyntax, context))
                {
                    return attrSyntax;
                }
            }
        }

        return null;
    }

    public static AttributeSyntax? FindAttribute(this BaseTypeDeclarationSyntax type, GeneratorSyntaxContext context, Func<AttributeSyntax, GeneratorSyntaxContext, bool> selector)
    {
        foreach (var attrListSyntax in type.AttributeLists)
        {
            foreach (var attrSyntax in attrListSyntax.Attributes)
            {
                if (selector(attrSyntax, context))
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
}
