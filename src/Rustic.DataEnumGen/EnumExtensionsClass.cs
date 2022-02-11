using Rustic.Source;

namespace Rustic.DataEnumGen;

internal static class EnumExtensionsClass
{
    public static void Generate(SrcBuilder text, in GenContext info)
    {
        using (text.Doc("summary"))
        {
            text.Append($"Collection of extensions for the {info.EnumName} Enum.");
        }
        using (text.Decl($"{info.Modifiers} static class {info.EnumName}Extensions"))
        {
            Name(text, in info);
            Description(text, in info);
            Values(text, in info);
        }
    }

    private static void Name(SrcBuilder text, in GenContext info)
    {
        using (text.Doc("summary"))
        {
            text.Append($"Returns the name of the <see cref=\"{info.EnumName}\" value.");
        }
        using (text.Decl($"public static string Name(this {info.EnumName} value)"))
        {
            text.Switch("value", in info, info.Members,
                static (ctx, current) => $"{ctx.EnumName}.{current.Name}",
                static (t, ctx, current) =>
                {
                    t.Stmt($"return nameof({ctx.EnumName}.{current.Name});");
                    return true;
                },
                static (t, _) =>
                {
                    t.Stmt("return value.ToString();");
                    return true;
                });
        }
    }

    private static void Description(SrcBuilder text, in GenContext info)
    {
        using (text.Doc("summary"))
        {
            text.Append($"Returns the value of the <see cref=\"System.ComponentModel.DescriptionAttribute\"/> of <see cref=\"{info.EnumName}\" value.");
        }
        using (text.Decl($"public static string? Description(this {info.EnumName} value)"))
        {
            text.Switch("value", in info, info.Members,
                static (ctx, current) => current.Description is null ? null : $"{ctx.EnumName}.{current.Name}",
                static (t, _, current) =>
                {
                    t.Stmt($"return \"{current.Description}\";");
                    return true;
                },
                static (t, _) =>
                {
                    t.Stmt("return null;");
                    return true;
                });
        }
    }

    private static void Values(SrcBuilder text, in GenContext info)
    {
        using (text.Doc("summary"))
        {
            text.Append($"Returns the span of all possible values of <see cref=\"{info.EnumName}\".");
        }
        using (text.Decl($"public static ReadOnlySpan<{info.EnumName}> Values"))
        {
            text.Stmt("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            using (text.Decl("get"))
            {
                using var array = text.Coll($"return new {info.EnumName}[] ");
                foreach (var e in info.Members)
                {
                    array.Add($"{info.EnumName}.{e.Name}");
                }
            }
        }
    }
}
