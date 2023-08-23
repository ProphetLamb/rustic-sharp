using System;
using System.Reflection.Metadata;

using Rustic.Source;

namespace Rustic;

internal sealed class UnionAttributeBuilder {
    private readonly BuildOptions _options;
    private readonly SrcBuilder _b;

    internal UnionAttributeBuilder(SrcBuilder b, BuildOptions options) {
        _b = b;
        _options = options;
    }

    public void Build() {
        using (_ = _b.Decl($"namespace {_options.UnionAttributeNamespace}")) {
            BuildClass();
        }
    }

    private void BuildClass() {
        BuildDocs();
        using (SrcBuilder.SrcColl attrs = _b.AttrList()) {
            attrs.Add("System.AttributeUsageAttribute(System.AttributeTargets.All, Inherited = false)");
        }
        using (_ = _b.Decl($"public sealed class {_options.UnionAttributeName} : System.Attribute")) {
            BuildDocs();
            using (_ = _b.Decl($"public {_options.UnionAttributeName}", (ref SrcBuilder.SrcColl _) => {})) {
            }
        }
    }

    private void BuildDocs() {
        using (_ = _b.Doc("summary")) {
            _b.AppendDoc("Annotate a `partial` `class` or `struct´ to mark it for the Union source generator");
            _b.AppendDoc(
                $"Requires a `private static partial void {_options.SignatureMethodName}` method with the Types and names of the discriminated union."
            );
        }
    }
}
