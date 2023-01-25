using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Memory;

/// <summary>Partially inlined immutable collection of function parameters.</summary>
public static class TinyRoVec
{
    /// <summary>Returns an empty <see cref="TinyRoVec{T}"/>.</summary>
    /// <typeparam name="T">The type of the span.</typeparam>
    /// <returns>An empty <see cref="TinyRoVec{T}"/>.</returns>
    public static TinyRoVec<T> Empty<T>() => default;

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    public static TinyRoVec<T> From<T>(in T arg0)
    {
        return new(1, arg0, default, default, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    public static TinyRoVec<T> From<T>(in T arg0, in T arg1)
    {
        return new(2, arg0, arg1, default, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    public static TinyRoVec<T> From<T>(in T arg0, in T arg1, in T arg2)
    {
        return new(3, arg0, arg1, arg2, default);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    /// <param name="arg3">The fourth value.</param>
    public static TinyRoVec<T> From<T>(in T arg0, in T arg1, in T arg2, in T arg3)
    {
        return new(4, arg0, arg1, arg2, arg3);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values array.</param>
    public static TinyRoVec<T> From<T>(in ArraySegment<T> values)
    {
        return new(values);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values array.</param>
    public static TinyRoVec<T> From<T>(T[] values)
    {
        return new(new ArraySegment<T>(values, 0, values.Length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    public static TinyRoVec<T> From<T>(T[] values, int start)
    {
        return new(new ArraySegment<T>(values, start, values.Length - start));
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    /// <param name="length">The number of values form the <paramref name="start"/>.</param>
    public static TinyRoVec<T> From<T>(T[] values, int start, int length)
    {
        return new(new ArraySegment<T>(values, start, length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Performs a shallow copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <param name="values">The sequence of values.</param>
    public static TinyRoVec<T> Copy<T, E>(E values)
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
        var arg0 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0);
        }
        var arg1 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0, arg1);
        }
        var arg2 = en.Current;
        if (!en.MoveNext())
        {
            return From(arg0, arg1, arg2);
        }
        var arg3 = en.Current;
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
public readonly struct TinyRoVec<T>
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
    internal TinyRoVec(int length, [AllowNull] in T arg0, [AllowNull] in T arg1, [AllowNull] in T arg2, [AllowNull] in T arg3)
    {
        ThrowHelper.ArgumentInRange(length, length <= 4);

        _params = default;
        _length = length;
        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    /// <summary>Initializes a new parameter span with a sequence of parameters.</summary>
    /// <param name="segment">The segment of parameters.</param>
    internal TinyRoVec(in ArraySegment<T> segment)
    {
        _params = segment;
        _length = segment.Count;
        _arg0 = default!;
        _arg1 = default!;
        _arg2 = default!;
        _arg3 = default!;
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
            ThrowHelper.ArgumentInRange(index, index >= 0 && index < Count);
            if (_params.Array is not null) {
                return _params.Array[index];
            }
            return index switch
            {
                0 => _arg0!,
                1 => _arg1!,
                2 => _arg2!,
                3 => _arg3!,
                _ => UnreachableException.Throw<T>(),
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
        var index = 0;
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
    public bool Equals(in TinyRoVec<T> other)
    {
        return this == other;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is TinyRoVec<T> other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return Count;
    }

    /// <summary>
    ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
    ///     this does *not* necessarily check to see if the *contents* are equal.
    /// </summary>
    public static bool operator ==(TinyRoVec<T> left, TinyRoVec<T> right)
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
    public static bool operator !=(TinyRoVec<T> left, TinyRoVec<T> right) => !(left == right);

    /// <summary>Retrieves the backing span of the <see cref="TinyRoVec{T}"/> or allocates a array which is returned as a span.</summary>
    /// <returns>The span containing all items.</returns>
    /// <remarks>When using .NET Standard 2.1 or greater, or .NET Core 2.1 or greater the operation always is cheap and never allocates.</remarks>
    [Pure]
    public ReadOnlySpan<T> ToSpan() => ToSpan(false);

    /// <summary>Returns the span representation of the <see cref="TinyRoVec{T}"/>.</summary>
    /// <param name="onlyIfCheap">Whether return an empty span instead of allocating an array, if no span is backing the <see cref="TinyRoVec{T}"/>.</param>
    /// <returns>The span containing all items.</returns>
    /// <remarks>When using .NET Standard 2.1 or greater, or .NET Core 2.1 or greater the operation always is cheap and never allocates.</remarks>
    [Pure]
    public ReadOnlySpan<T> ToSpan(bool onlyIfCheap)
    {
        if (IsEmpty || _params.Count > 0)
        {
            return _params.Array is null ? default : new ReadOnlySpan<T>(_params.Array, _params.Offset, _params.Count);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _arg0), _length);
#endif
        if (onlyIfCheap) {
            return default;
        }

        var array = _length switch
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
    public static implicit operator TinyRoVec<T>(in T self) => TinyRoVec.From(self);

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    public static implicit operator TinyRoVec<T>(in T[] self) => TinyRoVec.From(self);

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    public static implicit operator TinyRoVec<T>(in ArraySegment<T> self) => TinyRoVec.From(self);

    private string GetDebuggerDisplay()
    {
        StrBuilder vsb = new(stackalloc char[256]);
        vsb.Append("Length = ");
        vsb.Append(Count.ToString(CultureInfo.InvariantCulture));
        vsb.Append(", Params = {");

        var last = _length - 1;
        for (var i = 0; i < last; i++)
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

    /// <summary>Enumerates the elements of a <see cref="TinyRoVec{T}"/>.</summary>
    public struct Enumerator : IEnumerator<T>
    {
        private TinyRoVec<T> _array;
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="TinyRoVec">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(in TinyRoVec<T> TinyRoVec)
        {
            _array = TinyRoVec;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;

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
