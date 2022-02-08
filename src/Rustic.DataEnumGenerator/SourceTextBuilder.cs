using System;

namespace Rustic.DataEnumGenerator;

internal ref struct SourceTextBuilder
{
    public SourceTextBuilder(Span<char> initialBuffer, int indentSize = 4)
    {
        Builder = new StrBuilder(initialBuffer);
        IndentSize = indentSize;
        IndentLevel = 0;
    }

    public int IndentSize;
    public int IndentLevel;
    public StrBuilder Builder;

    public void AppendIndent()
    {
        Builder.Append(new string(' ', IndentLevel * IndentSize));
    }

    public void Append(string text)
    {
        Builder.Append(text);
    }

    public void Append(ReadOnlySpan<char> text)
    {
        Builder.Append(text);
    }

    public void Append(char text)
    {
        Builder.Append(text);
    }

    public void AppendLine(string text)
    {
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
    }

    public void AppendLine(ReadOnlySpan<char> text)
    {
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
    }

    public void AppendLine(char text)
    {
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
    }

    public void AppendLine()
    {
        AppendIndent();
        Builder.Append(Environment.NewLine);
    }

    public void AppendDoubleLine(string text)
    {
        Builder.Append(Environment.NewLine);
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
        Builder.Append(Environment.NewLine);
    }

    public void AppendDoubleLine(ReadOnlySpan<char> text)
    {
        Builder.Append(Environment.NewLine);
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
        Builder.Append(Environment.NewLine);
    }

    public void AppendDoubleLine(char text)
    {
        Builder.Append(Environment.NewLine);
        AppendIndent();
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
        Builder.Append(Environment.NewLine);
    }

    public void AppendDoubleLine()
    {
        Builder.Append(Environment.NewLine);
        AppendIndent();
        Builder.Append(Environment.NewLine);
        Builder.Append(Environment.NewLine);
    }

    public void Indent()
    {
        IndentLevel += 1;
    }

    public void Outdent()
    {
        var level = IndentLevel;
        if (level > 0)
        {
            IndentLevel = level - 1;
        }
    }

    public override string ToString()
    {
        IndentLevel = 0;
        return Builder.ToString();
    }
}
