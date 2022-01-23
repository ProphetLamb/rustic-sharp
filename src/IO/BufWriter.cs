using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.DebuggerViews;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility.IO
{
    /// <summary>
    ///     Reusable <see cref="IBufferWriter{T}"/> intended for use as a thread-static singleton.
    /// </summary>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(PoolBufWriterDebuggerView<>))]
    public class BufWriter<T> :
        IBufferWriter<T>,
        IList<T>,
        ICollection,
        IReadOnlyList<T>,
        IDisposable
    {
        /// <summary>
        /// The internal storage.
        /// </summary>
        protected T[]? Buffer;
        private int _index;

        /// <summary>
        ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
        /// </summary>
        public BufWriter()
        {
            Buffer = null;
            _index = 0;
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The minimum capacity of the writer.</param>
        public BufWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessEqualZero(ExceptionArgument.initialCapacity);
            }

            Buffer = new T[initialCapacity];
            _index = 0;
        }

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => Buffer;

        /// <inheritdoc cref="List{T}.Count" />
        public int Count => _index;

        /// <inheritdoc />
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc />
        object ICollection.SyncRoot => null!;

        /// <inheritdoc />
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///     The current capacity of the writer.
        /// </summary>
        public int Capacity => Buffer is null ? 0 : Buffer.Length;

        /// <inheritdoc cref="IList{T}.this"/>
        public ref T this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Buffer == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_ObjectNotInitialized();
                }

                if ((uint)index >= (uint)_index)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument.index, index, _index);
                }

                return ref Buffer![index];
            }
        }

        /// <inheritdoc />
        T IReadOnlyList<T>.this[int index] => this[index];

        /// <inheritdoc />
        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.count);
            }

            if (_index > Capacity - count)
            {
                ThrowInvalidOperationException_AdvancedTooFar(Capacity);
            }

            _index += count;
        }

        /// <inheritdoc />
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            if (_index > Capacity - sizeHint)
            {
                Grow(sizeHint);
            }

            Debug.Assert(Capacity > _index);
            return Buffer.AsMemory(_index);
        }

        /// <inheritdoc />
        public Span<T> GetSpan(int sizeHint = 0)
        {
            if (_index > Capacity - sizeHint)
            {
                Grow(sizeHint);
            }

            Debug.Assert(Capacity > _index);
            return Buffer.AsSpan(_index);
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            if (_index >= Capacity)
            {
                Grow(1);
            }

            Buffer![_index++] = item;
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (Buffer != null)
            {
                Array.Clear(Buffer, 0, _index);
            }

            _index = 0;
        }

        /// <inheritdoc />
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => IndexOf(item) >= 0;

        /// <inheritdoc cref="IList{T}.IndexOf(T)" />
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            if (Buffer != null)
            {
                Array.IndexOf(Buffer, item, 0, _index);
            }

            return -1;
        }

        /// <inheritdoc cref="IList{T}.Insert(int,T)" />
        public void Insert(int index, T item)
        {
            if (_index >= Capacity - 1)
            {
                Grow(1);
            }

            int remaining = _index - index;

            if (remaining != 0)
            {
                Array.Copy(Buffer!, index, Buffer!, index + 1, remaining);
            }
            else
            {
                Buffer![_index] = item;
            }

            _index += 1;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            ThrowHelper.ThrowIfObjectDisposed(Buffer == null);

            int remaining = _index - index - 1;

            if (remaining != 0)
            {
                Array.Copy(Buffer!, index + 1, Buffer!, index, remaining);
            }

            Buffer![--_index] = default!;
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            ThrowHelper.ThrowIfObjectDisposed(_index == -1);

            if (arrayIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.arrayIndex);
            }

            if (array.Length - arrayIndex < Count)
            {
                ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.array);
            }

            if (Buffer != null)
            {
                Array.Copy(Buffer!, 0, array, arrayIndex, _index);
            }
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        /// <summary>
        /// Returns the <see cref="Span{T}"/> representing the written / requested to portion of the buffer.
        /// </summary>
        /// <param name="array">The internal array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> ToSpan(out T[] array)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Buffer == null);

            array = Buffer!;
            Buffer = null; // Ensure that reset doesn't return the buffer.

            Reset();

            return new Span<T>(array, 0, _index);
        }

        /// <summary>
        /// Returns the <see cref="Memory{T}"/> representing the written / requested to portion of the buffer.
        /// </summary>
        /// <param name="array">The internal array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> ToMemory(out T[] array)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Buffer == null);

            array = Buffer!;
            Buffer = null; // Ensure that reset doesn't return the buffer.

            Reset();

            return new Memory<T>(array, 0, _index);
        }

        /// <summary>
        /// Returns a array containing a shallow-copy of the written / requested portion of the buffer.
        /// </summary>
        /// <returns>A array containing a shallow-copy of the written / requested portion of the buffer.</returns>
        /// <remarks>
        ///     Resets the object.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray() => ToArray(false);

        /// <summary>
        /// Returns a array containing a shallow-copy of the written / requested portion of the buffer.
        /// </summary>
        /// <param name="dispose">Whether to dispose the object, or reset.</param>
        /// <returns>A array containing a shallow-copy of the written / requested portion of the buffer.</returns>
        /// <remarks>
        ///     Resets or disposes the object.
        /// </remarks>
        public T[] ToArray(bool dispose)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Buffer == null);

            T[] array = new T[_index];
            Buffer.AsSpan(0, _index).CopyTo(array);

            if (dispose)
            {
                Dispose();
            }
            else
            {
                Reset();
            }

            return array;
        }

        /// <summary>Resets the writer to the initial state and returns the buffer to the array-pool.</summary>
        public virtual void Reset()
        {
            ThrowHelper.ThrowIfObjectDisposed(_index == -1);

            _index = 0;
            Buffer = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Reset();
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        [Pure]
        public VecIter<T> GetEnumerator() => new(Buffer, 0, _index);

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (Buffer != null)
            {
                Debug.Assert(_index > Buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");
                T[] temp = new T[Math.Max(_index + additionalCapacityBeyondPos, Buffer.Length * 2)];
                Buffer.AsSpan(0, _index).CopyTo(temp);
                Buffer = temp;
            }
            else
            {
                ThrowHelper.ThrowIfObjectDisposed(_index == -1);
                Buffer = new T[Math.Max(additionalCapacityBeyondPos, 16)];
            }
        }

        [DoesNotReturn]
        private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
        {
            throw new InvalidOperationException($"Cannot advance the buffer because the index would exceed the maximum capacity ({capacity}) of the buffer.");
        }
    }
}