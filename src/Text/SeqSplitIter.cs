using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory;

namespace Rustic.Text;

/// <summary>Collection of extensions and utility functionality related to <see cref="SplitIter{T}"/>.</summary>
public static class SeqSplitIterExtensions {
    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array.</summary>
    /// <remarks>Does consume from the iterator.</remarks>
    /// <typeparam name="S">The type of a sequence of chars.</typeparam>
    [Pure]
    public static string[] ToArray<S>(this SeqSplitIter<char, S> self)
        where S : IEnumerable<char> {
        using PoolBufWriter<string> buf = new();
        while (self.MoveNext()) {
            buf.Add(self.Current.ToString());
        }
        return buf.ToArray();
    }

    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array, aggregating the slice of elements.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="O">The type of the aggregation of elements.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <param name="self">The iterator to aggregate.</param>
    /// <param name="aggregate">The function used to aggregate the items in each iterator element.</param>
    public static O[] ToArray<T, O, S>(this SeqSplitIter<T, S> self, Func<O, T, O> aggregate)
        where O : new()
        where S : IEnumerable<T> {
        using PoolBufWriter<O> buf = new();
        while (self.MoveNext()) {
            ReadOnlySpan<T> seg = self.Current;
            O cur = new();
            foreach (var itm in seg) {
                cur = aggregate(cur, itm);
            }
        }
        return buf.ToArray();
    }

    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array, aggregating the slice of elements.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="O">The type of the aggregation of elements.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <param name="self">The iterator to aggregate.</param>
    /// <param name="aggregate">The function used to aggregate the items in each iterator element.</param>
    /// <param name="seed">The initial value used for aggregating each element.</param>
    public static O[] ToArray<T, O, S>(this SeqSplitIter<T, S> self, Func<O, T, O> aggregate, Func<O> seed)
        where S : IEnumerable<T> {
        using PoolBufWriter<O> buf = new();
        while (self.MoveNext()) {
            ReadOnlySpan<T> seg = self.Current;
            var cur = seed();
            foreach (var itm in seg) {
                cur = aggregate(cur, itm);
            }
        }
        return buf.ToArray();
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this ReadOnlySpan<T> span, in S separator, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this ReadOnlySpan<T> span, in S separator0, in S separator1, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator0, separator1), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator2">The third separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this ReadOnlySpan<T> span, in S separator0, in S separator1, in S separator2, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator0, separator1, separator2), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator2">The third separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator3">The third separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this ReadOnlySpan<T> span, in S separator0, in S separator1, in S separator2, in S separator3, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator0, separator1, separator2, separator3), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this ReadOnlySpan<T> span, TinyRoSpan<S> separators, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, separators, options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this Span<T> span, in S separator, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator), options, comparer);
    }


    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fisS separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this Span<T> span, in S separator0, in S separator1, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, TinyRoSpan.From(separator0, separator1), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <typeparam name="S">The type of a sequence of elements.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SeqSplitIter<T, S> Split<T, S>(this Span<T> span, TinyRoSpan<S> separators, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
        where S : IEnumerable<T> {
        return new(span, separators, options, comparer);
    }
}

/// <summary>Iterates a span in segments determined by separator sequences.</summary>
/// <typeparam name="T">The type of an element of the span.</typeparam>
/// <typeparam name="S">The type of a sequence of elements.</typeparam>
public ref struct SeqSplitIter<T, S>
    where S : IEnumerable<T> {
    private TinyRoSpan<S> _separators;
    private Tokenizer<T> _tokenizer;
    private SplitOptions _options;
    private int _sepLen;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SeqSplitIter(ReadOnlySpan<T> input, TinyRoSpan<S> separators, SplitOptions options, IEqualityComparer<T>? comparer) {
        _separators = separators;
        _tokenizer = new Tokenizer<T>(input, comparer);
        _options = options;
        _sepLen = 0;
    }

    /// <summary>The segment of the current state of the enumerator.</summary>
    public ReadOnlySpan<T> Current {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tokenizer.Raw.Slice(Position, Width);
    }

    /// <summary>Represents the zero-based start-index of the current segment inside the source span.</summary>
    public int Position {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tokenizer.Position;
    }

    /// <summary>Represents the length of the current segment.</summary>
    public int Width {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tokenizer.Width - SepOff();
    }

    /// <summary>Indicates whether the <see cref="Current"/> item is terminated by the separator.</summary>
    public bool IncludesSeparator {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_options & SplitOptions.IncludeSeparator) != 0 && _tokenizer.CursorPosition < _tokenizer.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SepOff() {
        return (_options & SplitOptions.IncludeSeparator) != 0 || _tokenizer.CursorPosition == _tokenizer.Length ? 0 : _sepLen;
    }

    /// <summary>Returns a new <see cref="SplitIter{T}"/> enumerator with the same input in the initial state. </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeqSplitIter<T, S> GetEnumerator() {
        return new SeqSplitIter<T, S>(_tokenizer.Raw, _separators, _options, _tokenizer.Comparer);
    }

    /// <summary>Attempts to move the enumerator to the next segment.</summary>
    /// <returns><see langword="true"/> if the enumerator successfully moved to the next segment; otherwise, <see langword="false"/>.</returns>
    public bool MoveNext() {
        if (_tokenizer.CursorPosition == _tokenizer.Length) {
            return false;
        }

        do {
            _tokenizer.FinalizeToken();
            var head = _tokenizer.Length;
            foreach (var sep in _separators) {
                if (_tokenizer.Peek(sep, out var h, out var l) && head > h) {
                    _sepLen = l;
                    head = h;
                }
            }

            _tokenizer.Width = head - _tokenizer.Position;
        } while ((_options & SplitOptions.RemoveEmptyEntries) != 0 && _tokenizer.Width <= SepOff());

        return true;
    }

    /// <summary>Resets the enumerator to the initial state.</summary>
    public void Reset() {
        _tokenizer.Reset();
        _sepLen = 0;
    }

    /// <summary>
    ///     Disposes the enumerator.
    /// </summary>
    public void Dispose() {
        _tokenizer.Dispose();
        this = default;
    }
}
