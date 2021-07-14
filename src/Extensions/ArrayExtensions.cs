using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace HeaplessUtility
{
    /// <summary>
    ///     Extension methods for <see cref="ArraySegment{T}"/> and <see cref="Array"/>.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Initializes a new <see cref="ArraySegmentIterator{T}"/> for the segment.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegmentIterator<T> GetIterator<T>(this ArraySegment<T> segment) => new(segment.Array, segment.Offset, segment.Count);
        
        /// <summary>
        ///     Initializes a new <see cref="ArraySegmentIterator{T}"/> for the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegmentIterator<T> GetIterator<T>(this T[] array) => new(array, 0, array.Length);
        
        /// <summary>
        ///     Initializes a new <see cref="ArraySegmentIterator{T}"/> for the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The zero-based index of the first element in the array.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// 
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegmentIterator<T> GetIterator<T>(this T[] array, int offset) => new(array, offset, array.Length - offset);
        
        /// <summary>
        ///     Initializes a new <see cref="ArraySegmentIterator{T}"/> for the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The zero-based index of the first element in the array.</param>
        /// <param name="count">The number of elements form the <paramref name="offset"/>.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegmentIterator<T> GetIterator<T>(this T[] array, int offset, int count) => new(array, offset, count);
    }
}