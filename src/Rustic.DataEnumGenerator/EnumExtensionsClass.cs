namespace Rustic.DataEnumGenerator;

internal static class EnumExtensionsClass
{
    public static void Generate(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"{info.Modifiers} static class {info.EnumExtensionsName}");
        builder.BlockStart();

        Name(ref builder, in info);
        Description(ref builder, in info);
        Values(ref builder, in info);

        builder.BlockEnd();
    }

    private static void Name(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"public static string Name(this {info.EnumName} value)");
        builder.BlockStart();

        builder.StartSwitchBlock("value");

        foreach (var e in info.Members)
        {
            builder.AppendLine($"case {info.EnumName}.{e.Name}:");
            builder.Indent();
            builder.Return($"nameof({info.EnumName}.{e.Name})");
            builder.Outdent();
        }

        builder.AppendLine("default:"); builder.Indent();
        builder.Return("value.ToString()"); builder.Outdent();

        builder.BlockEnd();

        builder.BlockEnd();
    }

    private static void Description(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"public static string? Description(this {info.EnumName} value)");
        builder.BlockStart();

        builder.StartSwitchBlock("value");

        foreach (var e in info.Members)
        {
            if (e.Description is not null)
            {
                builder.AppendLine($"case {info.EnumName}.{e.Name}:");
                builder.Indent();
                builder.Return($"\"{e.Description}\"");
                builder.Outdent();
            }
        }

        builder.AppendLine("default:");
        builder.Indent();
        builder.Return("null");
        builder.Outdent();

        builder.BlockEnd();

        builder.BlockEnd();
    }

    private static void Values(ref SrcBuilder builder, in GenInfo info)
    {
        builder.AppendLine($"public static ReadOnlySpan<{info.EnumName}> Values");
        builder.BlockStart();

        builder.AppendLine("get");
        builder.BlockStart();

        builder.AppendLine($"return new {info.EnumName}[]");
        builder.BlockStart();

        foreach (var e in info.Members)
        {
            builder.AppendLine($"{info.EnumName}.{e.Name},");
        }

        builder.Outdent(); builder.AppendLine("};");

        builder.BlockEnd();

        builder.BlockEnd();
    }
}
