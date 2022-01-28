using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

using Rustic.Common;
using Rustic.Memory.IO;

namespace Rustic.Memory;

/// <summary>Partially inlined immutable collection of function parameters.</summary>
public static class TinyVec
{
    /// <summary>Returns an empty <see cref="TinyVec{T}"/>.</summary>
    /// <typeparam name="T">The type of the span.</typeparam>
    /// <returns>An empty <see cref="TinyVec{T}"/>.</returns>
    public static TinyVec<T> Empty<T>() => default;

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    public static TinyVec<T> From<T>(in T arg0)
    {
        return new(1, arg0, default, default, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    public static TinyVec<T> From<T>(in T arg0, in T arg1)
    {
        return new(2, arg0, arg1, default, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    public static TinyVec<T> From<T>(in T arg0, in T arg1, in T arg2)
    {
        return new(3, arg0, arg1, arg2, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    /// <param name="arg3">The fourth value.</param>
    public static TinyVec<T> From<T>(in T arg0, in T arg1, in T arg2, in T arg3)
    {
        return new(4, arg0, arg1, arg2, arg3);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values array.</param>
    public static TinyVec<T> From<T>(in ArraySegment<T> values)
    {
        return new(values);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values array.</param>
    public static TinyVec<T> From<T>(T[] values)
    {
        return new(new ArraySegment<T>(values, 0, values.Length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    public static TinyVec<T> From<T>(T[] values, int start)
    {
        return new(new ArraySegment<T>(values, start, values.Length - start));
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    /// <param name="length">The number of values form the <paramref name="start"/>.</param>
    public static TinyVec<T> From<T>(T[] values, int start, int length)
    {
        return new(new ArraySegment<T>(values, start, length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Performs a shallow copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <param name="values">The sequence of values.</param>
    public static TinyVec<T> Copy<T, E>(E values)
        where E : IEnumerable<T>
    {
        if (values is T[] array)
        {
            // This should never occur.
            return From(array);
        }
        if (values is ArraySegment<T> segment)
        {
            return new(segment);
        }
        using var en = values.GetEnumerator();
        if (!en.MoveNext())
        {
            return default;
        }
        T arg0 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0);
        }
        T arg1 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0, arg1);
        }
        T arg2 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0, arg1, arg2);
        }
        T arg3 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0, arg1, arg2, arg3);
        }
        BufWriter<T> args = new(8)
        {
            arg0,
            arg1,
            arg2,
            arg3,
            en.Current
        };
        while (en.MoveNext())
        {
            args.Add(en.Current);
        }
        var count = args.Length;
        return new(args.ToSegment());
    }
}

/// <summary>A structure representing a immutable sequence of function parameters.</summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public readonly struct TinyVec<T>
    : IReadOnlyList<T>
{
    private readonly int _length;
    [AllowNull] private readonly T _arg0;
    [AllowNull] private readonly T _arg1;
    [AllowNull] private readonly T _arg2;
    [AllowNull] private readonly T _arg3;
    private readonly ArraySegment<T> _params;

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <param name="length">The number of non default values.</param>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    /// <param name="arg3">The fourth value.</param>
    internal TinyVec(int length, [AllowNull] in T arg0, [AllowNull] in T arg1, [AllowNull] in T arg2, [AllowNull] in T arg3)
    {
        length.ValidateArgRange(length <= 4);

        _params = default;
        _length = length;
        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    /// <summary>Initializes a new parameter span with a sequence of parameters.</summary>
    /// <param name="segment">The segment of parameters.</param>
    internal TinyVec(in ArraySegment<T> segment)
    {
        _params = segment;
        _length = segment.Count;

        int i = 0;
        _arg0 = _length > 0 ? segment[i++] : default!;
        _arg1 = _length > 1 ? segment[i++] : default!;
        _arg2 = _length > 2 ? segment[i++] : default!;
        _arg3 = _length > 3 ? segment[i] : default!;
    }

    /// <inheritdoc/>
    public int Count => _length;

    /// <inheritdoc cref="IReadOnlyVector{T}.IsEmpty"/>
    public bool IsEmpty => 0 >= (uint)_length;

    /// <inheritdoc/>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            index.ValidateArgRange(index >= 0 && index < Count);
            return index switch
            {
                0 => _arg0!,
                1 => _arg1!,
                2 => _arg2!,
                3 => _arg3!,
                _ => _params[index],
            };
        }
    }

    /// <inheritdoc/>
    public void CopyTo(Span<T> destination)
    {
        if (_params.Count > 0)
        {
            _params.AsSpan().CopyTo(destination);
        }

        if ((uint)_length <= (uint)destination.Length)
        {
            SetBlock(destination);
        }
    }

    /// <inheritdoc/>
    public bool TryCopyTo(Span<T> destination)
    {
        if (_params.Count > 0)
        {
            return _params.AsSpan().TryCopyTo(destination);
        }
        else if ((uint)_length <= (uint)destination.Length)
        {
            SetBlock(destination);
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBlock(Span<T> destination)
    {
        int index = 0;
        switch (_length)
        {
            case 4:
                destination[index++] = _arg0!;
                destination[index++] = _arg1!;
                destination[index++] = _arg2!;
                destination[index] = _arg3!;
                break;

            case 3:
                destination[index++] = _arg0!;
                destination[index++] = _arg1!;
                destination[index] = _arg2!;
                break;

            case 2:
                destination[index++] = _arg0!;
                destination[index] = _arg1!;
                break;

            case 1:
                destination[index] = _arg0!;
                break;
            default:
                // noop
                break;
        }
    }

    /// <inheritdoc/>
    public bool Equals(in TinyVec<T> other)
    {
        return this == other;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is TinyVec<T> other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
    ///     this does *not* necessarily check to see if the *contents* are equal.
    /// </summary>
    public static bool operator ==(TinyVec<T> left, TinyVec<T> right)
    {
        if (left._length != right._length)
        {
            return false;
        }

        if (left._length > 4)
        {
            return left._params.AsSpan() == right._params.AsSpan();
        }

        return EqualityComparer<T>.Default.Equals(left._arg0, right._arg0)
            && EqualityComparer<T>.Default.Equals(left._arg1, right._arg1)
            && EqualityComparer<T>.Default.Equals(left._arg2, right._arg2)
            && EqualityComparer<T>.Default.Equals(left._arg3, right._arg3);
    }

    /// <summary>
    ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
    ///     this does *not* check to see if the *contents* are equal.
    /// </summary>
    public static bool operator !=(TinyVec<T> left, TinyVec<T> right) => !(left == right);

    /// <summary>Retrieves the backing span of the <see cref="TinyVec{T}"/> or allocates a array which is returned as a span.</summary>
    /// <returns>The span containing all items.</returns>
    [Pure]
    public ReadOnlySpan<T> ToSpan() => ToSpan(false);

    /// <summary>Returns the span representation of the <see cref="TinyVec{T}"/>.</summary>
    /// <param name="onlyIfCheap">Whether return an empty span instead of allocating an array, if no span is backing the <see cref="TinyVec{T}"/>.</param>
    /// <returns>The span containing all items.</returns>
    [Pure]
    public ReadOnlySpan<T> ToSpan(bool onlyIfCheap)
    {
        if (onlyIfCheap || IsEmpty || _params.Count > 0)
        {
            return _params.Array is null ? default : new ReadOnlySpan<T>(_params.Array, _params.Offset, _params.Count);
        }

        T[] array = _length switch
        {
            4 => new[] { _arg0!, _arg1!, _arg2!, _arg3! },
            3 => new[] { _arg0!, _arg1!, _arg2! },
            2 => new[] { _arg0!, _arg1! },
            1 => new[] { _arg0! },
            _ => Array.Empty<T>()
        };

        return new ReadOnlySpan<T>(array, 0, _length);
    }

    /// <summary>Initializes a new span from the value.</summary>
    /// <param name="self">The value.</param>
    public static implicit operator TinyVec<T>(in T self) => TinyVec.From(self);

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    public static implicit operator TinyVec<T>(in T[] self) => TinyVec.From(self);

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    public static implicit operator TinyVec<T>(in ArraySegment<T> self) => TinyVec.From(self);

    private string GetDebuggerDisplay()
    {
        StrBuilder vsb = new(stackalloc char[256]);
        vsb.Append("Count = ");
        vsb.Append(Count.ToString(CultureInfo.InvariantCulture));
        vsb.Append(", Params = {");

        int last = _length - 1;
        for (int i = 0; i < last; i++)
        {
            vsb.Append(this[i]!.ToString());
            vsb.Append(", ");
        }

        if (!IsEmpty)
        {
            vsb.Append(this[last]!.ToString());
        }

        vsb.Append('}');
        return vsb.ToString();
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Enumerates the elements of a <see cref="TinyVec{T}"/>.</summary>
    public struct Enumerator : IEnumerator<T>
    {
        private TinyVec<T> _array;
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="TinyVec">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(in TinyVec<T> TinyVec)
        {
            _array = TinyVec;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = _index + 1;

            if ((uint)index < (uint)_array.Count)
            {
                _index = index;
                return true;
            }

            return false;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array[_index]!;
        }

        object? IEnumerator.Current => Current;

        /// <summary>Resets the enumerator to the initial state.</summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>Disposes the enumerator.</summary>
        public void Dispose()
        {
            this = default;
        }
    }
}
