﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Memory;

/// <summary>Partially inlined immutable collection of function parameters.</summary>
public static class TinyRoSpan {
    /// <summary>Returns an empty <see cref="TinyRoSpan{T}"/>.</summary>
    /// <typeparam name="T">The type of the span.</typeparam>
    /// <returns>An empty <see cref="TinyRoSpan{T}"/>.</returns>
    public static TinyRoSpan<T> Empty<T>() {
        return new(ReadOnlySpan<T>.Empty);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    public static TinyRoSpan<T> From<T>(T arg0) {
        return new(1, arg0, arg0, arg0, arg0);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    public static TinyRoSpan<T> From<T>(T arg0, T arg1) {
        return new(2, arg0, arg1, arg1, arg1);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    public static TinyRoSpan<T> From<T>(T arg0, T arg1, T arg2) {
        return new(3, arg0, arg1, arg2, arg2);
    }

    /// <summary>Initializes a new parameter span with one value.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    /// <param name="arg3">The fourth value.</param>
    public static TinyRoSpan<T> From<T>(T arg0, T arg1, T arg2, T arg3) {
        return new(4, arg0, arg1, arg2, arg3);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    public static TinyRoSpan<T> From<T>(ReadOnlySpan<T> values) {
        return new(values);
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    public static TinyRoSpan<T> From<T>(Span<T> values) {
        return new(values);
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    public static TinyRoSpan<T> From<T>(T[] values, int start) {
        return new(new ReadOnlySpan<T>(values, start, values.Length - start));
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values collection.</param>
    /// <param name="start">The zero-based index of the first value.</param>
    /// <param name="length">The number of values form the <paramref name="start"/>.</param>
    public static TinyRoSpan<T> From<T>(T[] values, int start, int length) {
        return new(new ReadOnlySpan<T>(values, start, length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The sequence of values.</param>
    /// <remarks>
    ///     If <paramref name="values"/> if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
    ///     then <see cref="TinyRoSpan{T}"/> uses the allocated memory of <paramref name="values"/>.
    /// <br/>
    ///     If a deep copy is desired use <see cref="Copy"/>.
    /// </remarks>
    public static TinyRoSpan<T> From<T>(T[] values) {
        return new(new ReadOnlySpan<T>(values, 0, values.Length));
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The sequence of values.</param>
    /// <remarks>
    ///     If <paramref name="values"/> if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
    ///     then <see cref="TinyRoSpan{T}"/> uses the allocated memory of <paramref name="values"/>.
    /// <br/>
    ///     If a deep copy is desired use <see cref="Copy"/>.
    /// </remarks>
    public static TinyRoSpan<T> From<T>(ReadOnlyMemory<T> values) {
        return From(values.Span);
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The sequence of values.</param>
    /// <remarks>
    ///     If <paramref name="values"/> if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
    ///     then <see cref="TinyRoSpan{T}"/> uses the allocated memory of <paramref name="values"/>.
    /// <br/>
    ///     If a deep copy is desired use <see cref="Copy"/>.
    /// </remarks>
    public static TinyRoSpan<T> From<T>(Memory<T> values) {
        return From(values.Span);
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Does not allocate or copy.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The sequence of values.</param>
    /// <remarks>
    ///     If <paramref name="values"/> if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
    ///     then <see cref="TinyRoSpan{T}"/> uses the allocated memory of <paramref name="values"/>.
    /// <br/>
    ///     If a deep copy is desired use <see cref="Copy"/>.
    /// </remarks>
    public static TinyRoSpan<T> From<T>(ArraySegment<T> values) {
        return new(values);
    }

    /// <summary>Initializes a new parameter span with a sequence of values. Performs a shallow-copy of the sequence of values.</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <param name="values">The sequence of values.</param>
    public static TinyRoSpan<T> Copy<T, E>(E values)
        where E : IEnumerable<T> {
        using IEnumerator<T>? en = values.GetEnumerator();
        if (!en.MoveNext()) {
            return default;
        }

        T? arg0 = en.Current;
        if (!en.MoveNext()) {
            return From(arg0);
        }

        T? arg1 = en.Current;
        if (!en.MoveNext()) {
            return From(arg0, arg1);
        }

        T? arg2 = en.Current;
        if (!en.MoveNext()) {
            return From(arg0, arg1, arg2);
        }

        T? arg3 = en.Current;
        if (!en.MoveNext()) {
            return From(arg0, arg1, arg2, arg3);
        }

        BufWriter<T> args = new(8) {
            arg0,
            arg1,
            arg2,
            arg3,
            en.Current,
        };

        while (en.MoveNext()) {
            args.Add(en.Current);
        }

        int count = args.Length;
        return new(args.ToSegment());
    }

    /// <summary>
    ///     Determines whether two sequences are equal by comparing the elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static bool SequenceEquals<T>(in this TinyRoSpan<T> span, in TinyRoSpan<T> other)
        where T : IEquatable<T> {
        // If we have 4 or less elements the == operator performs a sequence equals.
        // If we have more then 4 elements we compare the pointers of the internal span.
        if (span == other) {
            return true;
        }

        // Internal spans may not be
        if (span.Length != other.Length || span.Length <= 4) {
            return false;
        }

        // The internal spans are not the same, but are present, so the operation is always cheap.
        return span.ToSpan(true).SequenceEqual(other.ToSpan(true));
    }
}

/// <summary>A structure representing a immutable sequence of function parameters.</summary>
/// <typeparam name="T"></typeparam>
[StructLayout(LayoutKind.Sequential), DebuggerDisplay("{GetDebuggerDisplay(),nq}"),]
public readonly ref struct TinyRoSpan<T> {
    [AllowNull]
    private readonly T _arg0;
    [AllowNull]
    private readonly T _arg1;
    [AllowNull]
    private readonly T _arg2;
    [AllowNull]
    private readonly T _arg3;
    private readonly int _length;
    private readonly ReadOnlySpan<T> _values;

    /// <summary>Initializes a new parameter span with values.</summary>
    /// <param name="length">The number of non default values.</param>
    /// <param name="arg0">The first value.</param>
    /// <param name="arg1">The second value.</param>
    /// <param name="arg2">The third value.</param>
    /// <param name="arg3">The fourth value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TinyRoSpan(int length, [AllowNull] T arg0, [AllowNull] T arg1, [AllowNull] T arg2, [AllowNull] T arg3) {
        ThrowHelper.ArgumentInRange(length, length <= 4);

        _values = default;
        _length = length;
        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    /// <summary>Initializes a new parameter span with a sequence of values.</summary>
    /// <param name="values">The values collection.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TinyRoSpan(ReadOnlySpan<T> values) {
        _values = values;
        _length = values.Length;
        _arg0 = default!;
        _arg1 = default!;
        _arg2 = default!;
        _arg3 = default!;
    }

    /// <summary>The number of items in the params span.</summary>
    public int Length => _length;

    /// <summary>Returns true if Length is 0.</summary>
    public bool IsEmpty {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0 >= (uint) _length;
    }

    /// <inheritdoc cref="IReadOnlyList{T}.this" />
    public T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            ThrowHelper.ArgumentInRange(index, index >= 0 && index < Length);
            if (!_values.IsEmpty) {
                return _values[index];
            }

            return index switch {
                0 => _arg0!,
                1 => _arg1!,
                2 => _arg2!,
                3 => _arg3!,
                _ => ThrowUnreachable(),
            };
        }
    }

#pragma warning disable CS0809

    /// <inheritdoc cref="Object.Equals(Object)" />
    [Obsolete("Not applicable to a ref struct."), EditorBrowsable(EditorBrowsableState.Never),]
    public override bool Equals(object? obj) {
        ThrowHelper.ThrowNotSupportedException();
        return default!; // unreachable.
    }

    /// <inheritdoc cref="Object.GetHashCode" />
    [Obsolete("Not applicable to a ref struct."), EditorBrowsable(EditorBrowsableState.Never),]
    public override int GetHashCode() {
        ThrowHelper.ThrowNotSupportedException();
        return default!; // unreachable.
    }

#pragma warning restore CS0809

    /// <inheritdoc cref="Span{T}.CopyTo"/>
    public void CopyTo(Span<T> destination) {
        if (!_values.IsEmpty) {
            _values.CopyTo(destination);
        }

        if ((uint) _length <= (uint) destination.Length) {
            SetBlock(destination);
        }
    }

    /// <inheritdoc cref="Span{T}.TryCopyTo"/>
    public bool TryCopyTo(Span<T> destination) {
        if (!_values.IsEmpty) {
            return _values.TryCopyTo(destination);
        } else if ((uint) _length <= (uint) destination.Length) {
            SetBlock(destination);
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBlock(Span<T> destination) {
        int index = 0;
        switch (_length) {
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
            // Nothing to do.
            break;
        }
    }

    /// <summary>
    ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
    ///     this does *not* check to see if the *contents* are equal.
    /// </summary>
    public static bool operator ==(TinyRoSpan<T> left, TinyRoSpan<T> right) {
        if (left._length != right._length) {
            return false;
        }

        if (left._length > 4) {
            return left._values == right._values;
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
    public static bool operator !=(TinyRoSpan<T> left, TinyRoSpan<T> right) {
        return !(left == right);
    }

    /// <summary>Retrieves the backing span of the <see cref="TinyRoSpan{T}"/> or allocates a array which is returned as a span.</summary>
    /// <returns>The span containing all items.</returns>
    /// <remarks>When using .NET Standard 2.1 or greater, or .NET Core 2.1 or greater the operation always is cheap and never allocates.</remarks>
    public ReadOnlySpan<T> ToSpan() {
        return ToSpan(false);
    }

    /// <summary>Returns the span representation of the <see cref="TinyRoSpan{T}"/>.</summary>
    /// <param name="onlyIfCheap">Whether return an empty span instead of allocating an array, if no span is backing the <see cref="TinyRoSpan{T}"/>.</param>
    /// <returns>The span containing all items.</returns>
    /// <remarks>When using .NET Standard 2.1 or greater, or .NET Core 2.1 or greater the operation always is cheap and never allocates.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> ToSpan(bool onlyIfCheap) {
        if (IsEmpty || !_values.IsEmpty) {
            return _values;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _arg0), _length);
#else
        if (onlyIfCheap) {
            return default;
        }

        var array = _length switch {
            4 => new[] { _arg0!, _arg1!, _arg2!, _arg3! },
            3 => new[] { _arg0!, _arg1!, _arg2! },
            2 => new[] { _arg0!, _arg1! },
            1 => new[] { _arg0! },
            _ => default // unreachable
        };

        return new ReadOnlySpan<T>(array!, 0, _length);
#endif
    }

    /// <summary>Initializes a new span from the value.</summary>
    /// <param name="self">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in T self) {
        return TinyRoSpan.From(self);
    }

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in ReadOnlySpan<T> self) {
        return TinyRoSpan.From(self);
    }

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in Span<T> self) {
        return TinyRoSpan.From(self);
    }

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in T[] self) {
        return TinyRoSpan.From(self);
    }

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in (T, T) self) {
        return TinyRoSpan.From(self.Item1, self.Item2);
    }

    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TinyRoSpan<T>(in (T, T, T) self) {
        return TinyRoSpan.From(self.Item1, self.Item2, self.Item3);
    }

#if !NET472
    /// <summary>Initializes a new span from the sequence.</summary>
    /// <param name="self">The sequence of values.</param>
    public static implicit operator TinyRoSpan<T>(in (T, T, T, T) self) {
        return TinyRoSpan.From(self.Item1, self.Item2, self.Item3, self.Item4);
    }
#endif

    private string GetDebuggerDisplay() {
        StrBuilder sb = new();
        sb.Append("Length = ");
        sb.Append(Length.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat));
        sb.Append(", Params = {");

        int last = _length - 1;
        for (int i = 0; i < last; i++) {
            sb.Append(this[i]?.ToString());
            sb.Append(", ");
        }

        if (!IsEmpty) {
            sb.Append(this[last]?.ToString());
        }

        sb.Append('}');
        return sb.ToString();
    }


    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public Enumerator GetEnumerator() {
        return new(this);
    }

    [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining),]
    private static T ThrowUnreachable() {
        ThrowHelper.ThrowUnreachableException();
        return default!; // unreachable.
    }

    /// <summary>Enumerates the elements of a <see cref="TinyRoSpan{T}"/>.</summary>
    public ref struct Enumerator {
        /// <summary>The span being enumerated.</summary>
        private TinyRoSpan<T> _span;

        /// <summary>The next index to yield.</summary>
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="TinyRoSpan">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(TinyRoSpan<T> TinyRoSpan) {
            _span = TinyRoSpan;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            int index = _index + 1;

            if ((uint) index < (uint) _span.Length) {
                _index = index;
                return true;
            }

            return false;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _span[_index]!;
        }

        /// <summary>Resets the enumerator to the initial state.</summary>
        public void Reset() {
            _index = -1;
        }

        /// <summary>Disposes the enumerator.</summary>
        public void Dispose() {
            this = default;
        }
    }
}
