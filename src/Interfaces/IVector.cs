using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace HeaplessUtility.Interfaces
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
        ReadOnlySpan<T> AsSpan();

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        int IndexOf(in T item);

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        int LastIndexOf(in T item);

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        [Pure]
        int BinarySearch(in T item);

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        int BinarySearch(in T item, IComparer<T> comparer);
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

        /// <inheritdoc cref="List{T}.AddRange"/>
        void AddRange(ReadOnlySpan<T> values);

        /// <inheritdoc cref="List{T}.InsertRange"/>
        void InsertRange(int index, ReadOnlySpan<T> values);

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        void RemoveRange(int index, int count);
    }
}
