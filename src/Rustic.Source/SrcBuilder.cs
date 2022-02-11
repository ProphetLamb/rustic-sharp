using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Rustic.Source;

public class SrcBuilder
{
    private int _size;
    private int _level;
    private string _indent;

    public SrcBuilder(int initialCap, int indentSize = 4)
    {
        Builder = new StringBuilder(initialCap);
        _indent = String.Empty;
        SetIndent(indentSize, 0);
    }

    public StringBuilder Builder { get; }

    public int IndentSize
    {
        get => _size;
        set => SetIndent(value, _level);
    }

    public int IndentLevel
    {
        get => _level;
        set => SetIndent(_size, value);
    }

    public string IndentChars => _indent;

    public void SetIndent(int size, int level)
    {
        _size = size;
        _level = level;
        _indent = new string(' ', IndentLevel * IndentSize);
    }

    #region StringBuilder

    public SrcBuilder AppendIndent()
    {
        Builder.Append(IndentChars);
        return this;
    }

    public SrcBuilder AppendIndent(char text)
    {
        AppendIndent();
        Builder.Append(text);
        return this;
    }

    public SrcBuilder AppendIndent(string text)
    {
        AppendIndent();
        Builder.Append(text);
        return this;
    }

    public SrcBuilder Append(string? text)
    {
        if (!String.IsNullOrEmpty(text))
        {
            Builder.Append(text);
        }
        return this;
    }

    public SrcBuilder Append(ReadOnlySpan<char> text)
    {
        if (!text.IsEmpty)
        {
#if NETSTANDARD2_1_OR_GREATER || NETSTANDARD2_1
            Builder.Append(text);
#else
            Builder.Append(text.ToString());
#endif

        }
        return this;
    }

    public SrcBuilder Append(char text)
    {
        Builder.Append(text);
        return this;
    }

    public SrcBuilder AppendLine(string? text)
    {
        AppendIndent();
        if (!String.IsNullOrEmpty(text))
        {
            Builder.AppendLine(text);
        }
        else
        {
            NL();
        }
        return this;
    }

    public SrcBuilder AppendLine(char text)
    {
        AppendIndent();
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    public SrcBuilder AppendLine()
    {
        AppendIndent();
        Builder.AppendLine();
        return this;
    }

    public SrcBuilder NoIndentLine(string? text)
    {
        if (!String.IsNullOrEmpty(text))
        {
            Builder.AppendLine(text);
        }
        else
        {
            Builder.AppendLine();
        }
        return this;
    }

    public SrcBuilder NoIndentLine(char text)
    {
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    public SrcBuilder AppendDoubleLine(string? text)
    {
        Builder.AppendLine();
        AppendIndent();
        if (!String.IsNullOrEmpty(text))
        {
            Builder.AppendLine(text);
        }
        return this;
    }

    public SrcBuilder AppendDoubleLine(char text)
    {
        Builder.AppendLine();
        AppendIndent();
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    public SrcBuilder AppendDoubleLine()
    {
        Builder.AppendLine();
        AppendIndent();
        Builder.AppendLine();
        return this;
    }

    public override string ToString()
    {
        IndentLevel = 0;
        return Builder.ToString();
    }

    #endregion StringBuilder

    #region Source file

    public SrcBuilder IndentString()
    {
        IndentLevel += 1;
        return this;
    }

    public SrcBuilder Outdent()
    {
        var level = IndentLevel;
        if (level > 0)
        {
            IndentLevel = level - 1;
        }
        return this;
    }

    public SrcBuilder NL()
    {
        Builder.AppendLine();
        return this;
    }

    public SrcBuilder Stmt(string statement)
    {
        AppendLine(statement);
        return this;
    }

    public SrcBuilder BlockStart()
    {
        AppendLine('{');
        IndentString();
        return this;
    }

    public SrcBuilder BlockEnd()
    {
        Outdent();
        AppendLine('}');
        NL();
        return this;
    }

    #endregion Source file

    #region Csharp

    public SrcBuilder StartIfBlock(string condition)
    {
        AppendIndent();
        Builder.Append("if (").Append(condition).AppendLine(")");
        BlockStart();
        return this;
    }

    public SrcBuilder StartSwitchBlock(string value)
    {
        AppendIndent();
        Builder.Append("switch (").Append(value).AppendLine(")");
        BlockStart();
        return this;
    }

    public SrcBuilder CaseStart(string? constant = null)
    {
        AppendIndent();
        if (constant is null)
        {
            Builder.Append("default:");
        }
        else
        {
            Builder.Append("case ").Append(constant).Append(':');
        }
        IndentString();
        NL();
        return this;
    }

    public SrcBuilder CaseEnd()
    {
        AppendLine("break;");
        Outdent();
        return this;
    }

    public SrcBuilder StartRegion(string name)
    {
        AppendDoubleLine($"#region {name}");
        return this;
    }

    public SrcBuilder EndRegion(string name)
    {
        AppendDoubleLine($"#endregion {name}");
        return this;
    }

    public SrcBlock Block()
    {
        return new SrcBlock(this);
    }

    public SrcBlock If(string? condition)
    {
        AppendIndent();
        Builder.Append("if (").Append(condition).AppendLine(")");
        return Block();
    }

    public SrcBlock Decl(string declaration)
    {
        AppendLine(declaration);
        return Block();
    }

    public delegate void ParamsAction(ref SrcColl p);
    public delegate void ParamsAction<C>(in C ctx, ref SrcColl p);

    public SrcBlock Decl(string definition, ParamsAction parameters)
    {
        AppendIndent(definition);
        var p = Params();
        parameters(ref p);
        p.Dispose();
        return Block();
    }


    public SrcBlock Decl<C>(string definition, in C ctx, ParamsAction<C> parameters)
    {
        AppendIndent(definition);
        var p = Params();
        parameters(in ctx, ref p);
        p.Dispose();
        NL();
        return Block();
    }

    public SrcIndent Indented()
    {
        return new SrcIndent(this);
    }

    public SrcBlock Switch(string value)
    {
        AppendIndent();
        Builder.Append("switch (")
            .Append(value)
            .AppendLine(")");
        return Block();
    }

    public void Switch<T>(string value, ImmutableArray<T> branchSource, Func<T, string?> caseConstant, Func<SrcBuilder, T, bool> caseBlock, Func<SrcBuilder, bool>? defaultBlock = null)
    {
        using (Switch(value))
        {
            foreach (var b in branchSource)
            {
                string? constant = caseConstant(b);
                if (constant is not null)
                {
                    using var c = Case(constant);
                    var returned = caseBlock(this, b);
                    if (returned)
                    {
                        c.Returned();
                    }
                }
            }
            if (defaultBlock is not null)
            {
                using var c = Case(default);
                var returned = defaultBlock(this);
                if (returned)
                {
                    c.Returned();
                }
            }
        }
    }

    public void Switch<T, C>(string value, in C ctx, ImmutableArray<T> branchSource, Func<C, T, string?> caseConstant, Func<SrcBuilder, C, T, bool> caseBlock, Func<SrcBuilder, C, bool>? defaultBlock = null)
    {
        using (Switch(value))
        {
            foreach (var b in branchSource)
            {
                string? constant = caseConstant(ctx, b);
                if (constant is not null)
                {
                    using var c = Case(constant);
                    bool returned = caseBlock(this, ctx, b);
                    if (returned)
                    {
                        c.Returned();
                    }
                }
            }
            if (defaultBlock is not null)
            {
                using var c = Case(default);
                var returned = defaultBlock(this, ctx);
                if (returned)
                {
                    c.Returned();
                }
            }
        }
    }

    public void Switch<T>(string value, ImmutableArray<T> branchSource, Func<T, string?> caseConstant, Action<SrcBuilder, T> caseBlock, Action<SrcBuilder>? defaultBlock = null)
    {
        using (Switch(value))
        {
            foreach (var b in branchSource)
            {
                string? constant = caseConstant(b);
                if (constant is not null)
                {
                    using (Case(constant))
                    {
                        caseBlock(this, b);
                    }
                }
            }
            if (defaultBlock is not null)
            {
                using (Case(default))
                {
                    defaultBlock(this);
                }
            }
        }
    }

    public void Switch<T, C>(string value, in C ctx, ImmutableArray<T> branchSource, Func<C, T, string?> caseConstant, Action<SrcBuilder, C, T> caseBlock, Action<SrcBuilder, C>? defaultBlock = null)
    {
        using (Switch(value))
        {
            foreach (var b in branchSource)
            {
                string? constant = caseConstant(ctx, b);
                if (constant is not null)
                {
                    using (Case(constant))
                    {
                        caseBlock(this, ctx, b);
                    }
                }
            }
            if (defaultBlock is not null)
            {
                using (Case(default))
                {
                    defaultBlock(this, ctx);
                }
            }
        }
    }

    public SrcCase Case(string? constant)
    {
        return new SrcCase(this, constant);
    }

    public SrcColl Coll(string collection)
    {
        AppendIndent(collection);
        return new SrcColl(this,
            static (b) => b.NL().AppendLine('{').IndentString(),
            static (c, s) => c.Builder.AppendIndent().Append(s),
            static (b) => b.Append(',').NL(),
            static (b) => b.Append(',').NL().Outdent().AppendLine("};"));
    }

    public SrcColl Params()
    {
        return new SrcColl(this,
            static (b) => b.Append('('),
            static (c, s) => c.Builder.Append(s),
            static (b) => b.Append(", "),
            static (b) => b.Append(")"));
    }

    public SrcColl Call(string invocation)
    {
        AppendIndent(invocation);
        return new SrcColl(this,
            static (b) => b.Append('('),
            static (c, s) => c.Builder.Append(s),
            static (b) => b.Append(", "),
            static (b) => b.Append(");").NL());
    }

    public SrcPre InRegion(string name)
    {
        return new SrcPre(this, $"#region {name}", $"#endregion {name}");
    }

    public SrcPre InNullable()
    {
        return new SrcPre(this, "#nullable enable", "#nullable restore");
    }

    public struct SrcBlock : IDisposable
    {
        private bool _disposed;
        public SrcBuilder Builder { get; }

        public SrcBlock(SrcBuilder builder)
        {
            _disposed = false;
            Builder = builder;
            Builder.BlockStart();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.BlockEnd();
            }
            _disposed = true;
        }
    }

    public struct SrcIndent : IDisposable
    {
        private bool _disposed;
        public SrcBuilder Builder { get; }

        public SrcIndent(SrcBuilder builder)
        {
            _disposed = false;
            Builder = builder;
            Builder.IndentString();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.Outdent();
            }
            _disposed = true;
        }
    }

    public struct SrcCase : IDisposable
    {
        private bool _disposed;
        private bool _returned;
        public SrcBuilder Builder { get; }

        public SrcCase(SrcBuilder builder, string? constant = null)
        {
            _disposed = false;
            _returned = false;
            Builder = builder;
            Builder.CaseStart(constant);
        }

        public void Returned()
        {
            _returned = true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_returned)
                {
                    Builder.Outdent();
                }
                else
                {
                    Builder.CaseEnd();
                }
            }
            _disposed = true;
        }
    }

    public struct SrcColl : IDisposable, IEnumerable<string>
    {
        private bool _disposed;
        public SrcBuilder Builder { get; }
        public bool IsFirstElement { get; set; }
        public Action<SrcBuilder> Prefix { get; set; }
        public Action<SrcColl, string> Infix { get; }
        public Action<SrcBuilder> Separator { get; }
        public Action<SrcBuilder> Suffix { get; }

        public SrcColl(SrcBuilder builder, Action<SrcBuilder> prefix, Action<SrcColl, string> infix, Action<SrcBuilder> separator, Action<SrcBuilder> suffix)
        {
            _disposed = false;
            Builder = builder;
            Prefix = prefix;
            Infix = infix;
            Separator = separator;
            Suffix = suffix;

            Prefix(Builder);
            IsFirstElement = true;
        }

        public void Add(string parameter)
        {
            if (!IsFirstElement)
            {
                Separator(Builder);
            }

            IsFirstElement = false;
            Infix(this, parameter);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Suffix(Builder);
            }
            _disposed = true;
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield break; // Dummy implementation
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break; // Dummy implementation
        }
    }

    public struct SrcPre : IDisposable
    {
        private bool _disposed;
        public string Start { get; }
        public string End { get; }
        public SrcBuilder Builder { get; }

        public SrcPre(SrcBuilder builder, string start, string end)
        {
            _disposed = false;
            Start = start;
            End = end;
            Builder = builder;
            Builder.AppendDoubleLine(Start);
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.AppendDoubleLine(End);
            }
            _disposed = true;
        }
    }

    #endregion Csharp

    #region XML

    public SrcBuilder AppendDoc()
    {
        return AppendIndent("/// ");
    }

    public SrcBuilder AppendDoc(char text)
    {
        return AppendIndent("/// ").Append(text);
    }

    public SrcBuilder AppendDoc(string? text)
    {
        if (String.IsNullOrEmpty(text))
        {
            return AppendDoc();
        }

        return AppendIndent("/// ").Append(text);
    }

    public SrcBuilder DocStart(string elementName, string? attributes)
    {
        if (String.IsNullOrEmpty(attributes))
        {
            return AppendDoc($"<{elementName}>");
        }

        return AppendDoc($"<{elementName} {attributes}>");
    }

    public SrcBuilder DocEnd(string elementName)
    {
        return Append($"<{elementName}/>");
    }

    public SrcBuilder DocInline(string elementName, string? attributes = null, string? content = null)
    {
        if (String.IsNullOrEmpty(attributes))
        {
            Append('<').Append(elementName);
        }
        else
        {
            Append('<').Append(elementName).Append(' ').Append(attributes);
        }

        if (!String.IsNullOrEmpty(content))
        {
            Append('>').Append(content).Append('<').Append(elementName);
        }
        return Append("/>");
    }

    public SrcDoc Doc(string elementName, string? attributes = null)
    {
        return new SrcDoc(this, elementName, attributes);
    }

    public struct SrcDoc : IDisposable
    {
        private bool _disposed;

        public SrcBuilder Builder { get; }
        public string ElementName { get; }
        public string? Attributes { get; }

        public SrcDoc(SrcBuilder builder, string elementName, string? attributes)
        {
            _disposed = false;
            Builder = builder;
            ElementName = elementName;
            Attributes = attributes;

            Builder.DocStart(ElementName, Attributes);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.DocEnd(ElementName).NL();
            }
            _disposed = true;
        }
    }

    #endregion XML
}
