using System;
using System.Collections.Generic;
using System.Globalization;

using Rustic.Memory;

namespace Rustic.Text;

/// <summary>Allows dynamic string formatting accoring the templates.</summary>
public class Fmt
{
    private static readonly Lazy<Fmt> s_fmtInstance = new(() => new Fmt());

    /// <summary>The global instance containing definitions.</summary>
    public static Fmt Definition => s_fmtInstance.Value;

    /// <summary>Formats a string using the specified definition.</summary>
    /// <param name="format">The format string.</param>
    /// <param name="definition">The format definition.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <typeparam name="D">The type of the definition.</typeparam>
    /// <returns>The formatted string.</returns>
    public static string Format<D>(ReadOnlySpan<char> format, in D definition,
        IEqualityComparer<char>? comparer = null)
        where D : IFmtDef
    {
        StrBuilder sb = new(stackalloc char[2048.Min(format.Length + (48 * definition.Count))]);
        FmtBuilder<D> fb = new(sb, format, definition, comparer);
        while (fb.Next()) { }
        return fb.ToString();
    }

    /// <summary>Formats a string using index based definitions.</summary>
    /// <param name="format">The format string</param>
    /// <param name="arguments">The format arguments.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <param name="provider">The localizing formatter used.</param>
    /// <returns>The formatted string.</returns>
    public string Index(ReadOnlySpan<char> format, TinyRoVec<object?> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new IdxDef<object?>(arguments, provider), comparer);
    }

    /// <inheritdoc cref="Index"/>
    public string Index<T>(ReadOnlySpan<char> format, TinyRoVec<T> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new IdxDef<T>(arguments, provider), comparer);
    }

    /// <summary>Formats a string using name based definitions.</summary>
    /// <param name="format">The format string</param>
    /// <param name="arguments">The format arguments.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <param name="provider">The localizing formatter used.</param>
    /// <returns>The formatted string.</returns>
    public string Named(ReadOnlySpan<char> format, IReadOnlyDictionary<string, object?> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new NamedDef<object?>(arguments, provider), comparer);
    }

    /// <inheritdoc cref="Named"/>
    public string Named<T>(ReadOnlySpan<char> format, IReadOnlyDictionary<string, T> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new NamedDef<T>(arguments, provider), comparer);
    }
}

/// <summary>Controls the formatting using a definition.</summary>
/// <typeparam name="D">The type of the definition.</typeparam>
public ref struct FmtBuilder<D>
    where D : IFmtDef
{
    private D _definition;
    private Tokenizer<char> _tokenizer;
    private StrBuilder _builder;

    /// <summary>Creates a new instance of <see cref="FmtBuilder{D}"/>.</summary>
    /// <param name="builder">The <see cref="StrBuilder"/> written to.</param>
    /// <param name="input">The format string.</param>
    /// <param name="definition">The format definition</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    public FmtBuilder(StrBuilder builder, ReadOnlySpan<char> input, in D definition, IEqualityComparer<char>? comparer)
    {
        _definition = definition;
        _tokenizer = new Tokenizer<char>(input, comparer);
        _builder = builder;
    }

    /// <summary>Attempts to format the next token.</summary>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public bool Next()
    {
        _tokenizer.FinalizeToken();
        if (!_definition.NextTextEnd(ref _tokenizer))
        {
            return false;
        }
        _builder.Append(_tokenizer.FinalizeToken());

        if (!_definition.NextHoleBegin(ref _tokenizer))
        {
            return false;
        }

        _tokenizer.FinalizeToken();
        if (!_definition.NextHoleEnd(ref _tokenizer))
        {
            return false;
        }

        if (_definition.TryGetValue(_tokenizer.FinalizeToken(), out var value)) {
            value.CopyTo(_builder.AppendSpan(value.Length));
        }

        return _definition.NextTextStart(ref _tokenizer);
    }

    /// <summary>Builds the string representation of the format. Disposes the builder.</summary>
    public override string ToString()
    {
        var s = _builder.ToString();
        Dispose();
        return s;
    }

    /// <summary>Releases all resources held by the builder, and rests the state.</summary>
    public void Dispose()
    {
        _tokenizer.Dispose();
        _builder.Dispose();
        this = default;
    }
}

/// <summary>Interface used to identify a format defintion.</summary>
public interface IFmtDef
{
    /// <summary>The format provider used for formatting.</summary>
    IFormatProvider? Format { get; }

    /// <summary>The number of formatting arguments.</summary>
    int Count { get; }

    /// <summary>Moves the tokenizer to the char at which this text portion ended.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextTextEnd(ref Tokenizer<char> tokenizer);
    /// <summary>Moves the tokenizer to the char at which this holes content begins.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextHoleBegin(ref Tokenizer<char> tokenizer);
    /// <summary>Moves the tokenizer to the char at which this hole content ends.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextHoleEnd(ref Tokenizer<char> tokenizer);
    /// <summary>Moves the tokenizer to the char at which the next text portion begins.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextTextStart(ref Tokenizer<char> tokenizer);

    /// <summary>Attempts to resolve the <paramref name="key"/> using the formatting arguments.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value representing the key.</param>
    /// <returns><c>true</c> if the <paramref name="key"/> is found; otherwise, <c>false</c>.</returns>
    bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value);
}

/// <summary>Format definition for numeric index based formatting</summary>
/// <typeparam name="T">The type of the formatting arguments.</typeparam>
public readonly struct IdxDef<T> : IFmtDef
{
    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public IdxDef(TinyRoVec<T> arguments, IFormatProvider? format = null)
        : this(String.Empty, arguments, format)
    {
    }

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="prefix">The prefix required before curly bracket open to identify a hole.</param>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public IdxDef(string prefix, TinyRoVec<T> arguments, IFormatProvider? format = null)
    {
        Prefix = prefix;
        Arguments = arguments;
        Format = format;
    }

    /// <summary>The prefix required before curly bracket open to identify a hole.</summary>
    public string Prefix { get; }

    /// <summary>The formatting arguments used to fill holes in the format.</summary>
    public TinyRoVec<object?> Arguments { get; }

    /// <summary>The number of formatting arguments.</summary>
    public int Count => Arguments.Count;

    /// <summary>The formatter providing localization.</summary>
    public IFormatProvider? Format { get; }

    /// <inheritdoc />
    public bool NextTextEnd(ref Tokenizer<char> tokenizer)
    {
        while ((Prefix.Length == 0 || tokenizer.Read(Prefix.AsSpan())) && tokenizer.ReadUntilAny('{') && tokenizer.TryReadNext('{')) { }
        tokenizer.Consume(-Prefix.Length - 1);
        return true;
    }

    /// <inheritdoc />
    public bool NextHoleBegin(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(Prefix.Length + 1);
    }

    /// <inheritdoc />
    public bool NextHoleEnd(ref Tokenizer<char> tokenizer)
    {
        while (tokenizer.ReadUntilAny('}') && tokenizer.TryReadNext('}')) { }

        tokenizer.Consume(-1);
        return false;
    }

    /// <inheritdoc />
    public bool NextTextStart(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(1);
    }

    /// <inheritdoc />
    public bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (!Int32.TryParse(key, NumberStyles.Integer, Format, out var idx))
#else
        if (!Int32.TryParse(key.ToString(), NumberStyles.Integer, Format, out var idx))
#endif
        {
            value = ReadOnlySpan<char>.Empty;
            return false;
        }

        if (idx < 0 || idx >= Arguments.Count)
        {
            value = ReadOnlySpan<char>.Empty;
            return false;
        }

        var arg = Arguments[idx];
        string? s;
        if (Format?.GetFormat(arg?.GetType()) is ICustomFormatter f)
        {
            s = f.Format("{0}", arg, Format);
        }
        else
        {
            s = arg?.ToString();
        }

        value = s.AsSpan();
        return true;
    }
}

/// <summary>Format definition for text-name based formatting</summary>
/// <typeparam name="T">The type of the formatting arguments.</typeparam>
public readonly struct NamedDef<T> : IFmtDef
{
    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public NamedDef(IReadOnlyDictionary<string, T> arguments, IFormatProvider? format)
        : this(String.Empty, arguments, format)
    {
    }

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="prefix">The prefix required before curly bracket open to identify a hole.</param>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public NamedDef(string prefix, IReadOnlyDictionary<string, T> arguments, IFormatProvider? format)
    {
        Prefix = prefix;
        Arguments = arguments;
        Format = format;
    }

    /// <summary>The prefix required before curly bracket open to identify a hole.</summary>
    public string Prefix { get; }

    /// <summary>The formatting arguments used to fill holes in the format.</summary>
    public IReadOnlyDictionary<string, T> Arguments { get; }

    /// <summary>The number of formatting arguments.</summary>
    public int Count => Arguments.Count;

    /// <summary>The formatter providing localization.</summary>
    public IFormatProvider? Format { get; }

    /// <inheritdoc />
    public bool NextTextEnd(ref Tokenizer<char> tokenizer)
    {
        while ((Prefix.Length == 0 || tokenizer.Read(Prefix.AsSpan())) && tokenizer.ReadAny('{') && tokenizer.TryReadNext('{')) { }

        tokenizer.Consume(-Prefix.Length - 1);
        return true;
    }

    /// <inheritdoc />
    public bool NextHoleBegin(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(Prefix.Length + 1);
    }

    /// <inheritdoc />
    public bool NextHoleEnd(ref Tokenizer<char> tokenizer)
    {
        while (tokenizer.ReadUntilAny('}') && tokenizer.TryReadNext('}')) { }

        tokenizer.Consume(-1);
        return false;
    }

    /// <inheritdoc />
    public bool NextTextStart(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(1);
    }

    /// <inheritdoc />
    public bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value)
    {
        if (!Arguments.TryGetValue(key.ToString(), out var arg))
        {
            value = ReadOnlySpan<char>.Empty;
            return false;
        }

        string? s;
        if (Format?.GetFormat(arg?.GetType()) is ICustomFormatter f)
        {
            s = f.Format("{0}", arg, Format);
        }
        else
        {
            s = arg?.ToString();
        }

        value = s.AsSpan();
        return true;
    }
}
