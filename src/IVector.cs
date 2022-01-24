using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Common;

namespace HeaplessUtility
{
    /// <summary>
    /// Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items of the vector.</typeparam>
    public interface IReadOnlyVector<T>
        : IReadOnlyList<T>
    {
        /// <summary>
        /// Returns a value that indicates whether the vector is empty.
        /// </summary>
        [Pure]
        bool IsEmpty { get; }

        /// <summary>
        /// Returns a value that indicates whether the vector is at its default value, 
        /// no memory is allocated.
        /// </summary>
        [Pure]
        bool IsDefault { get; }

        /// <summary>
        /// Returns the number of elements in the vector.
        /// </summary>
        int Length { get; }

        /// <inheritdoc cref="List{T}.Capacity"/>
        int Capacity { get; }

        /// <summary>
        ///     Gets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        [Pure]
        new ref readonly T this[int index] { get; }

        /// <summary>
        ///     Gets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        [Pure]
        ref readonly T this[Index index] { get; }

        /// <summary>
        ///     Creates a new span over a target vector.
        /// </summary>
        /// <returns>The span representation of the vector.</returns>
        [Pure]
        ReadOnlySpan<T> AsSpan(int start, int length);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        int IndexOf(int start, int count, in T item, IEqualityComparer<T>? comparer = null);

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        int LastIndexOf(int start, int count, in T item, IEqualityComparer<T>? comparer = null);

        /// <inheritdoc cref="List{T}.BinarySearch(int, int, T, IComparer{T})"/>
        [Pure]
        int BinarySearch(int start, int count, in T item, IComparer<T> comparer);

        /// <inheritdoc cref="Span{T}.TryCopyTo"/>
        bool TryCopyTo(Span<T> destination);
    }

    /// <summary>
    /// Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items of the vector.</typeparam>
    public interface IVector<T>
        : IReadOnlyVector<T>, IList<T>, ICollection
    {
        /// <summary>
        ///     Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        new ref T this[int index] { get; }

        /// <summary>
        ///     Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        new ref T this[Index index] { get; }

        /// <summary>
        /// Ensures that the collection can contain at least <paramref name="additionalCapacity"/> more elements.
        /// </summary>
        /// <param name="additionalCapacity">The number of additional elements the collection must be able to contain.</param>
        /// <returns>The new capacity of the collection.</returns>
        int Reserve(int additionalCapacity);

        /// <inheritdoc cref="List{T}.InsertRange"/>
        void InsertRange(int index, ReadOnlySpan<T> values);

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        void RemoveRange(int start, int count);

        /// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})"/>
        void Sort(int start, int count, IComparer<T>? comparer = null);

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        void Reverse(int start, int count);
    }

    public static class ReadOnlyVectorTraits
    {
        /// <summary>
        ///     Creates a new span over a target vector.
        /// </summary>
        /// <returns>The span representation of the vector.</returns>
        [Pure]
        public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self)
        {
            return self.AsSpan(0, self.Length);
        }

        /// <summary>
        ///     Creates a new span over a target vector.
        /// </summary>
        /// <returns>The span representation of the vector.</returns>
        [Pure]
        public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self, Index index)
        {
            var len = self.Length;
            var off = index.GetOffset(len);
            return self.AsSpan(off, len - off);
        }

        /// <summary>
        ///     Creates a new span over a target vector.
        /// </summary>
        /// <returns>The span representation of the vector.</returns>
        [Pure]
        public static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyVector<T> self, Range range)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            return self.AsSpan(off, cnt);
        }

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this IReadOnlyVector<T> self, Range range, in T item)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            return self.IndexOf(off, cnt, item);
        }

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this IReadOnlyVector<T> self, in T item, IEqualityComparer<T>? comparer = null)
        {
            return self.IndexOf(0, self.Length, item, comparer);
        }

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf<T>(this IReadOnlyVector<T> self, Range range, in T item, IEqualityComparer<T>? comparer = null)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            return self.LastIndexOf(off, cnt, item, comparer);
        }

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf<T>(this IReadOnlyVector<T> self, in T item, IEqualityComparer<T>? comparer = null)
        {
            return self.LastIndexOf(0, self.Length, item, comparer);
        }


        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(this IReadOnlyVector<T> self, Range range, in T item, IComparer<T> comparer)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            return self.BinarySearch(off, cnt, item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(this IReadOnlyVector<T> self, Index start, in T item, IComparer<T> comparer)
        {
            var len = self.Length;
            var off = start.GetOffset(len);
            return self.BinarySearch(off, len - off, item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(this IReadOnlyVector<T> self, int start, in T item, IComparer<T> comparer)
        {
            return self.BinarySearch(start, self.Length - start, item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(this IReadOnlyVector<T> self, in T item, IComparer<T> comparer)
        {
            return self.BinarySearch(0, self.Length, item, comparer);
        }

        /// <inheritdoc cref="Span{T}.CopyTo"/>
        [Pure]
        public static void CopyTo<T>(this IReadOnlyVector<T> self, Span<T> destination)
        {
            if (!self.TryCopyTo(destination))
            {
                ThrowHelper.ThrowArgumentException("Unable to copy the vector to the destination", nameof(destination));
            }
        }
    }

    public static class VectorTraits
    {

        /// <inheritdoc cref="List{T}.AddRange"/>
        public static void AddRange<T>(this IVector<T> self, ReadOnlySpan<T> values)
        {
            self.InsertRange(self.Length, values);
        }

        /// <inheritdoc cref="List{T}.InsertRange"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InsertRange<T>(this IVector<T> self, Index index, ReadOnlySpan<T> values)
        {
            int off = index.GetOffset(self.Length);
            self.InsertRange(off, values);
        }

        /// <inheritdoc cref="List{T}.Remove"/>
        public static bool Remove<T>(this IVector<T> self, in T item, IEqualityComparer<T>? comparer)
        {
            int index = self.IndexOf(item, comparer);
            if (index >= 0)
            {
                self.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveRange<T>(this IVector<T> self, Range range)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            self.RemoveRange(off, cnt);
        }

        /// <inheritdoc cref="List{T}.RemoveAt"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAt<T>(this IVector<T> self, Index index)
        {
            var off = index.GetOffset(self.Length);
            self.RemoveAt(off);
        }

        /// <summary>
        /// Removes the element at the specified <paramref name="index"/> from the vector by over-writing it with the last element.
        /// </summary>
        /// <remarks>
        /// No block of elements in moved.
        /// The order of the vector is disturbed.
        /// </remarks>
        public static void SwapRemove<T>(this IVector<T> self, int index)
        {
            index.ValidateArg(index >= 0 && index < self.Length);
            int last = self.Length - 1;
            self[index] = self[last];
            self.RemoveAt(last); // Should not copy when removing the last element.
        }

        /// <inheritdoc cref="List{T}.Sort"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this IVector<T> self)
        {
            if (!self.IsEmpty)
            {
                self.Sort(0, self.Length, null);
            }
        }

        /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this IVector<T> self, IComparer<T> comparer)
        {
            if (!self.IsEmpty)
            {
                self.Sort(0, self.Length, comparer);
            }
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this IVector<T> self, Comparison<T> comparison)
        {
            if (!self.IsEmpty)
            {
                self.Sort(new ComparisonCmp<T>(comparison));
            }
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this IVector<T> self, Range range)
        {
            var (off, cnt) = range.GetOffsetAndLength(self.Length);
            self.Reverse(off, cnt);
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this IVector<T> self)
        {
            self.Reverse(0, self.Length);
        }

        private sealed class ComparisonCmp<T> : IComparer<T>, IEqualityComparer<T>
        {
            public Comparison<T> Comparison { get; }

            public ComparisonCmp(Comparison<T> comparison)
            {
                Comparison = comparison;
            }

            public int Compare([AllowNull] T x, [AllowNull] T y)
            {
                return Comparison(x!, y!);
            }

            public bool Equals([AllowNull] T x, [AllowNull] T y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(T obj)
            {
                Debug.Assert(false, "Comparison cannot compute a hashcode.");
                return obj.GetHashCode();
            }
        }
    }
}
