namespace Rustic.DataEnumGenerator;

internal static class EnumExtensionsClass
{
    public static void Generate(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"{info.Modifiers} static class {info.EnumExtensionsName}");
        builder.AppendLine('{'); builder.Indent();

        Name(ref builder, in info);
        Description(ref builder, in info);
        Values(ref builder, in info);

        builder.Outdent(); builder.AppendLine('}');
    }

    private static void Name(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static string Name(this {info.EnumName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("switch (value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            builder.AppendLine($"case {info.EnumName}.{e.Name}:");
            builder.Indent();
            builder.AppendLine($"return nameof({info.EnumName}.{e.Name});");
            builder.Outdent();
        }

        builder.AppendLine("default:"); builder.Indent();
        builder.AppendLine("return value.ToString();"); builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void Description(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static string? Description(this {info.EnumName} value)");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("switch (value)");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            if (e.Description is not null)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:");
                builder.Indent();
                builder.AppendLine($"return \"{e.Description}\";");
                builder.Outdent();
            }
        }

        builder.AppendLine("default:");
        builder.Indent();
        builder.AppendLine("return null;");
        builder.Outdent();

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }

    private static void Values(ref SourceTextBuilder builder, in GeneratorInfo info)
    {
        builder.AppendLine($"public static ReadOnlySpan<{info.EnumName}> Values");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine("get");
        builder.AppendLine('{'); builder.Indent();

        builder.AppendLine($"return new {info.EnumName}[]");
        builder.AppendLine('{'); builder.Indent();

        foreach (var e in info.Members)
        {
            builder.AppendLine($"{info.EnumName}.{e.Name},");
        }

        builder.Outdent(); builder.AppendLine("};");

        builder.Outdent(); builder.AppendLine('}');

        builder.Outdent(); builder.AppendLine('}');
        builder.AppendLine();
    }
}
