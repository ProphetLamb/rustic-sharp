using System;

namespace Rustic.DataEnumGenerator;

internal ref struct SrcBuilder
{
    public SrcBuilder(Span<char> initialBuffer, int indentSize = 4)
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

    public void NoIndentLine(string text)
    {
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
    }

    public void NoIndentLine(ReadOnlySpan<char> text)
    {
        Builder.Append(text);
        Builder.Append(Environment.NewLine);
    }

    public void NoIndentLine(char text)
    {
        Builder.Append(text);
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

    #region Language specific

    public void BlockStart()
    {
        AppendLine('{');
        Indent();
    }

    public void BlockEnd()
    {
        Outdent();
        AppendLine('}');
        AppendLine();
    }

    public void Return(string? expression = null)
    {
        AppendIndent();
        Builder.Append("return ");
        Builder.Append(expression);
        Builder.Append(';');
        AppendLine();
    }

    public void StartIfBlock(string condition)
    {
        AppendIndent();
        Builder.Append("if (");
        Builder.Append(condition);
        Builder.Append(")");
        BlockStart();
    }

    public void StartSwitchBlock(string value)
    {
        AppendIndent();
        Builder.Append("switch (");
        Builder.Append(value);
        Builder.Append(")");
        BlockStart();
    }

    public void CaseStart(string? constant = null)
    {
        AppendIndent();
        if (constant is null)
        {
            Builder.Append("default:");
        }
        else
        {
            Builder.Append("case ");
            Builder.Append(constant);
            Builder.Append(":");
        }
        Indent();
        AppendLine();
    }

    public void CaseEnd()
    {
        Outdent();
        AppendLine("break;");
    }

    public void Region(string name)
    {
        AppendDoubleLine($"#region {name}");
    }

    public void EndRegion(string name)
    {
        AppendDoubleLine($"#endregion {name}");
    }

    #endregion Language specific
}
