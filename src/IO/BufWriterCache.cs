using System;

namespace HeaplessUtility.IO
{
    /// <summary>
    ///     Provides a cache for a reusable instance of <see cref="BufWriter{T}"/> per thread.
    /// </summary>
    /// <typeparam name="T">The type of elements of the writer.</typeparam>
    public static class BufWriterCache<T>
    {
        /// <summary>
        ///     The maximum capacity at which a writer can be placed in the cache.
        /// </summary>
        public const int MaxWriterCapacity = 420;

        [ThreadStatic] private static BufWriter<T>? t_cachedInstance;

        /// <summary>
        ///     Gets a <see cref="BufWriter{T}"/> with at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity of the writer.</param>
        /// <returns>Returns a <see cref="BufWriter{T}"/> with at least the specified <paramref name="capacity"/>.</returns>
        /// <remarks>If a <see cref="BufWriter{T}"/> of an appropriate size is cached, it will be returned and the cache emptied.</remarks>
        public static BufWriter<T> Acquire(int capacity = 16)
        {
            BufWriter<T>? writer = t_cachedInstance;
            if (writer != null)
            {
                if (capacity <= writer.Capacity)
                {
                    t_cachedInstance = null;
                    writer.Clear();
                    return writer;
                }
            }

            return new BufWriter<T>(capacity);
        }

        /// <summary>
        ///     Place the specified writer in the cache if it is not too big.
        /// </summary>
        /// <param name="writer">The writer to place in the cache.</param>
        public static void Release(BufWriter<T> writer)
        {
            if (writer.Capacity <= 16)
            {
                t_cachedInstance = writer;
            }
        }

        /// <summary>
        ///     Gets the span of the <see cref="BufWriter{T}"/> and tries to place it in the cache.
        /// </summary>
        /// <param name="writer">The writer to place in the cache.</param>
        /// <param name="array">The internal array.</param>
        /// <returns></returns>
        public static Span<T> GetSpanAndRelease(BufWriter<T> writer, out T[] array)
        {
            Span<T> span = writer.ToSpan(out array);
            Release(writer);
            return span;
        }
    }
}