using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory;
using Rustic.Memory.IO;

namespace Rustic.Text;

/// <summary>Defines how the results of the <see cref="SplitIter{T}"/> are transformed.</summary>
[Flags]
public enum SplitOptions : byte
{
    /// <summary>Default behavior. No transformation.</summary>
    None = 0,

    /// <summary>Do not return zero-length segments. Instead return the next result, if any.</summary>
    RemoveEmptyEntries = 1 << 0,

    /// <summary>Include the separator at the end of the resulting segment, if not at the end.</summary>
    IncludeSeparator = 1 << 1,

    /// <summary>All options.</summary>
    /// <remarks><c>RemoveEmptyEntries | SkipLeadingSegment | SkipTailingSegment</c></remarks>
    All = 0xff,
}

/// <summary>Iterates a span in segments determined by separators.</summary>
/// <typeparam name="T">The type of an element of the span.</typeparam>
public ref struct SplitIter<T>
{
    private readonly ReadOnlySpan<T> _source;
    private readonly TinySpan<T> _separators;
    private readonly SplitOptions _options;
    private readonly IEqualityComparer<T>? _comparer;
    private int _index;
    private int _segmentLength;

    internal SplitIter(ReadOnlySpan<T> values, in TinySpan<T> separators, SplitOptions options, IEqualityComparer<T>? comparer)
    {
        _source = values;
        _separators = separators;
        _options = options;
        _comparer = comparer;
        _index = 0;
        _segmentLength = 0;
    }

    /// <summary>The segment of the current state of the enumerator.</summary>
    public ReadOnlySpan<T> Current
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _source.Slice(_index, _segmentLength);
    }

    /// <summary>Represents the zero-based start-index of the current segment inside the source span.</summary>
    public int SegmentIndex => _index;

    /// <summary>Represents the length of the current segment.</summary>
    public int SegmentLength => _segmentLength;

    /// <summary>Indicates wether the <see cref="Current"/> item is terminated by the separator.</summary>
    public bool IncludesSeparator => (_options & SplitOptions.IncludeSeparator) != 0 && _index + _segmentLength < _source.Length;

    /// <summary>Returns a new <see cref="SplitIter{T}"/> enumerator with the same input in the initial state. </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitIter<T> GetEnumerator()
    {
        return new SplitIter<T>(_source, _separators, _options, _comparer);
    }

    /// <summary>Attempts to move the enumerator to the next segment.</summary>
    /// <returns><see langword="true"/> if the enumerator successfully moved to the next segment; otherwise, <see langword="false"/>.</returns>
    public bool MoveNext()
    {
        // Moves index to after the previous separator & zeros length
        var position = SkipCurrent();
        if (position >= _source.Length)
        {
            return false;
        }

        var comparer = _comparer;
        if (comparer is null)
        {
            if (typeof(T).IsValueType)
            {
                return EnsureMoveNext(position, MoveNextValueType(position));
            }

            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
            // so cache in a local rather than get EqualityComparer per loop iteration.
            comparer = EqualityComparer<T>.Default;
        }
        return EnsureMoveNext(position, MoveNextComparer(position, comparer));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SkipCurrent()
    {
        var pos = _index + _segmentLength;

        if ((_options & SplitOptions.IncludeSeparator) == 0 && pos != 0)
        {
            pos += 1;
        }

        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveNextValueType(int pos)
    {
        var len = _source.Length;
        for (; pos < len; pos += 1)
        {
            for (var seg = 0; seg < _separators.Length; seg += 1)
            {
                if (EqualityComparer<T>.Default.Equals(_separators[seg], _source[pos]))
                {
                    return pos;
                }
            }
        }
        return len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveNextComparer(int pos, IEqualityComparer<T> comparer)
    {
        var len = _source.Length;
        for (; pos < len; pos += 1)
        {
            for (var seg = 0; seg < _separators.Length; seg += 1)
            {
                if (comparer.Equals(_separators[seg], _source[pos]))
                {
                    return pos;
                }
            }
        }
        return len;
    }

    private bool EnsureMoveNext(int start, int end)
    {
        var len = end - start;
        _index = start;
        _segmentLength = len;

        if ((_options & SplitOptions.IncludeSeparator) != 0 && end != _source.Length)
        {
            _segmentLength += 1;
        }

        if ((_options & SplitOptions.RemoveEmptyEntries) != 0 && len == 0)
        {
            return MoveNext();
        }
        return true;
    }

    /// <summary>Resets the enumerator to the initial state.</summary>
    public void Reset()
    {
        _index = 0;
        _segmentLength = 0;
    }

    /// <summary>
    ///     Disposes the enumerator.
    /// </summary>
    public void Dispose()
    {
        this = default;
    }
}

/// <summary>Collection of extensions and utility functionality related to <see cref="SplitIter{T}"/>.</summary>
public static class SplitIterExtensions
{
    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array.</summary>
    /// <remarks>Does consume from the iterator.</remarks>
    [Pure]
    public static string[] ToArray(this SplitIter<char> self)
    {
        using PoolBufWriter<string> buf = new();
        while (self.MoveNext())
        {
            buf.Add(self.Current.ToString());
        }
        return buf.ToArray();
    }

    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array, aggregating the slice of elements.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="O">The type of the aggregation of elements.</typeparam>
    /// <param name="self">The iterator to aggregate.</param>
    /// <param name="aggregate">The function used to aggregate the items in each iterator element.</param>
    public static O[] ToArray<T, O>(this SplitIter<T> self, Func<O, T, O> aggregate)
        where O : new()
    {
        using PoolBufWriter<O> buf = new();
        foreach (var seg in self)
        {
            O cur = new();
            foreach (var itm in seg)
            {
                cur = aggregate(cur, itm);
            }
        }
        return buf.ToArray();
    }

    /// <summary>Enumerates the remaining elements in the <see cref="SplitIter{T}"/> into a array, aggregating the slice of elements.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="O">The type of the aggregation of elements.</typeparam>
    /// <param name="self">The iterator to aggregate.</param>
    /// <param name="aggregate">The function used to aggregate the items in each iterator element.</param>
    /// <param name="seed">The initial value used for aggregating each element.</param>
    public static O[] ToArray<T, O>(this SplitIter<T> self, Func<O, T, O> aggregate, Func<O> seed)
    {
        using PoolBufWriter<O> buf = new();
        foreach (var seg in self)
        {
            var cur = seed();
            foreach (var itm in seg)
            {
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
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this ReadOnlySpan<T> span, in T separator, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator0, separator1), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator2">The third separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, in T separator2, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator0, separator1, separator2), options, comparer);
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
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, in T separator2, in T separator3, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator0, separator1, separator2, separator3), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this ReadOnlySpan<T> span, TinySpan<T> separators, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, separators, options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this Span<T> span, in T separator)
    {
        return new(span, TinySpan.From(separator), SplitOptions.None, null);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this Span<T> span, in T separator, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator), options, comparer);
    }


    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the separators.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
    /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this Span<T> span, in T separator0, in T separator1, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, TinySpan.From(separator0, separator1), options, comparer);
    }

    /// <summary>Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.</summary>
    /// <param name="span">The span.</param>
    /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
    /// <param name="options">The options defining how to return the segments.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
    /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
    public static SplitIter<T> Split<T>(this Span<T> span, TinySpan<T> separators, SplitOptions options = SplitOptions.None, IEqualityComparer<T>? comparer = default)
    {
        return new(span, separators, options, comparer);
    }
}
