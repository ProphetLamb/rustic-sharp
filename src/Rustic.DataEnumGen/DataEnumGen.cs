using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Rustic.Source;

using static Rustic.Option;

namespace Rustic.DataEnumGen;

#if !FEATURE_INC_SRC_GEN

[Generator(LanguageNames.CSharp)]
[CLSCompliant(false)]
public class DataEnumGen : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(static (ctx) =>
            ctx.AddSource($"{GenContext.DataEnumSymbol}.g.cs", SourceText.From(GenContext.DataEnumSyntax, Encoding.UTF8)));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var enumDecls = context.CollectSyntax(
                 static (s, _) => s is EnumDeclarationSyntax,
                 static (ctx, s, _) => GenContext.CollectEnumDeclInfo(ctx.Compilation.GetSemCtx(s)))
             .FilterMap(static (m) => m);

        Generate(context, enumDecls.ToImmutableArray());
    }

    private static void Generate(GeneratorExecutionContext context, ImmutableArray<GenContext> members)
    {
        if (members.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var info in members.Distinct())
        {
            SrcBuilder text = new(2048);
            GenContext.Generate(text, in info);
            context.AddSource($"{info.EnumName}Value.g.cs", SourceText.From(text.ToString(), Encoding.UTF8));
        }
    }
}

#else

[Generator(LanguageNames.CSharp)]
[CLSCompliant(false)]
public class DataEnumGen : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static (ctx) =>
            ctx.AddSource($"{GenContext.DataEnumSymbol}.g.cs", SourceText.From(GenContext.DataEnumSyntax, Encoding.UTF8)));

        var enumDecls = context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) => s is EnumDeclarationSyntax,
                static (ctx, _) => GenContext.CollectEnumDeclInfo(ctx))
            .Where(static (m) => m.IsSome)
            .Select(static (m, _) => m.SomeUnchecked());

        context.RegisterSourceOutput(enumDecls.Collect(), static (ctx, src) => Generate(ctx, src));
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<GenContext> members)
    {
        if (members.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var info in members.Distinct())
        {
            SrcBuilder text = new(2048);
            GenContext.Generate(text, in info);
            context.AddSource($"{info.EnumName}Value.g.cs", SourceText.From(text.ToString(), Encoding.UTF8));
        }
    }
}

#endif

[CLSCompliant(false)]
internal readonly struct GenContext
{
    public readonly NamespaceDeclarationSyntax Ns;
    public readonly ImmutableArray<BaseTypeDeclarationSyntax> Nesting;
    public readonly EnumDeclarationSyntax EnumDecl;
    public readonly ImmutableArray<EnumContext> Members;
    public readonly ImmutableArray<EnumContext> DataMembers;

    public readonly string Namespace;
    public readonly string Modifiers;
    public readonly string EnumName;

    public GenContext(NamespaceDeclarationSyntax ns, ImmutableArray<BaseTypeDeclarationSyntax> nesting,
        EnumDeclarationSyntax enumDecl, ImmutableArray<EnumContext> members)
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

    public const string DataEnumSymbol = "Rustic.DataEnumAttribute";

    public const string DescriptionSymbol = "System.ComponentModel.DescriptionAttribute";

    public const string DataEnumSyntax = @"#nullable enable
using System;

namespace Rustic
{
    /// <summary>Allows a enum member to ship with additional data.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DataEnumAttribute : Attribute
    {
        public DataEnumAttribute(Type data)
        {
            Data = data;
        }

        public Type Data { get; }
    }
}
#nullable restore
";

    public static Option<GenContext> CollectEnumDeclInfo(SynModel model)
    {
        if (!model.As<EnumDeclarationSyntax>().TrySome(out var enumModel))
        {
            return default;
        }

        if (enumModel.FindTypeAttr(static (m, _) => m.GetTypeName() == FlagsSymbol) is not null)
        {
            // Flags enum are not allowed!
            return default;
        }

        var enumDecl = enumModel.Node;
        var members = ImmutableArray.CreateBuilder<EnumContext>(enumDecl.Members.Count);
        foreach (var memberSyntax in enumDecl.Members)
        {
            if (memberSyntax is EnumMemberDeclarationSyntax memberDecl)
            {
                members.Add(CollectEnumDeclInfo(enumModel));
            }
        }

        var (nsDecl, nestingDecls) = enumDecl.GetHierarchy<BaseTypeDeclarationSyntax>();
        return Some(new GenContext(nsDecl, nestingDecls, enumDecl, members.MoveToImmutable()));
    }

    public static EnumContext CollectEnumDeclInfo(SynModel<EnumDeclarationSyntax> model)
    {
        var dataEnumAttr = model.FindMemAttr(static (m, _) => m.GetTypeName() == DataEnumSymbol);
        TypeSyntax? dataType = null;
        var typeArg = dataEnumAttr?.ArgumentList?.Arguments[0];
        if (typeArg?.Expression is TypeOfExpressionSyntax tof)
        {
            dataType = tof.Type;
        }

        var descrAttr = model.FindMemAttr(static (m, _) => m.GetTypeName() == DescriptionSymbol);
        string? descr = null;
        var descrArg = descrAttr?.ArgumentList?.Arguments[0];
        if (descrArg?.Expression is LiteralExpressionSyntax literal)
        {
            descr = literal.Token.ValueText;
        }

        return new EnumContext(model, dataType, descr);
    }

    public static void Generate(SrcBuilder text, in GenContext info)
    {
        using (text.InNullable())
        {
            text.Stmt("using System;")
                .Stmt("using System.ComponentModel;")
                .Stmt("using System.Collections.Generic;")
                .Stmt("using System.Runtime.CompilerServices;")
                .Stmt("using System.Runtime.Serialization;")
                .Stmt("using System.Runtime.InteropServices;")
                .NL()
                .Stmt("using Rustic;")
                .NL()
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

[CLSCompliant(false)]
public readonly struct EnumContext : IEquatable<EnumContext>
{
    public readonly SynModel<EnumDeclarationSyntax> Enum;
    public readonly TypeSyntax? TypeNode;
    public readonly string? Description;

    public readonly string Name;
    public readonly string NameLower;
    public readonly string? TypeName;

    public EnumContext(SynModel<EnumDeclarationSyntax> enumModel, TypeSyntax? typeNode, string? description)
    {
        Enum = enumModel;
        TypeNode = typeNode;
        Description = description;

        Name = enumModel.Node.Identifier.Text;
        NameLower = Name.ToLower();
        TypeName = TypeNode?.ToString();
    }

    public bool IsDataEnum => TypeNode is not null;

    #region IEquality members

    public bool Equals(EnumContext other)
    {
        return Enum.Equals(other.Enum) && Equals(TypeNode, other.TypeNode);
    }

    public override bool Equals(object? obj)
    {
        return obj is EnumContext other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Enum.GetHashCode() * 397) ^ ((TypeNode?.GetHashCode()) ?? 0);
        }
    }

    public static bool operator ==(EnumContext left, EnumContext right) => left.Equals(right);

    public static bool operator !=(EnumContext left, EnumContext right) => !left.Equals(right);

    #endregion IEquality members
}
