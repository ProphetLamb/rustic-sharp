using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rustic;

namespace Rustic.Memory;

/// <summary>Wrapper around <see cref="ReadOnlySpan{T}"/> that allows indexed access in reversed order.</summary>
public readonly ref struct ReversibleIndexedSpan<T> {
    private readonly bool _reverse;
    /// <summary>The underlying data span, in the original order.</summary>
    public readonly ReadOnlySpan<T> Span;

    /// <summary>Initializes a new <see cref="ReversibleIndexedSpan"/>.</summary>
    /// <param name="span">The span</param>
    /// <param name="reverse">If the indexer access is reversed</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReversibleIndexedSpan(ReadOnlySpan<T> span, bool reverse) {
        Span = span;
        _reverse = reverse;
    }

    /// <summary>Indicates whether the <see cref="ReversibleIndexedSpan{T}"/> is reversed or not.</summary>
    public bool IsReverse => _reverse;

    /// <summary>Accesses an element in the span, respecting index reversing.</summary>
    /// <param name="index">The index of the element.</param>
    public ref readonly T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (!_reverse) {
                return ref Span[index];
            }

            return ref Span[Span.Length - 1 - index];
        }
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.IsEmpty"/>
    public bool IsEmpty {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.IsEmpty;
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Length"/>
    public int Length {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.Length;
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Slice(int)"/>
    public ReversibleIndexedSpan<T> Slice(int start) {
        if (!_reverse) {
            return new(Span.Slice(start), _reverse);
        }

        int count = Span.Length - start;
        int end = start + count;
        int reverseStart = Span.Length - end;
        return new(Span.Slice(reverseStart, count), _reverse);
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReversibleIndexedSpan<T> Slice(int start, int count) {
        if (!_reverse) {
            return new(Span.Slice(start, count), _reverse);
        }

        int end = start + count;
        int reverseStart = Span.Length - end;
        return new(Span.Slice(reverseStart, count), _reverse);
    }

    /// <summary>Slices the span from the end</summary>
    /// <param name="end">The number of elements to slice from the end.</param>
    /// <returns>The sliced span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReversibleIndexedSpan<T> SliceEnd(int end) {
        if (!_reverse) {
            return new(Span.Slice(0, Length - end), _reverse);
        }

        return new(Span.Slice(end), _reverse);
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.CopyTo(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> span) {
        if (!_reverse) {
            Span.CopyTo(span);
        }

        if (!TryCopyTo(span)) {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(span), span.Length);
        }
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.TryCopyTo(Span{T})"/>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool TryCopyTo(Span<T> span) {
        if (!_reverse) {
            return Span.TryCopyTo(span);
        }

        return MemoryCopyHelper.TryCopyToReversed(Span, span);
    }

    /// <summary>Creates a new not reversed span.</summary>
    /// <param name="span">The data</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReversibleIndexedSpan<T>(ReadOnlySpan<T> span) => new(span, false);

    /// <summary>Returns a span in correct order.</summary>
    /// <remarks>If reversed allocates a array and returns a span over the array.</remarks>
    public ReadOnlySpan<T> ToSpan() {
        if (!_reverse) {
            return Span;
        }

        T[] array = new T[Length];
        Span<T> span = array.AsSpan();
        MemoryCopyHelper.CopyToReversed(Span, span);
        return span;
    }

    /// <summary>Returns a span in correct order.</summary>
    /// <param name="sharedPoolArray">The array from the shared pool, if any.</param>
    /// <remarks>If reversed retrieves an array from the <see cref="ArrayPool{T}.Shared"/> and returns a span a portion over the array.</remarks>
    public ReadOnlySpan<T> ToSpan(out T[]? sharedPoolArray) {
        if (!_reverse) {
            sharedPoolArray = null;
            return Span;
        }

        sharedPoolArray = ArrayPool<T>.Shared.Rent(Length);
        Span<T> span = sharedPoolArray.AsSpan(0, Length);
        MemoryCopyHelper.CopyToReversed(Span, span);
        return span;
    }

    /// <summary>Returns a new <see cref="ReversibleIndexedSpan{T}"/> over the data in opposite direction.</summary>
    public ReversibleIndexedSpan<T> Reverse() => new(Span, !_reverse);

    /// <inheritdoc />
    public override string ToString() {
        if (!_reverse) {
            return Span.ToString();
        }

        T[]? poolArray;
        string result;
        if (typeof(T) == typeof(char)) {
            // char spans are common and we can alleviate pressure on the heap by using stackalloc for the temporary buffer at this point.
            poolArray = Length <= 1024 ? null : ArrayPool<T>.Shared.Rent(Length);
            Span<char> span = poolArray is null ? stackalloc char[Length] : Unsafe.As<T[], char[]>(ref poolArray).AsSpan(0, Length);
            MemoryCopyHelper.CopyToReversed(
                ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(Span)),
                ref MemoryMarshal.GetReference(span),
                (nuint)Length
            );

            result = span.ToString();
        } else {
            poolArray = ArrayPool<T>.Shared.Rent(Length);
            Span<T> span = poolArray.AsSpan(0, Length);
            MemoryCopyHelper.CopyToReversed(Span, span);
            result = span.ToString();
        }

        if (poolArray is not null) {
            ArrayPool<T>.Shared.Return(poolArray);
        }
        return result;
    }

#pragma warning disable CS0809

    /// <inheritdoc cref="Object.Equals(Object)" />
    [Obsolete("Not applicable to a ref struct.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) {
        ThrowHelper.ThrowNotSupportedException();
        return default!; // unreachable.
    }

    /// <inheritdoc cref="Object.GetHashCode" />
    [Obsolete("Not applicable to a ref struct.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() {
        ThrowHelper.ThrowNotSupportedException();
        return default!; // unreachable.
    }

#pragma warning restore CS0809
}
