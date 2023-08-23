using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

using Rustic.Source;

namespace Rustic;
internal static class Extensions {
    /// <summary>Adds a attribute list. Returns a fluent builder used to defined the attribute elements.</summary>
    /// <param name="attributeTarget">The attribute target</param>
    /// <example>[attributeTarget: Attribute1, Attribute2("Parameter")]</example>
    public static SrcBuilder.SrcColl AttrList(this SrcBuilder b, string? attributeTarget = null) {
        b.AppendIndent();
        return new SrcBuilder.SrcColl(
            b,
            b => {
                b.Append('[');
                if (!string.IsNullOrEmpty(attributeTarget)) {
                    b.Append(attributeTarget).Append(": ");
                }
            },
            static (c, s) => c.Builder.Append(s),
            static b => b.Append(", "),
            static b => b.Append(']')
        );
    }
    public static SyntaxToken? FirstOrDefault(this SyntaxTokenList tokens, SyntaxKind kind) {
        foreach (SyntaxToken t in tokens) {
            if (t.IsKind(kind)) {
                return t;
            }
        }

        return default;
    }
}
