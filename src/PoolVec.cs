using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HeaplessUtility
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>. 
    ///     The list allocated from a <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items of the list.</typeparam>
    public class PoolVec<T>
        : Vec<T>, IDisposable
    {
        protected ArrayPool<T> Pool;

        /// <summary>
        ///     Initializes a new list.
        /// </summary>
        /// <param name="pool">The pool from which to allocate.</param>
        public PoolVec(ArrayPool<T>? pool = null)
        {
            Pool = pool ?? ArrayPool<T>.Shared;
        }

        /// <summary>
        ///     Initializes a new list with a initial buffer.
        /// </summary>
        /// <param name="initialBuffer">The initial buffer.</param>
        /// <param name="pool">The pool from which to allocate.</param>
        public PoolVec(T[] initialBuffer, ArrayPool<T>? pool = null)
            : base(initialBuffer)
        {
            Pool = pool ?? ArrayPool<T>.Shared;
        }

        /// <summary>
        ///     Initializes a new list with a specified minimum initial capacity.
        /// </summary>
        /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
        /// <param name="pool">The pool from which to allocate.</param>
        public PoolVec(int initialMinimumCapacity, ArrayPool<T>? pool = null)
            : this()
        {
            Pool = pool ?? ArrayPool<T>.Shared;
            Storage = Pool.Rent(initialMinimumCapacity);
        }

        protected override void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (Storage is not null)
            {
                Debug.Assert(Count > Storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

                var temp = Pool.Rent((int)Math.Max((uint)(Count + additionalCapacityBeyondPos), (uint)Storage.Length * 2));
                Array.Copy(Storage, 0, temp, 0, Count);
                Storage = temp;
                if (temp is not null)
                {
                    Pool.Return(temp);
                }
            }
            else
            {
                Storage = Pool.Rent(additionalCapacityBeyondPos);
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            var temp = Storage;
            Storage = null;
            if (temp is not null)
            {
                Pool.Return(temp);
            }
        }
    }
}
