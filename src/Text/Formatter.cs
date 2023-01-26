using System;
using System.Collections.Generic;
using System.Globalization;

using Rustic.Memory;

namespace Rustic.Text;

public class Fmt
{
    private static readonly Lazy<Fmt> FmtInstance = new(() => new Fmt());

    /// <summary>The global instance containing definitions.</summary>
    public static Fmt Definition => FmtInstance.Value;

    public static string Format<D>(ReadOnlySpan<char> format, in D definition,
        IEqualityComparer<char>? comparer = null)
        where D : IFmtDef
    {
        StrBuilder sb = new(stackalloc char[2048.Min(format.Length + (48 * definition.Count))]);
        FmtBuilder<D> fb = new(sb, format, definition, comparer);
        while (fb.Next()) ;
        return fb.ToString();
    }

    public string Index(ReadOnlySpan<char> format, TinyRoVec<object?> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new IdxDef<object?>(arguments, provider), comparer);
    }

    public string Index<T>(ReadOnlySpan<char> format, TinyRoVec<T> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new IdxDef<T>(arguments, provider), comparer);
    }

    public string Named<T>(ReadOnlySpan<char> format, IReadOnlyDictionary<string, T> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new NamedDef<T>(arguments, provider), comparer);
    }

    public string Named(ReadOnlySpan<char> format, IReadOnlyDictionary<string, object?> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null)
    {
        return Format(format, new NamedDef<object?>(arguments, provider), comparer);
    }
}

public ref struct FmtBuilder<D>
    where D : IFmtDef
{
    private D _definition;
    private Tokenizer<char> _tokenizer;
    private StrBuilder _builder;

    public FmtBuilder(StrBuilder builder, ReadOnlySpan<char> input, in D definition, IEqualityComparer<char>? comparer)
    {
        _definition = definition;
        _tokenizer = new Tokenizer<char>(input, comparer);
        _builder = builder;
    }

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

        if (_definition.TryGetValue(_tokenizer.FinalizeToken(), out var value))
        {
            _builder.Append(value);
        }

        return _definition.NextTextStart(ref _tokenizer);
    }

    public override string ToString()
    {
        var s = _builder.ToString();
        Dispose();
        return s;
    }

    public void Dispose()
    {
        _tokenizer.Dispose();
        _builder.Dispose();
        this = default;
    }
}

public interface IFmtDef
{
    /// <summary>The format provider used for formatting.</summary>
    IFormatProvider? Format { get; }

    /// <summary>The number of formatting arguments.</summary>
    int Count { get; }

    bool NextTextEnd(ref Tokenizer<char> tokenizer);
    bool NextHoleBegin(ref Tokenizer<char> tokenizer);
    bool NextHoleEnd(ref Tokenizer<char> tokenizer);
    bool NextTextStart(ref Tokenizer<char> tokenizer);

    /// <summary>Attempts to resolve the <paramref name="key"/> using the formatting arguments.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value representing the key.</param>
    /// <returns><c>true</c> if the <paramref name="key"/> is found; otherwise, <c>false</c>.</returns>
    bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value);
}

public readonly struct IdxDef<T> : IFmtDef
{
    public IdxDef(TinyRoVec<T> arguments, IFormatProvider? format = null)
        : this(String.Empty, arguments, format)
    {
    }

    public IdxDef(string prefix, TinyRoVec<T> arguments, IFormatProvider? format = null)
    {
        Prefix = prefix;
        Arguments = arguments;
        Format = format;
    }

    public string Prefix { get; }

    public TinyRoVec<object?> Arguments { get; }

    public int Count => Arguments.Count;

    public IFormatProvider? Format { get; }

    public bool NextTextEnd(ref Tokenizer<char> tokenizer)
    {
        while ((Prefix.Length == 0 || tokenizer.Read(Prefix.AsSpan())) && tokenizer.ReadUntilAny('{') && tokenizer.TryReadNext('{')) ;
        tokenizer.Consume(-Prefix.Length - 1);
        return true;
    }

    public bool NextHoleBegin(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(Prefix.Length + 1);
    }

    public bool NextHoleEnd(ref Tokenizer<char> tokenizer)
    {
        while (tokenizer.ReadUntilAny('}') && tokenizer.TryReadNext('}')) ;

        tokenizer.Consume(-1);
        return false;
    }

    public bool NextTextStart(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(1);
    }

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

public readonly struct NamedDef<T> : IFmtDef
{
    public NamedDef(IReadOnlyDictionary<string, T> arguments, IFormatProvider? format)
        : this(String.Empty, arguments, format)
    {
    }

    public NamedDef(string prefix, IReadOnlyDictionary<string, T> arguments, IFormatProvider? format)
    {
        Prefix = prefix;
        Arguments = arguments;
        Format = format;
    }

    public string Prefix { get; }

    public IReadOnlyDictionary<string, T> Arguments { get; }

    public int Count => Arguments.Count;

    public IFormatProvider? Format { get; }

    public bool NextTextEnd(ref Tokenizer<char> tokenizer)
    {
        while ((Prefix.Length == 0 || tokenizer.Read(Prefix.AsSpan())) && tokenizer.ReadAny('{') && tokenizer.TryReadNext('{')) ;

        tokenizer.Consume(-Prefix.Length - 1);
        return true;
    }

    public bool NextHoleBegin(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(Prefix.Length + 1);
    }

    public bool NextHoleEnd(ref Tokenizer<char> tokenizer)
    {
        while (tokenizer.ReadUntilAny('}') && tokenizer.TryReadNext('}')) ;

        tokenizer.Consume(-1);
        return false;
    }

    public bool NextTextStart(ref Tokenizer<char> tokenizer)
    {
        return tokenizer.Consume(1);
    }

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
