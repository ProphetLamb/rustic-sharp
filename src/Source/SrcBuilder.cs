using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rustic.Source;

/// <summary>Determines if a case block requires breaking or it is already returned.</summary>
[Flags]
public enum CaseStyle
{
    /// <summary>Default style. Adds `break;` statement</summary>
    None = 0,
    /// <summary>no `break;` statement.</summary>
    NoBreak = 1 << 0,
}

/// <summary>Fluent builder for producing csharp source code as text.</summary>
public class SrcBuilder
{
    private int _size;
    private int _level;
    private string _indent;

    /// <summary>Initializes a new instance of <see cref="SrcBuilder"/>.</summary>
    /// <param name="initialCap">The initial capacity of the string buffer.</param>
    /// <param name="indentSize">The number of `space` characters used to indent a level.</param>
    public SrcBuilder(int initialCap, int indentSize = 4)
    {
        Builder = new StringBuilder(initialCap);
        _indent = String.Empty;
        SetIndent(indentSize, 0);
    }

    /// <summary>The <see cref="StringBuilder"/> used within the <see cref="SrcBuilder"/>.</summary>
    public StringBuilder Builder { get; }

    /// <summary>Specifies the number of `space` characters used to indent a level.</summary>
    public int IndentSize
    {
        get => _size;
        set => SetIndent(value, _level);
    }

    /// <summary>Specifies the number of indentation levels from the beginning of the line.</summary>
    public int IndentLevel
    {
        get => _level;
        set => SetIndent(_size, value);
    }

    /// <summary>Represents the indentation as defined by <see cref="IndentSize"/> and <see cref="IndentLevel"/>.</summary>
    public string IndentChars => _indent;

    /// <summary>Sets the indent size and level to the specifed values.</summary>
    /// <param name="size">The number of `space` characters used to indent a level.</param>
    /// <param name="level">The number of indentation levels from the beginning of the line.</param>
    public void SetIndent(int size, int level)
    {
        _size = size;
        _level = level;
        _indent = new string(' ', IndentLevel * IndentSize);
    }

    #region StringBuilder

    /// <inheritdoc cref="AppendIndent(string)"/>
    public SrcBuilder AppendIndent()
    {
        Builder.Append(IndentChars);
        return this;
    }

    /// <inheritdoc cref="AppendIndent(string)"/>
    public SrcBuilder AppendIndent(char text)
    {
        AppendIndent();
        Builder.Append(text);
        return this;
    }

    /// <summary>Appends the <see cref="IndentChars"/> to the builder.</summary>
    /// <param name="text">The text to append after the indent.</param>
    public SrcBuilder AppendIndent(string text)
    {
        AppendIndent();
        Builder.Append(text);
        return this;
    }

    /// <summary>Appends the value to the builder, with no indentation, equivalent to <see cref="StringBuilder.Append(string)"/></summary>
    /// <param name="text">The text to append</param>
    public SrcBuilder Append(string? text)
    {
        if (!String.IsNullOrEmpty(text))
        {
            Builder.Append(text);
        }
        return this;
    }

    /// <inheritdoc cref="Append(string)"/>
    public SrcBuilder Append(ReadOnlySpan<char> text)
    {
        if (!text.IsEmpty)
        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            Builder.Append(text);
#else
#pragma warning disable CS1503,CA1830
            Builder.Append(text.ToString());
#pragma warning restore CS1503,CA1830
#endif

        }
        return this;
    }

    /// <inheritdoc cref="Append(string)"/>
    public SrcBuilder Append(char text)
    {
        Builder.Append(text);
        return this;
    }

    /// <summary>Appends the <see cref="IndentChars"/>, the specified text if any, and a linebreak sequence.</summary>
    /// <param name="text">The text to append.</param>
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

    /// <inheritdoc cref="AppendLine(string)"/>
    public SrcBuilder AppendLine(char text)
    {
        AppendIndent();
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    public SrcBuilder AppendLine()
    {
        AppendIndent();
        Builder.AppendLine();
        return this;
    }

    /// <summary>Appends the the specified text if any, and a linebreak sequence. Does not append any indentation contrary to <see cref="AppendLine(string?)"/></summary>
    /// <param name="text">The text to append.</param>
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

    /// <inheritdoc cref="NoIndentLine(string)"/>
    public SrcBuilder NoIndentLine(char text)
    {
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    /// <summary>Appends a linebreak sequence, the <see cref="IndentChars"/>, the specified text if any, and a linebreak sequence.</summary>
    /// <param name="text">The text to append.</param>
    /// <example>Used to separate method bodies inside type declarations</example>
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

    /// <inheritdoc cref="AppendDoubleLine(string)"/>
    public SrcBuilder AppendDoubleLine(char text)
    {
        Builder.AppendLine();
        AppendIndent();
        Builder.Append(text)
            .AppendLine();
        return this;
    }

    /// <inheritdoc cref="AppendDoubleLine(string)"/>
    public SrcBuilder AppendDoubleLine()
    {
        Builder.AppendLine();
        AppendIndent();
        Builder.AppendLine();
        return this;
    }

    /// <summary>Resets the <see cref="IndentLevel"/> and returns the string representing the written data.</summary>
    public override string ToString()
    {
        IndentLevel = 0;
        return Builder.ToString();
    }

    #endregion StringBuilder

    #region Source file

    /// <summary>Increments the <see cref="IndentLevel"/> by one</summary>
    public SrcBuilder Indent()
    {
        IndentLevel += 1;
        return this;
    }

    /// <summary>Decrements the <see cref="IndentLevel"/> by one, if possible; otherwise does nothing.</summary>
    public SrcBuilder Outdent()
    {
        var level = IndentLevel;
        if (level > 0)
        {
            IndentLevel = level - 1;
        }
        return this;
    }

    /// <summary>Appends a linebreak sequence.</summary>
    // ReSharper disable once InconsistentNaming
    public SrcBuilder NL()
    {
        Builder.AppendLine();
        return this;
    }

    /// <summary>Appends the indented statement followed by a linebreak. No tailing semicolon is added.</summary>
    /// <remarks>Same as <see cref="AppendLine(string?)"/>.</remarks>
    /// <param name="statement">The statement to add.</param>
    public SrcBuilder Stmt(string statement)
    {
        AppendLine(statement);
        return this;
    }

    /// <summary>Appends indented `{` and a linebreak. Increases the <see cref="IndentLevel"/>.</summary>
    public SrcBuilder BlockStart()
    {
        AppendLine('{');
        Indent();
        return this;
    }

    /// <summary>Decreases the <see cref="IndentLevel"/>. Appends indented `}` and two linebreaks.</summary>
    public SrcBuilder BlockEnd()
    {
        Outdent();
        AppendLine('}');
        NL();
        return this;
    }

    #endregion Source file

    #region Csharp

    /// <summary>Appends indented `if (<paramref name="condition"/>)` followed by <see cref="BlockStart"/>.</summary>
    /// <param name="condition">The if condition.</param>
    public SrcBuilder StartIfBlock(string condition)
    {
        AppendIndent();
        Builder.Append("if (").Append(condition).AppendLine(")");
        BlockStart();
        return this;
    }

    /// <summary>Appends indented `switch (<paramref name="value"/>)` followed by <see cref="BlockStart"/>.</summary>
    /// <param name="value">The value used for the switch expression.</param>
    public SrcBuilder StartSwitchBlock(string value)
    {
        AppendIndent();
        Builder.Append("switch (").Append(value).AppendLine(")");
        BlockStart();
        return this;
    }

    /// <summary>
    /// Appends an indented case block. Increments <see cref="IndentLevel"/>.
    /// <para>If <paramref name="constant"/> is null, adds the `default:` branch.</para>
    /// <para>If <paramref name="constant"/> is not null, adds the `case <paramref name="constant"/>:` branch.</para>
    /// </summary>
    /// <param name="constant">The constant value used for the case branch, or default branch is null.</param>
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
        Indent();
        NL();
        return this;
    }

    /// <summary>Terminates a case branch `break;`. Decrements <see cref="IndentLevel"/>.</summary>
    public SrcBuilder CaseEnd()
    {
        AppendLine("break;");
        Outdent();
        return this;
    }

    /// <summary>Begins a `#region` preprocessor block.</summary>
    /// <param name="name">The name of the region.</param>
    public SrcBuilder StartRegion(string name)
    {
        AppendDoubleLine($"#region {name}");
        return this;
    }

    /// <summary>Terminates a `#endregion` preprocessor block.</summary>
    /// <param name="name">The name of the region.</param>
    public SrcBuilder EndRegion(string name)
    {
        AppendDoubleLine($"#endregion {name}");
        return this;
    }

    /// <summary>Creates a new block builder, which can be used fluently.</summary>
    public SrcBlock Block()
    {
        return new SrcBlock(this);
    }

    /// <summary>Same as <see cref="StartIfBlock"/>, but creates a new block builder, which can be used fluently.</summary>
    /// <param name="condition">The if condition.</param>
    public SrcBlock If(string? condition)
    {
        AppendIndent();
        Builder.Append("if (").Append(condition).AppendLine(")");
        return Block();
    }

    /// <summary>Appends the specified <paramref name="definition"/>, and creates a new block builder, which can be used fluently.</summary>
    /// <param name="definition">The declaration.</param>
    public SrcBlock Decl(string definition)
    {
        AppendLine(definition);
        return Block();
    }

    /// <summary>Parameter formatting callback.</summary>
    public delegate void ParamsAction(ref SrcColl p);
    /// <summary>Parameter formatting callback with a given context object.</summary>
    public delegate void ParamsAction<C>(in C ctx, ref SrcColl p);

    /// <summary>Appends the specified <paramref name="definition"/>, and creates a new block builder, which can be used fluently.</summary>
    /// <param name="definition">The definition for the call target.</param>
    /// <param name="parameters">The callback used to create the arguments for the call.</param>
    public SrcBlock Decl(string definition, ParamsAction parameters)
    {
        AppendIndent(definition);
        var p = Params();
        parameters(ref p);
        p.Dispose();
        return Block();
    }

    /// <summary>Appends the specified <paramref name="definition"/>, and creates a new block builder, which can be used fluently.</summary>
    /// <param name="definition">The definition for the call target.</param>
    /// <param name="ctx">The context object </param>
    /// <param name="parameters">The callback used to create the arguments for the call.</param>
    public SrcBlock Decl<C>(string definition, in C ctx, ParamsAction<C> parameters)
    {
        AppendIndent(definition);
        var p = Params();
        parameters(in ctx, ref p);
        p.Dispose();
        NL();
        return Block();
    }

    /// <summary>Creates a new indentation builder, which can be used fluently.</summary>
    public SrcIndent Indented()
    {
        return new SrcIndent(this);
    }

    /// <summary>Same as <see cref="StartSwitchBlock"/>, but returns a builder which can be used fluently to create the branches.</summary>
    /// <param name="value">The constant value used for the switch statement.</param>
    public SrcBlock Switch(string value)
    {
        AppendIndent();
        Builder.Append("switch (")
            .Append(value)
            .AppendLine(")");
        return Block();
    }

    /// <summary>Creates a switch statement at once using callback functions to specify features.</summary>
    /// <param name="value">The constant value used for the switch statement.</param>
    /// <param name="source">The sequence of branches to add to the case statement.</param>
    /// <param name="caseConstant">Determines the constant value for the case expression `case <paramref name="caseConstant"/>:`.</param>
    /// <param name="caseBlock">Determines the content of the block following the case constant. Returns the whether to add a `break;` statement, or not.</param>
    /// <param name="defaultBlock">Determines the content of the default block.</param>
    /// <typeparam name="S">The type of the sequence used as a source for the case branches.</typeparam>
    /// <typeparam name="T">The type of elements the the source sequence.</typeparam>
    public void Switch<S, T>(string value, in S source, Func<T, string?> caseConstant, Func<SrcBuilder, T, CaseStyle> caseBlock, Func<SrcBuilder, S, CaseStyle>? defaultBlock = null)
        where S : IEnumerable<T>
    {
        using (Switch(value))
        {
            foreach (var b in source)
            {
                string? constant = caseConstant(b);
                if (constant is not null)
                {
                    using var c = Case(constant);
                    var returned = caseBlock(this, b);
                    if ((returned & CaseStyle.NoBreak) != 0)
                    {
                        c.Returned();
                    }
                }
            }
            if (defaultBlock is not null)
            {
                using var c = Case(default);
                var returned = defaultBlock(this, source);
                if ((returned & CaseStyle.NoBreak) != 0)
                {
                    c.Returned();
                }
            }
        }
    }

    /// <summary>Creates a switch statement at once using callback functions to specify features.</summary>
    /// <param name="value">The constant value used for the switch statement.</param>
    /// <param name="source">The sequence of branches to add to the case statement.</param>
    /// <param name="caseConstant">Determines the constant value for the case expression `case <paramref name="caseConstant"/>:`.</param>
    /// <param name="caseBlock">Determines the content of the block following the case constant. Adds `break;` after each block.</param>
    /// <param name="defaultBlock">Determines the content of the default block.</param>
    /// <typeparam name="S">The type of the sequence used as a source for the case branches.</typeparam>
    /// <typeparam name="T">The type of elements the the source sequence.</typeparam>
    public void Switch<S, T>(string value, in S source, Func<T, string?> caseConstant, Action<SrcBuilder, T> caseBlock, Action<SrcBuilder, S>? defaultBlock = null)
        where S : IEnumerable<T>
    {
        using (Switch(value))
        {
            foreach (var b in source)
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
                    defaultBlock(this, source);
                }
            }
        }
    }

    /// <summary>Same as <see cref="CaseStart"/>, but returns a builder which can be used to write the case block fluently.</summary>
    /// <param name="constant">The constant expression used for the `case <paramref name="constant"/>:`.</param>
    public SrcCase Case(string? constant)
    {
        return new SrcCase(this, constant);
    }

    /// <summary>Adds the collection definition, then a collection initializer. Returns a fluent builder used to define collection elements.</summary>
    /// <param name="collection">The collection definition</param>
    /// <example>collection {<br/>  one,<br/>  two,<br/>  three<br/>};<br/></example>
    public SrcColl Coll(string collection)
    {
        AppendIndent(collection);
        return new SrcColl(this,
            static (b) => b.NL().AppendLine('{').Indent(),
            static (c, s) => c.Builder.AppendIndent().Append(s),
            static (b) => b.Append(',').NL(),
            static (b) => b.Append(',').NL().Outdent().AppendLine("};"));
    }

    /// <summary>Returns a fluent builder which can be used to create function parameters or arguments.</summary>
    /// <example>(one, two, three)</example>
    public SrcColl Params()
    {
        return new SrcColl(this,
            static (b) => b.Append('('),
            static (c, s) => c.Builder.Append(s),
            static (b) => b.Append(", "),
            static (b) => b.Append(")"));
    }

    /// <summary>Adds an invocation, then returns a fluent builder which can be used to create function parameters or arguments.</summary>
    /// <param name="invocation">The invocation to add</param>
    public SrcColl Call(string invocation)
    {
        AppendIndent(invocation);
        return new SrcColl(this,
            static (b) => b.Append('('),
            static (c, s) => c.Builder.Append(s),
            static (b) => b.Append(", "),
            static (b) => b.Append(");").NL());
    }

    /// <summary>Adds a region block. Returns a fluent builder which can be used to write into that region.</summary>
    /// <param name="name">The name of the region</param>
    public SrcPre Region(string name)
    {
        return new SrcPre(this, $"#region {name}", $"#endregion {name}");
    }

    /// <summary>Adds a `nullable enable`, `nullable restore` block. Returns a fluent builder which can be used to write into that region.</summary>
    public SrcPre NullableEnable()
    {
        return new SrcPre(this, "#nullable enable", "#nullable restore");
    }

    /// <summary>Disposable handle starting and terminating a block.</summary>
    public struct SrcBlock : IDisposable
    {
        private bool _disposed;
        /// <summary>The builder used to write inside this handle.</summary>
        public SrcBuilder Builder { get; }

        /// <summary>Initializes a new handle.</summary>
        /// <param name="builder">The builder used inside the handle.</param>
        public SrcBlock(SrcBuilder builder)
        {
            _disposed = false;
            Builder = builder;
            Builder.BlockStart();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.BlockEnd();
            }
            _disposed = true;
        }
    }

    /// <summary>Disposable handle starting and terminating a indented section.</summary>
    public struct SrcIndent : IDisposable
    {
        private bool _disposed;
        /// <inheritdoc cref="SrcBlock.Builder"/>
        public SrcBuilder Builder { get; }

        /// <summary>Initializes a new handle.</summary>
        /// <param name="builder">The builder used inside the handle.</param>
        public SrcIndent(SrcBuilder builder)
        {
            _disposed = false;
            Builder = builder;
            Builder.Indent();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.Outdent();
            }
            _disposed = true;
        }
    }

    /// <summary>Disposable handle starting and terminating a case block.</summary>
    public struct SrcCase : IDisposable
    {
        private bool _disposed;
        private bool _returned;
        /// <inheritdoc cref="SrcBlock.Builder"/>
        public SrcBuilder Builder { get; }

        /// <summary>Initializes a new handle</summary>
        /// <param name="builder">The builder used inside the handle.</param>
        /// <param name="constant">The case constant; if null is a default case branch.</param>
        public SrcCase(SrcBuilder builder, string? constant = null)
        {
            _disposed = false;
            _returned = false;
            Builder = builder;
            Builder.CaseStart(constant);
        }

        /// <summary>Marks the case as already having returned. No `break;` statement will be added.</summary>
        public void Returned()
        {
            _returned = true;
        }

        /// <inheritdoc />
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

    /// <summary>Disposable handle used to format collections, parameters and sequences.</summary>
    public struct SrcColl : IDisposable, IEnumerable<string>
    {
        private bool _disposed;
        /// <inheritdoc cref="SrcBlock.Builder"/>
        public SrcBuilder Builder { get; }
        /// <summary>Specifies that the current element is the first element in the sequence.</summary>
        public bool IsFirstElement { get; set; }
        /// <summary>The action called before the first element is built.</summary>
        public Action<SrcBuilder> Prefix { get; set; }
        /// <summary>The action called when building a single element.</summary>
        public Action<SrcColl, string> Infix { get; }
        /// <summary>The separator called between building two elements.</summary>
        public Action<SrcBuilder> Separator { get; }
        /// <summary>The action called after the last element is built.</summary>
        public Action<SrcBuilder> Suffix { get; }

        /// <summary>Initializes a new handle.</summary>
        /// <param name="builder">The builder used inside the handle</param>
        /// <param name="prefix">The action called before the first element is built.</param>
        /// <param name="infix">The action called when building a single element.</param>
        /// <param name="separator">The separator called between building two elements.</param>
        /// <param name="suffix">The action called after the last element is built.</param>
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

        /// <summary>Adds a element to the sequence.</summary>
        /// <param name="parameter">The parameter passed to <see cref="Separator"/>.</param>
        public void Add(string parameter)
        {
            if (!IsFirstElement)
            {
                Separator(Builder);
            }

            IsFirstElement = false;
            Infix(this, parameter);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                Suffix(Builder);
            }
            _disposed = true;
        }

        /// <inheritdoc />
        [Obsolete("Used to allow for a collection initializer. Not implemented!", true)]
        public IEnumerator<string> GetEnumerator()
        {
            yield break; // Dummy implementation
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break; // Dummy implementation
        }
    }

    /// <summary>Disposable handle used to represent a block with a prefix and a suffix.</summary>
    public struct SrcPre : IDisposable
    {
        private bool _disposed;
        /// <inheritdoc cref="SrcBlock.Builder"/>
        public SrcBuilder Builder { get; }
        /// <summary>Represents the prefix.</summary>
        public string Start { get; }
        /// <summary>Represents the suffix.</summary>
        public string End { get; }

        /// <summary>Initializes a new handle.</summary>
        /// <param name="builder">The builder used inside the handle.</param>
        /// <param name="start">The prefix.</param>
        /// <param name="end">The suffix.</param>
        public SrcPre(SrcBuilder builder, string start, string end)
        {
            _disposed = false;
            Start = start;
            End = end;
            Builder = builder;
            Builder.AppendDoubleLine(Start);
        }


        /// <inheritdoc />
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

    /// <inheritdoc cref="AppendDoc(string)"/>
    public SrcBuilder AppendDoc()
    {
        return AppendIndent("/// ");
    }

    /// <inheritdoc cref="AppendDoc(string)"/>
    public SrcBuilder AppendDoc(char text)
    {
        return AppendIndent("/// ").Append(text);
    }

    /// <summary>Appends an indented documentation comment line `///`</summary>
    /// <param name="text">The comment line content</param>
    public SrcBuilder AppendDoc(string? text)
    {
        if (String.IsNullOrEmpty(text))
        {
            return AppendDoc();
        }

        return AppendIndent("/// ").Append(text);
    }

    /// <summary>Appends a starting tag for a XML-documentation element with optional attributes.</summary>
    /// <param name="elementName">The name of the XML-tag.</param>
    /// <param name="attributes">The string representation of the attributes.</param>
    public SrcBuilder DocStart(string elementName, string? attributes)
    {
        if (String.IsNullOrEmpty(attributes))
        {
            return AppendDoc($"<{elementName}>");
        }

        return AppendDoc($"<{elementName} {attributes}>");
    }

    /// <summary>Appends a terminating tag for a XML-documentation element.</summary>
    /// <param name="elementName">The name of the XML-tag.</param>
    public SrcBuilder DocEnd(string elementName)
    {
        return Append($"<{elementName}/>");
    }

    /// <summary>Same as <see cref="DocStart"/> and <see cref="DocEnd"/>, but with content and in a single line.</summary>
    /// <param name="elementName">The XML-tag name.</param>
    /// <param name="attributes">The XML-attributes.</param>
    /// <param name="content">The content.</param>
    /// <returns></returns>
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

    /// <summary>Returns a builder for the XML-documentation element. Allowing for fluent creation of content.</summary>
    /// <param name="elementName">The XML-tag name.</param>
    /// <param name="attributes">The XML-attributes.</param>
    /// <returns></returns>
    public SrcDoc Doc(string elementName, string? attributes = null)
    {
        return new SrcDoc(this, elementName, attributes);
    }

    /// <summary>Disposable handle for a XML-documentation element block.</summary>
    public struct SrcDoc : IDisposable
    {
        private bool _disposed;

        /// <inheritdoc cref="SrcBlock.Builder"/>
        public SrcBuilder Builder { get; }
        /// <summary>The XML-tag name.</summary>
        public string ElementName { get; }
        /// <summary>The XML-attributes.</summary>
        public string? Attributes { get; }

        /// <summary>Initializes a new handle.</summary>
        /// <param name="builder">The builder used inside the handle.</param>
        /// <param name="elementName">The XML-tag name.</param>
        /// <param name="attributes">The XML-attributes.</param>
        public SrcDoc(SrcBuilder builder, string elementName, string? attributes)
        {
            _disposed = false;
            Builder = builder;
            ElementName = elementName;
            Attributes = attributes;

            Builder.DocStart(ElementName, Attributes);
        }

        /// <inheritdoc />
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
