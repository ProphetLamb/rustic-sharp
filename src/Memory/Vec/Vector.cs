using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Rustic.Memory;

/// <summary>Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.</summary>
/// <typeparam name="T">The type of items of the vector.</typeparam>
public interface IReadOnlyVector<T>
    : IReadOnlyList<T> {
    /// <summary>Returns a value that indicates whether the vector is empty.</summary>
    [Pure]
    bool IsEmpty { get; }

    /// <summary>Returns a value that indicates whether the vector is at its default value, no memory is allocated.</summary>
    [Pure]
    bool IsDefault { get; }

    /// <summary>Returns the number of elements in the vector.</summary>
    new int Count { get; }

    /// <inheritdoc cref="List{T}.Capacity"/>
    int Capacity { get; }

    /// <summary>Gets the element at the specified <paramref name="index"/>.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    [Pure]
    new ref readonly T this[int index] { get; }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Gets the element at the specified <paramref name="index"/>.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    [Pure]
    ref readonly T this[Index index] { get; }
#endif

    /// <summary>Creates a new span over a target vector.</summary>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    ReadOnlySpan<T> AsSpan(int start, int length);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified <paramref name="item"/> in the vector.
    /// </summary>
    /// <typeparam name="E">The type of the comparer.</typeparam>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    /// <param name="comparer">The comparer implementation to use when comparing elements.</param>
    [Pure]
    int IndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>;

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified <paramref name="item"/> in the vector.
    /// </summary>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    [Pure]
    int IndexOf(int start, int count, in T item);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified <paramref name="item"/> in the vector.
    /// </summary>
    /// <typeparam name="E">The type of the comparer.</typeparam>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    /// <param name="comparer">The comparer implementation to use when comparing elements.</param>
    [Pure]
    int LastIndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>;

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified <paramref name="item"/> in the vector.
    /// </summary>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    [Pure]
    int LastIndexOf(int start, int count, in T item);

    /// <summary>Searches a range of elements in the sorted vector for an element using the <see cref="Comparer{T}.Default"/> and returns the zero-based index of the element.</summary>
    /// <typeparam name="C">The type of the comparer.</typeparam>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    /// <param name="comparer">The comparer implementation to use when comparing elements.</param>
    [Pure]
    int BinarySearch<C>(int start, int count, in T item, in C comparer)
        where C : IComparer<T>;

    /// <summary>Searches a range of elements in the sorted vector for an element using the <see cref="Comparer{T}.Default"/> and returns the zero-based index of the element.</summary>
    /// <param name="start">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The element to locate.</param>
    [Pure]
    int BinarySearch(int start, int count, in T item);

    /// <inheritdoc cref="Span{T}.TryCopyTo"/>
    bool TryCopyTo(Span<T> destination);
}

/// <summary>Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.</summary>
/// <typeparam name="T">The type of items of the vector.</typeparam>
public interface IVector<T>
    : IReadOnlyVector<T>, IList<T>, ICollection {
    /// <summary>Gets or sets the element at the specified <paramref name="index"/>.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    new ref T this[int index] { get; }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Gets or sets the element at the specified <paramref name="index"/>.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    new ref T this[Index index] { get; }
#endif

    /// <summary>Returns the number of elements in the vector.</summary>
    new int Count { get; }

    /// <summary>Ensures that the collection can contain at least <paramref name="additionalCapacity"/> more elements.</summary>
    /// <param name="additionalCapacity">The number of additional elements the collection must be able to contain.</param>
    /// <returns>The new capacity of the collection.</returns>
    int Reserve(int additionalCapacity);

    /// <summary>Inserts a range of <paramref name="values"/> into the vector at the specified index.</summary>
    /// <param name="index">The index at which to insert the first element.</param>
    /// <param name="values">The collection of values to insert.</param>
    void InsertRange(int index, ReadOnlySpan<T> values);

    /// <summary>Removes a specified range of values for the vector.</summary>
    /// <param name="start">The zero-based starting index of the range to remove.</param>
    /// <param name="count">The length of the range to remove.</param>
    void RemoveRange(int start, int count);

    /// <summary>Sorts a range of elements in the vector using the specified <paramref name="comparer"/>.</summary>
    /// <typeparam name="C">The type of the comparer.</typeparam>
    /// <param name="start">The zero-based starting index of the range to sort.</param>
    /// <param name="count">The length of the range to sort.</param>
    /// <param name="comparer">The comparer implementation to use when comparing elements.</param>
    void Sort<C>(int start, int count, in C comparer)
        where C : IComparer<T>;

    /// <summary>Sorts a range of elements in the vector using the <see cref="Comparer{T}.Default"/>.</summary>
    /// <param name="start">The zero-based starting index of the range to sort.</param>
    /// <param name="count">The length of the range to sort.</param>
    void Sort(int start, int count);

    /// <summary>Reveses the order of a range of elements in the vector.</summary>
    /// <param name="start">The zero-based starting index of the range to reverse.</param>
    /// <param name="count">The length of the range to reverse.</param>
    void Reverse(int start, int count);
}

/// <summary>Collection of extensions and utility functions related to <see cref="IReadOnlyVector{T}"/>.</summary>
public static class ReadOnlyVectorTraits {
    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self) {
        return self.AsSpan(0, self.Count);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self, Index index)
    {
        var len = self.Count;
        var off = index.GetOffset(len);
        return self.AsSpan(off, len - off);
    }

    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self, Range range)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.AsSpan(off, cnt);
    }

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this IReadOnlyVector<T> self, Range range, in T item)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.IndexOf(off, cnt, item);
    }

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T, E>(this IReadOnlyVector<T> self, Range range, in T item, in E comparer)
        where E : IEqualityComparer<T>
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.IndexOf(off, cnt, item, comparer);
    }
#endif

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int IndexOf<T>(this IReadOnlyVector<T> self, in T item) {
        return self.IndexOf(0, self.Count, item);
    }

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int IndexOf<T, E>(this IReadOnlyVector<T> self, in T item, in E comparer)
        where E : IEqualityComparer<T> {
        return self.IndexOf(0, self.Count, item, comparer);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this IReadOnlyVector<T> self, Range range, in T item)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.LastIndexOf(off, cnt, item);
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T, E>(this IReadOnlyVector<T> self, Range range, in T item, E comparer)
        where E : IEqualityComparer<T>
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.LastIndexOf(off, cnt, item, comparer);
    }
#endif

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int LastIndexOf<T>(this IReadOnlyVector<T> self, in T item) {
        return self.LastIndexOf(0, self.Count, item);
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int LastIndexOf<T, E>(this IReadOnlyVector<T> self, in T item, E comparer)
        where E : IEqualityComparer<T> {
        return self.LastIndexOf(0, self.Count, item, comparer);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T>(this IReadOnlyVector<T> self, Range range, in T item)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.BinarySearch(off, cnt, item);
    }

    /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, C>(this IReadOnlyVector<T> self, Range range, in T item, C comparer)
        where C : IComparer<T>
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        return self.BinarySearch(off, cnt, item, comparer);
    }
#endif

    /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int BinarySearch<T>(this IReadOnlyVector<T> self, in T item) {
        return self.BinarySearch(0, self.Count, item);
    }

    /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int BinarySearch<T, C>(this IReadOnlyVector<T> self, in T item, C comparer)
        where C : IComparer<T> {
        return self.BinarySearch(0, self.Count, item, comparer);
    }

    /// <inheritdoc cref="Span{T}.CopyTo"/>
    public static void CopyTo<T>(this IReadOnlyVector<T> self, Span<T> destination) {
        if (!self.TryCopyTo(destination)) {
            ThrowHelper.ThrowArgumentException("Unable to copy the vector to the destination", nameof(destination));
        }
    }

    /// <inheritdoc cref="List{T}.Contains"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static bool Contains<T>(this IReadOnlyVector<T> self, in T item) {
        return self.IndexOf(in item) >= 0;
    }
}

/// <summary>Collection of extensions and utility functions related to <see cref="IVector{T}"/>.</summary>
public static class VectorTraits {
    /// <inheritdoc cref="List{T}.AddRange"/>
    public static void AddRange<T>(this IVector<T> self, ReadOnlySpan<T> values) {
        self.InsertRange(self.Count, values);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc cref="List{T}.InsertRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InsertRange<T>(this IVector<T> self, Index index, ReadOnlySpan<T> values)
    {
        var off = index.GetOffset(self.Count);
        self.InsertRange(off, values);
    }
#endif

    /// <inheritdoc cref="List{T}.Remove"/>
    public static bool Remove<T>(this IVector<T> self, in T item) {
        int index = self.IndexOf(in item);
        if (index >= 0) {
            self.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="List{T}.Remove"/>
    public static bool Remove<T, E>(this IVector<T> self, in T item, E comparer)
        where E : IEqualityComparer<T> {
        int index = self.IndexOf(in item, comparer);
        if (index >= 0) {
            self.RemoveAt(index);
            return true;
        }

        return false;
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc cref="List{T}.RemoveRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveRange<T>(this IVector<T> self, Range range)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        self.RemoveRange(off, cnt);
    }

    /// <inheritdoc cref="List{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAt<T>(this IVector<T> self, Index index)
    {
        var off = index.GetOffset(self.Count);
        self.RemoveAt(off);
    }
#endif

    /// <summary>Removes the element at the specified <paramref name="index"/> from the vector by over-writing it with the last element.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <remarks>No block of elements in moved. The order of the vector is disturbed.</remarks>
    public static void SwapRemove<T>(this IVector<T> self, int index) {
        ThrowHelper.ArgumentInRange(index, index >= 0 && index < self.Count);
        int last = self.Count - 1;
        self[index] = self[last];
        self.RemoveAt(last); // Should not copy when removing the last element.
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Removes the element at the specified <paramref name="index"/> from the vector by over-writing it with the last element.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <remarks>No block of elements in moved. The order of the vector is disturbed.</remarks>
    public static void SwapRemove<T>(this IVector<T> self, Index index)
    {
        var off = index.GetOffset(self.Count);
        self.SwapRemove(off);
    }
#endif

    /// <inheritdoc cref="List{T}.Sort()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sort<T>(this IVector<T> self) {
        self.Sort(0, self.Count);
    }

    /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sort<T, C>(this IVector<T> self, C comparer)
        where C : IComparer<T> {
        self.Sort(0, self.Count, comparer);
    }

    /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sort<T>(this IVector<T> self, Comparison<T> comparison) {
        self.Sort(new ComparisonCmp<T>(comparison));
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc cref="List{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse<T>(this IVector<T> self, Range range)
    {
        var (off, cnt) = range.GetOffsetAndLength(self.Count);
        self.Reverse(off, cnt);
    }
#endif

    /// <inheritdoc cref="List{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse<T>(this IVector<T> self) {
        self.Reverse(0, self.Count);
    }

    /// <summary>Attempts to remove the topmost element from the stack.</summary>
    /// <param name="self">The stack</param>
    /// <param name="value">The value removed from the stack, or default</param>
    /// <typeparam name="T">The type of elements in the stack</typeparam>
    /// <returns><c>true</c> if a value was removed; otherwise <c>false</c></returns>
    public static bool TryPop<T>(this IVector<T> self, [NotNullWhen(true)] out T? value) {
        if (!self.IsEmpty) {
            int last = self.Count - 1;
            value = self[last]!;
            self.RemoveAt(last);
            return true;
        }

        value = default!;
        return false;
    }

    private struct ComparisonCmp<T> : IComparer<T>, IEqualityComparer<T> {
        public Comparison<T> Comparison { get; }

        public ComparisonCmp(Comparison<T> comparison) {
            Comparison = comparison;
        }

        public int Compare([AllowNull] T x, [AllowNull] T y) {
            return Comparison(x!, y!);
        }

        public bool Equals([AllowNull] T x, [AllowNull] T y) {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj) {
            Debug.Fail("Comparison cannot compute a hashcode. Fallback to obj.GetHashCode()");
            return obj is null ? 0 : obj.GetHashCode();
        }
    }
}
