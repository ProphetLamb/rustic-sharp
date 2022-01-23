using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using HeaplessUtility.Exceptions;

namespace HeaplessUtility.IO
{
    /// <summary>
    ///     Reusable <see cref="IBufferWriter{T}"/> intended for use as a thread-static singleton.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <remarks>
    ///     Usage:
    /// <code>
    ///     var obj = [...]
    /// <br/>
    ///     BufWriter&lt;byte&gt; writer = new();
    /// <br/>
    ///     Serializer.Serialize(writer, obj);
    /// <br/>
    ///     DoWork(writer.ToSpan(out byte[] poolArray));
    /// <br/>
    ///     ArrayPool&lt;byte&gt;.Return(poolArray);
    /// </code>
    ///  - or -
    /// <code>
    ///     var obj = [...]
    /// <br/>
    ///     BufWriter&lt;byte&gt; writer = new();
    /// <br/>
    ///     Serializer.Serialize(writer, obj);
    /// <br/>
    ///     return writer.ToArray(true);
    /// </code>
    /// </remarks>
    public class PoolBufWriter<T> : BufWriter<T>
    {
        private readonly ArrayPool<T> _pool;

        /// <summary>
        ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
        /// </summary>
        /// <param name="pool">The array pool from which to rent.</param>
        public PoolBufWriter(ArrayPool<T>? pool = null)
            : base()
        {
            _pool = pool ?? ArrayPool<T>.Shared;
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The minimum capacity of the writer.</param>
        /// <param name="pool">The array pool from which to rent.</param>
        public PoolBufWriter(int initialCapacity, ArrayPool<T>? pool = null)
        {
            _pool = pool ?? ArrayPool<T>.Shared;
            Buffer = _pool.Rent(initialCapacity);
        }

        protected override void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (Buffer != null)
            {
                Debug.Assert(Count > Buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");
                T[] temp = _pool.Rent(Math.Max(Count + additionalCapacityBeyondPos, Buffer.Length * 2));
                Buffer.AsSpan(0, Count).CopyTo(temp);
                Buffer = temp;
            }
            else
            {
                ThrowHelper.ThrowIfObjectDisposed(Count == -1);
                Buffer = _pool.Rent(additionalCapacityBeyondPos);
            }
        }

        public override void Reset()
        {
            var poolArray = Buffer;
            base.Reset();
            if (poolArray is not null)
            {
                _pool.Return(poolArray);
            }
        }
    }
}
