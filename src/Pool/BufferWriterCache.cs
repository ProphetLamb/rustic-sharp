using System;

namespace HeaplessUtility.Pool
{
    /// <summary>
    ///     Provides a cache for a reusable instance of <see cref="BufferWriter{T}"/> per thread.
    /// </summary>
    /// <typeparam name="T">The type of elements of the writer.</typeparam>
    public static class BufferWriterCache<T>
    {
        /// <summary>
        ///     The maximum capacity at which a writer can be placed in the cache.
        /// </summary>
        public const int MaxWriterCapacity = 420;

        [ThreadStatic] private static BufferWriter<T>? s_cachedInstance;
        
        /// <summary>
        ///     Gets a <see cref="BufferWriter{T}"/> with at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity of the writer.</param>
        /// <returns>Returns a <see cref="BufferWriter{T}"/> with at least the specified <paramref name="capacity"/>.</returns>
        /// <remarks>If a <see cref="BufferWriter{T}"/> of an appropriate size is cached, it will be returned and the cache emptied.</remarks>
        public static BufferWriter<T> Acquire(int capacity = 16)
        {
            BufferWriter<T>? writer = s_cachedInstance;
            if (writer != null)
            {
                if (capacity <= writer.Capacity)
                {
                    s_cachedInstance = null;
                    writer.Clear();
                    return writer;
                }
            }

            return new BufferWriter<T>(capacity);
        }
        
        /// <summary>
        ///     Place the specified writer in the cache if it is not too big.
        /// </summary>
        /// <param name="writer">The writer to place in the cache.</param>
        public static void Release(BufferWriter<T> writer)
        {
            if (writer.Capacity <= 16)
            {
                s_cachedInstance = writer;
            }
        }
        
        /// <summary>
        ///     Gets the span of the <see cref="BufferWriter{T}"/> and tries to place it in the cache.
        /// </summary>
        /// <param name="writer">The writer to place in the cache.</param>
        /// <param name="leased">The reference to the pool-array, to be returned to the pool when no longer needed.</param>
        /// <returns></returns>
        public static Span<T> GetSpanAndRelease(BufferWriter<T> writer, out T[]? leased)
        {
            Span<T> span = writer.ToSpan(out leased);
            Release(writer);
            return span;
        }
    }
}