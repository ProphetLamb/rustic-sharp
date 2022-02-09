using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Rustic.Source;

namespace Rustic.DataEnumGenerator;

internal readonly struct GenInfo
{
    public readonly NamespaceDeclarationSyntax Ns;
    public readonly ImmutableArray<BaseTypeDeclarationSyntax> Nesting;
    public readonly EnumDeclarationSyntax EnumDecl;
    public readonly ImmutableArray<EnumDeclInfo> Members;
    public readonly ImmutableArray<EnumDeclInfo> DataMembers;

    public readonly string Namespace;
    public readonly string Modifiers;
    public readonly string EnumName;

    public GenInfo(NamespaceDeclarationSyntax ns, ImmutableArray<BaseTypeDeclarationSyntax> nesting,
        EnumDeclarationSyntax enumDecl, ImmutableArray<EnumDeclInfo> members)
    {
        Ns = ns;
        Nesting = nesting;
        EnumDecl = enumDecl;
        Members = members;
        DataMembers = members.Where(m => m.IsDataEnum).ToImmutableArray();
        Namespace = Ns.Name.ToString();
        Modifiers = EnumDecl.Modifiers.ToString();
        EnumName = EnumDecl.Identifier.Text;
    }

    public const string FlagsSymbol = "System.FlagsAttribute";

    public const string DataEnumSymbol = "Rustic.Attributes.DataEnumAttribute";

    public const string DescriptionSymbol = "System.ComponentModel.DescriptionAttribute";

    public const string DataEnumSource = @"#nullable enable
namespace Rustic.Attributes
{
    /// <summary>Allows a enum member to ship with additional data.</summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class DataEnumAttribute : System.Attribute
    {
        public DataEnumAttribute(System.Type data)
        {
            Data = data;
        }

        public System.Type Data { get; }
    }
}
#nullable restore";

    public static void Generate(SrcBuilder text, in GenInfo info)
    {
        using (text.InNullable())
        {
            text.AppendIndent("using System;")
                .Stmt("using System.ComponentModel;")
                .Stmt("using System.Collections.Generic;")
                .Stmt("using System.Runtime.CompilerServices;")
                .Stmt("using System.Runtime.Serialization;")
                .Stmt("using System.Runtime.InteropServices;")
                .NL();

            using (text.Decl($"namespace {info.Namespace}"))
            {
                EnumExtensionsClass.Generate(text, in info);

                // Only if there is any DataEnum member.
                if (info.Members.Any(m => m.IsDataEnum))
                {
                    text.NL();
                    EnumValueStruct.Generate(text, in info);
                }
            }
        }
    }
}
