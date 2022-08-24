using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Rustic.Source;

namespace Rustic.DataEnumGen;

[Flags]
internal enum EnumDeclFlags
{
    FlagsEnum = 1 << 0,
}

[DebuggerDisplay("{Symbol}")]
internal readonly struct GenContext
{
    public readonly EnumDeclFlags Flags;
    public readonly NamespaceDeclarationSyntax Ns;
    public readonly ImmutableArray<BaseTypeDeclarationSyntax> Nesting;
    public readonly EnumDeclarationSyntax EnumDecl;
    public readonly ImmutableArray<EnumContext> Members;

    public readonly string Namespace;
    public readonly string Modifiers;
    public readonly string Symbol;
    public readonly string EnumDataSymbol;

    public GenContext(EnumDeclFlags flags, NamespaceDeclarationSyntax ns, ImmutableArray<BaseTypeDeclarationSyntax> nesting,
        EnumDeclarationSyntax enumDecl, ImmutableArray<EnumContext> members)
    {
        Flags = flags;
        Ns = ns;
        Nesting = nesting;
        EnumDecl = enumDecl;
        Members = members;
        Namespace = Ns.Name.ToString();
        Modifiers = EnumDecl.Modifiers.ToString();
        Symbol = EnumDecl.Identifier.Text;
        EnumDataSymbol = GetEnumDataStructName(Symbol);
    }

    public bool IsFlags => (Flags & EnumDeclFlags.FlagsEnum) != 0;

    public MemberIter GetMembers() => new(this,false);
    public MemberIter GetDataMembers() => new(this, true);

    public bool Eq(in GenContext other) => EnumDecl == other.EnumDecl;

    private static string GetEnumDataStructName(string enumName)
    {
        // The enum suffix frees up the name of the enum for us to use.
        if (enumName.EndsWith("enum", StringComparison.OrdinalIgnoreCase))
        {
            string? name = enumName.Remove(enumName.Length - 4, 4);
            if (!String.IsNullOrWhiteSpace(name))
            {
                return name;
            }
        }

        return enumName + "Data";
    }

    public static GenContext? CollectDeclInfo(SynModel model)
    {
        if (!model.Is<EnumDeclarationSyntax>(out var enumModel))
        {
            return default;
        }

        EnumDeclFlags flags = 0;

        AttributeSyntax? flagsAttr = enumModel.FindTypeAttr(static (m, _) => m.GetTypeName() == Const.FlagsSymbol);
        flags |= flagsAttr is null ? 0 : EnumDeclFlags.FlagsEnum;

        EnumDeclarationSyntax? enumDecl = enumModel.Node;
        ImmutableArray<EnumContext>.Builder? members = ImmutableArray.CreateBuilder<EnumContext>(enumDecl.Members.Count);
        foreach (EnumMemberDeclarationSyntax? memberSyntax in enumDecl.Members)
        {
            if (memberSyntax is EnumMemberDeclarationSyntax memberDecl)
            {
                members.Add(CollectMemberInfo(model.Sub(memberDecl)));
            }
        }

        (NamespaceDeclarationSyntax? nsDecl, var nestingDecls) = enumDecl.GetHierarchy<BaseTypeDeclarationSyntax>();
        return new GenContext(flags, nsDecl, nestingDecls, enumDecl, members.MoveToImmutable());
    }

    public static EnumContext CollectMemberInfo(SynModel<EnumMemberDeclarationSyntax> model)
    {
        AttributeSyntax? dataEnumAttr = model.FindMemAttr(static (m, a) => m.Model.GetTypeName(a) == Const.DataEnumSymbol);
        TypeSyntax? dataType = null;
        AttributeArgumentSyntax? typeArg = dataEnumAttr?.ArgumentList?.Arguments[0];
        if (typeArg?.Expression is TypeOfExpressionSyntax tof)
        {
            dataType = tof.Type;
        }

        AttributeSyntax? descrAttr = model.FindMemAttr(static (m, a) => m.Model.GetTypeName(a) == Const.DescriptionSymbol);
        string? descr = null;
        AttributeArgumentSyntax? descrArg = descrAttr?.ArgumentList?.Arguments[0];
        if (descrArg?.Expression is LiteralExpressionSyntax literal)
        {
            descr = literal.Token.ValueText;
        }

        return new EnumContext(model, dataType, descr);
    }
}

internal struct MemberIter : IEnumerator<MemberContext>, IEnumerable<MemberContext>
{
    private GenContext _gen;
    private bool _dataMembersOnly;
    private int _pos;

    public MemberIter(GenContext ctx, bool dataMembersOnly)
    {
        _gen = ctx;
        _dataMembersOnly = dataMembersOnly;
        _pos = -1;
    }

    public MemberContext Current => new(_gen, _gen.Members[_pos]);

    object IEnumerator.Current => Current;

    public MemberIter GetEnumerator()
    {
        return new MemberIter(_gen, _dataMembersOnly);
    }

    IEnumerator<MemberContext> IEnumerable<MemberContext>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext()
    {
        while ((_pos += 1) < _gen.Members.Length)
        {
            Debug.Assert(_pos < _gen.Members.Length);

            if (!_dataMembersOnly || _gen.Members[_pos].IsDataEnum)
            {
                return true;
            }
        }

        return false;
    }

    public void Reset()
    {
        _pos = -1;
    }

    public void Dispose()
    {
        this = default;
    }
}

[DebuggerDisplay("{Symbol}")]
internal readonly struct EnumContext
{
    public readonly SynModel<EnumMemberDeclarationSyntax> Enum;
    public readonly TypeSyntax? TypeNode;
    public readonly string? Description;

    public readonly string Symbol;
    public readonly string NameLower;
    public readonly string? TypeName;

    public EnumContext(SynModel<EnumMemberDeclarationSyntax> enumModel, TypeSyntax? typeNode, string? description)
    {
        Enum = enumModel;
        TypeNode = typeNode;
        Description = description;

        Symbol = enumModel.Node.Identifier.Text;
        NameLower = Symbol.ToLower();
        TypeName = typeNode?.ToString();
    }

    public bool IsDataEnum => TypeNode is not null;

    public bool Eq(in EnumContext other) => Enum.Eq(other.Enum) && ReferenceEquals(TypeNode, other.TypeNode);
}

[DebuggerDisplay("{Gen.Symbol}.{Mem.Symbol}")]
internal readonly struct MemberContext
{
    public readonly GenContext Gen;
    public readonly EnumContext Mem;

    public MemberContext(GenContext generator, EnumContext member)
    {
        Gen = generator;
        Mem = member;
    }

    public string Symbol => $"{Gen.Symbol}.{Mem.Symbol}";

    public bool Eq(in MemberContext other) => Gen.Eq(in other.Gen) && Mem.Eq(in other.Mem);
}
