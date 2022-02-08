using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rustic.DataEnumGenerator;

internal readonly partial struct GenInfo
{
    public readonly NamespaceDeclarationSyntax Ns;
    public readonly ImmutableArray<BaseTypeDeclarationSyntax> Nesting;
    public readonly EnumDeclarationSyntax EnumDecl;
    public readonly ImmutableArray<EnumDeclInfo> Members;

    public readonly string Namespace;
    public readonly string Modifiers;
    public readonly string EnumName;
    public readonly string EnumValueName;
    public readonly string EnumExtensionsName;

    public GenInfo(NamespaceDeclarationSyntax ns, ImmutableArray<BaseTypeDeclarationSyntax> nesting,
        EnumDeclarationSyntax enumDecl, ImmutableArray<EnumDeclInfo> members)
    {
        Ns = ns;
        Nesting = nesting;
        EnumDecl = enumDecl;
        Members = members;
        Namespace = Ns.Name.ToString();
        Modifiers = EnumDecl.Modifiers.ToString();
        EnumName = EnumDecl.Identifier.Text;
        EnumValueName = $"{EnumDecl.Identifier.Text}Value";
        EnumExtensionsName = $"{EnumDecl.Identifier.Text}Extensions";
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

    public static void Generate(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine("#nullable enable");
        builder.AppendLine("using System;");
        builder.AppendLine("using System.ComponentModel;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using System.Runtime.Serialization;");
        builder.AppendLine("using System.Runtime.InteropServices;");
        builder.AppendLine();
        builder.AppendLine($"namespace {info.Namespace}");

        builder.BlockStart();

        EnumExtensionsClass.Generate(ref builder, in info);

        // Only if there is any DataEnum member.
        if (info.Members.Any(m => m.IsDataEnum))
        {
            builder.AppendLine();
            EnumValueStruct.Generate(ref builder, in info);
        }

        builder.BlockEnd();
        builder.AppendLine("#nullable restore");
    }
}
