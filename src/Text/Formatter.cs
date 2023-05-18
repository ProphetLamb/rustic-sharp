using System;
using System.Collections.Generic;
using System.Globalization;

using Rustic.Memory;

namespace Rustic.Text;

/// <summary>Allows dynamic string formatting according the templates.</summary>
public sealed class Fmt {
    [ThreadStatic]
    private static Fmt? t_fmtInstance;

    /// <summary>The global instance containing definitions.</summary>
    public static Fmt Def => t_fmtInstance ??= new();

    /// <summary>Formats a string using the specified definition.</summary>
    /// <param name="format">The format string.</param>
    /// <param name="definition">The format definition.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <typeparam name="D">The type of the definition.</typeparam>
    /// <returns>The formatted string.</returns>
    public static string Format<D>(ReadOnlySpan<char> format, scoped ref D definition,
        IEqualityComparer<char>? comparer = null)
        where D : IFmtDef {
        scoped StrBuilder builder = new(stackalloc char[2048.Min(format.Length + (48 * definition.Count))]);
        scoped Tokenizer<char> tokenizer = new(format, comparer);

        while (true) {
            if (!definition.NextTextEnd(ref tokenizer, ref builder)) {
                break;
            }

            tokenizer.FinalizeToken();
            int holeStart = builder.Length;
            if (!definition.NextTextStart(ref tokenizer, ref builder)) {
                ThrowHelper.ThrowFormatException(tokenizer.Position, tokenizer.CursorPosition, "The hole is never closed.");
                break;
            }

            tokenizer.FinalizeToken();
            // retrieve hole, and reset builder to hole start
            ReadOnlySpan<char> hole = builder.AsSpan(holeStart);
            builder.Length = holeStart;

            if (definition.TryGetValue(hole, out ReadOnlySpan<char> value)) {
                value.CopyTo(builder.AppendSpan(value.Length));
            } else {
                ThrowHelper.ThrowFormatException(tokenizer.Position - hole.Length, tokenizer.Position, $"The hole `{hole.ToString()}` in the format does not have a corresponding definition provided.");
                break;
            }
        }

        definition.FinalTextEnd(ref tokenizer, ref builder);
        if (tokenizer.Width != 0 || !tokenizer.IsCursorEnd) {
            ThrowHelper.ThrowFormatException(tokenizer.Position, tokenizer.CursorPosition, "The format string was not fully processed.");
        }

        return builder.ToString();
    }


    /// <summary>Formats a string using index based definitions.</summary>
    /// <param name="format">The format string</param>
    /// <param name="arguments">The format arguments.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <param name="provider">The localizing formatter used.</param>
    /// <returns>The formatted string.</returns>
    public string Index(ReadOnlySpan<char> format, TinyRoVec<object?> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null) {
        IdxDef<object?> def = new(arguments, provider);
        return Format(format, ref def, comparer);
    }

    /// <inheritdoc cref="Index"/>
    public string Index<T>(ReadOnlySpan<char> format, TinyRoVec<T> arguments, IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null) {
        IdxDef<T> def = new(arguments, provider);
        return Format(format, ref def, comparer);
    }

    /// <summary>Formats a string using name based definitions.</summary>
    /// <param name="format">The format string</param>
    /// <param name="arguments">The format arguments.</param>
    /// <param name="comparer">The comparer determining whether two chars are equal.</param>
    /// <param name="provider">The localizing formatter used.</param>
    /// <returns>The formatted string.</returns>
    public string Named(ReadOnlySpan<char> format, IReadOnlyDictionary<string, object?> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null) {
        NamedDef<object?> def = new(arguments, provider);
        return Format(format, ref def, comparer);
    }

    /// <inheritdoc cref="Named"/>
    public string Named<T>(ReadOnlySpan<char> format, IReadOnlyDictionary<string, T> arguments,
        IEqualityComparer<char>? comparer = null, IFormatProvider? provider = null) {
        NamedDef<T> def = new(arguments, provider);
        return Format(format, ref def, comparer);
    }
}

/// <summary>Interface used to identify a format defintion.</summary>
public interface IFmtDef {
    /// <summary>The format provider used for formatting.</summary>
    IFormatProvider? Format { get; }

    /// <summary>The number of formatting arguments.</summary>
    int Count { get; }

    /// <summary>Moves the tokenizer to the char at which this text portion ended. Consumes the hole indicating characters. Adds the relevant text to the builder.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <param name="builder">The builder used to collect text.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder);
    /// <summary>Moves the tokenizer to the char at which the next text portion begins. Consumes the hole terminating characters. Adds the relevant hole definition to the builder.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <param name="builder">The builder used to collect the hole definition</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    bool NextTextStart(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder);

    /// <summary>Moves the tokenizer to the end of the format string. Ensures that the tokenizer cursor is at the end and empty.</summary>
    /// <param name="tokenizer">The tokenizer used.</param>
    /// <param name="builder">The builder used to collect text.</param>
    /// <exception cref="FormatException">The format string is invalid or not fully processed.</exception>
    void FinalTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder);

    /// <summary>Attempts to resolve the <paramref name="key"/> using the formatting arguments.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value representing the key.</param>
    /// <returns><c>true</c> if the <paramref name="key"/> is found; otherwise, <c>false</c>.</returns>
    bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value);
}

/// <summary>Format definition for numeric index based formatting</summary>
/// <typeparam name="T">The type of the formatting arguments.</typeparam>
public struct IdxDef<T> : IFmtDef {
    private int _escapedHoleLevel = 0;

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public IdxDef(TinyRoVec<T> arguments, IFormatProvider? format = null)
        : this(String.Empty, arguments, format) {
    }

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="prefix">The prefix required before curly bracket open to identify a hole.</param>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public IdxDef(string prefix, TinyRoVec<T> arguments, IFormatProvider? format = null) {
        Prefix = prefix;
        Arguments = arguments;
        Format = format;
    }

    /// <summary>The prefix required before curly bracket open to identify a hole.</summary>
    public string Prefix { get; }

    /// <summary>The formatting arguments used to fill holes in the format.</summary>
    public TinyRoVec<T> Arguments { get; }

    /// <summary>The number of formatting arguments.</summary>
    public int Count => Arguments.Count;

    /// <summary>The formatter providing localization.</summary>
    public IFormatProvider? Format { get; }

    /// <summary>Consumed chars until a non escaped curly bracket is found. Does not append the latest <see cref="Tokenizer{T}.Token"/> to the builder.</summary>
    /// <returns><c>true</c> if a curly bracket was found; otherwise <c>false</c>.</returns>
    private unsafe bool NextBracket(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder, delegate*<char, bool> predicate) {
        while (tokenizer.TryReadUntilAny(('{', '}'))) {
            var cursor = tokenizer.GetAtCursor(-1);

            if (cursor == '{' && tokenizer.TryReadNext('{')) {
                builder.Append(tokenizer.FinalizeToken().SliceEnd(1));
                _escapedHoleLevel += 1;
                continue;
            }
            if (cursor == '}' && tokenizer.TryReadNext('}')) {
                builder.Append(tokenizer.FinalizeToken().SliceEnd(1));
                _escapedHoleLevel -= 1;
                continue;
            }

            if (predicate(cursor)) {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public unsafe bool NextTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool determineHoleEnd(char c) => c == '{';

        while (true) {
            if (!NextBracket(ref tokenizer, ref builder, &determineHoleEnd)) {
                return false; // No more holes
            }

            ReversibleIndexedSpan<char> token = tokenizer.FinalizeToken();

            if (Prefix.IsEmpty()) {
                builder.Append(token.SliceEnd(1));
                return true; // Hole found
            }

            ReadOnlySpan<char> possiblePrefix = token.Slice(token.Length - Prefix.Length).ToSpan();
            if (possiblePrefix.SequenceEqual(Prefix.AsSpan())) {
                builder.Append(token.SliceEnd(1));
                return true; // Hole found
            }

            // Hole found, but not with the correct prefix; skip
            builder.Append(token);
        }
    }

    /// <inheritdoc />
    public unsafe bool NextTextStart(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool determineHoleEnd(char c) => c == '}';

        if (!NextBracket(ref tokenizer, ref builder, &determineHoleEnd)) {
            return false; // Hole not terminated
        }

        ReversibleIndexedSpan<char> token = tokenizer.FinalizeToken().SliceEnd(1);
        builder.Append(token);
        return true;
    }

    /// <inheritdoc />
    public unsafe void FinalTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool noop(char _) => true;
        NextBracket(ref tokenizer, ref builder, &noop);

        if (_escapedHoleLevel != 0) {
            ThrowHelper.ThrowFormatException(tokenizer.Position, tokenizer.CursorPosition, "Escaped brackets are not balanced. The number of opening brackets `{{` must match the number of closing brackets `}}`.");
        }

        // Append the remaining formatted string
        tokenizer.CursorPosition = tokenizer.Length;
        builder.Append(tokenizer.FinalizeToken());
    }

    /// <inheritdoc />
    public readonly bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value) {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (!Int32.TryParse(key, NumberStyles.Integer, Format, out var idx))
#else
        if (!Int32.TryParse(key.ToString(), NumberStyles.Integer, Format, out var idx))
#endif
        {
            value = ReadOnlySpan<char>.Empty;
            return false;
        }

        if (idx < 0 || idx >= Arguments.Count) {
            value = ReadOnlySpan<char>.Empty;
            return false;
        }

        var arg = Arguments[idx];
        string? s;
        if (Format?.GetFormat(arg?.GetType()) is ICustomFormatter f) {
            s = f.Format("{0}", arg, Format);
        } else {
            s = arg?.ToString();
        }

        value = s.AsSpan();
        return true;
    }
}

/// <summary>Format definition for named text based formatting</summary>
/// <typeparam name="T">The type of the formatting arguments.</typeparam>
public struct NamedDef<T> : IFmtDef {
    private int _escapedHoleLevel = 0;

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public NamedDef(IReadOnlyDictionary<string, T> arguments, IFormatProvider? format = null)
        : this(String.Empty, arguments, format) {
    }

    /// <summary>Initializes a new instance of <see cref="IdxDef{T}"/>.</summary>
    /// <param name="prefix">The prefix required before curly bracket open to identify a hole.</param>
    /// <param name="arguments">The formatting arguments used to fill holes in the format.</param>
    /// <param name="format">The formatter providing localization.</param>
    public NamedDef(string prefix, IReadOnlyDictionary<string, T> arguments, IFormatProvider? format = null) {
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

    /// <summary>Consumed chars until a non escaped curly bracket is found. Does not append the latest <see cref="Tokenizer{T}.Token"/> to the builder.</summary>
    /// <returns><c>true</c> if a curly bracket was found; otherwise <c>false</c>.</returns>
    private unsafe bool NextBracket(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder, delegate*<char, bool> predicate) {
        while (tokenizer.TryReadUntilAny(('{', '}'))) {
            var cursor = tokenizer.GetAtCursor(-1);

            if (cursor == '{' && tokenizer.TryReadNext('{')) {
                builder.Append(tokenizer.FinalizeToken().SliceEnd(1));
                _escapedHoleLevel += 1;
                continue;
            }
            if (cursor == '}' && tokenizer.TryReadNext('}')) {
                builder.Append(tokenizer.FinalizeToken().SliceEnd(1));
                _escapedHoleLevel -= 1;
                continue;
            }

            if (predicate(cursor)) {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public unsafe bool NextTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool determineHoleEnd(char c) => c == '{';

        while (true) {
            if (!NextBracket(ref tokenizer, ref builder, &determineHoleEnd)) {
                return false; // No more holes
            }

            ReversibleIndexedSpan<char> token = tokenizer.FinalizeToken();

            if (Prefix.IsEmpty()) {
                builder.Append(token.SliceEnd(1));
                return true; // Hole found
            }

            ReadOnlySpan<char> possiblePrefix = token.Slice(token.Length - Prefix.Length).ToSpan();
            if (possiblePrefix.SequenceEqual(Prefix.AsSpan())) {
                builder.Append(token.SliceEnd(1));
                return true; // Hole found
            }

            // Hole found, but not with the correct prefix; skip
            builder.Append(token);
        }
    }

    /// <inheritdoc />
    public unsafe bool NextTextStart(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool determineHoleEnd(char c) => c == '}';

        if (!NextBracket(ref tokenizer, ref builder, &determineHoleEnd)) {
            return false; // Hole not terminated
        }

        ReversibleIndexedSpan<char> token = tokenizer.FinalizeToken().SliceEnd(1);
        builder.Append(token);
        return true;
    }

    /// <inheritdoc />
    public unsafe void FinalTextEnd(scoped ref Tokenizer<char> tokenizer, scoped ref StrBuilder builder) {
        static bool noop(char _) => true;
        NextBracket(ref tokenizer, ref builder, &noop);

        if (_escapedHoleLevel != 0) {
            ThrowHelper.ThrowFormatException(tokenizer.Position, tokenizer.CursorPosition, "Escaped brackets are not balanced. The number of opening brackets `{{` must match the number of closing brackets `}}`.");
        }

        // Append the remaining formatted string
        tokenizer.CursorPosition = tokenizer.Length;
        builder.Append(tokenizer.FinalizeToken());
    }

    /// <inheritdoc />
    public readonly bool TryGetValue(in ReadOnlySpan<char> key, out ReadOnlySpan<char> value) {
        if (!Arguments.TryGetValue(key.ToString(), out T? arg)) {
            value = default;
            return false;
        }
        string? s;
        if (Format?.GetFormat(arg?.GetType()) is ICustomFormatter f) {
            s = f.Format("{0}", arg, Format);
        } else {
            s = arg?.ToString();
        }

        value = s.AsSpan();
        return true;
    }
}
