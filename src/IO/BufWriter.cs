using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Common;

namespace HeaplessUtility.IO
{
    /// <summary>
    ///     Reusable <see cref="IBufferWriter{T}"/> intended for use as a thread-static singleton.
    /// </summary>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(PoolBufWriterDebuggerView<>))]
    public class BufWriter<T> :
        IBufferWriter<T>,
        IVector<T>,
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
            initialCapacity.ValidateArg(initialCapacity >= 0);
            if (initialCapacity != 0)
            {
                Buffer = new T[initialCapacity];
            }
            _index = 0;
        }

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => Buffer;

        /// <inheritdoc cref="List{T}.Count" />
        public int Length
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _index;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value < Capacity);
                _index = value;
            }
        }

        /// <inheritdoc />
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc />
        object ICollection.SyncRoot => null!;

        /// <inheritdoc />
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc />
        public int Capacity => Buffer is null ? 0 : Buffer.Length;

        /// <inheritdoc />
        public bool IsEmpty => 0u >= (uint)Length;

        /// <inheritdoc />
        public bool IsDefault => Buffer is null;

        /// <inheritdoc />
        public int Count => Length;


        /// <inheritdoc />
        public ref T this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                index.ValidateArg(index >= 0 && index < Length);
                this.ValidateArg(Buffer is not null);
                return ref Buffer[index];
            }
        }

        T IReadOnlyList<T>.this[int index] => this[index];

        ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

        /// <inheritdoc />
        public ref T this[Index index] => ref this[index.GetOffset(Length)];

        ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index];

        /// <inheritdoc />

        /// <inheritdoc />
        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            count.ValidateArg(count >= 0);
            count.ValidateArg(Length <= Capacity - count);
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
            if (Buffer is not null)
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
            this.ValidateArg(Buffer is not null);

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
            arrayIndex.ValidateArg(arrayIndex >= 0);
            array.ValidateArg(array.Length - arrayIndex >= Length);
            this.ValidateArg(Length != -1);

            if (Buffer is not null)
            {
                Array.Copy(Buffer!, 0, array, arrayIndex, _index);
            }
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        /// <summary>
        /// Returns the <see cref="Span{T}"/> representing the written / requested portion of the buffer.
        /// <see cref="Reset"/>s the buffer.
        /// </summary>
        /// <param name="array">The internal array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> ToSpan(out T[] array)
        {
            this.ValidateArg(Buffer is not null);

            array = Buffer;
            Span<T> span = new(array, 0, _index);

            Reset();

            return span;
        }

        /// <summary>
        /// Returns the <see cref="Memory{T}"/> representing the written / requested portion of the buffer.
        /// <see cref="Reset"/>s the buffer.
        /// </summary>
        /// <param name="array">The internal array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> ToMemory(out T[] array)
        {
            this.ValidateArg(Buffer is not null);

            array = Buffer;
            Memory<T> mem = new(array, 0, _index);

            Reset();

            return mem;
        }

        /// <summary>
        /// Returns the <see cref="ArraySegment{T}"/> representing the written / requested portion of the buffer.
        /// <see cref="Reset"/>s the buffer.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<T> ToSegment()
        {
            this.ValidateArg(Buffer is not null);

            ArraySegment<T> segment = new(Buffer!, 0, _index);

            Reset();

            return segment;
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
            this.ValidateArg(Buffer is not null);

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
            this.ValidateArg(Buffer is not null);

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
                this.ValidateArg(Length != -1);
                Buffer = new T[Math.Max(additionalCapacityBeyondPos, 16)];
            }
        }

        /// <inheritdoc />
        public int Reserve(int additionalCapacity)
        {
            if (Length >= Capacity - additionalCapacity)
            {
                Grow(additionalCapacity);
            }
            return Capacity;
        }

        /// <inheritdoc />
        public void InsertRange(int index, ReadOnlySpan<T> values)
        {
            index.ValidateArg(index >= 0);
            index.ValidateArg(index <= Length);

            int count = values.Length;
            if (count == 0)
            {
                return;
            }

            if (Buffer is null || Length > Capacity - count)
            {
                Grow(count);
            }

            T[] storage = Buffer!;
            Array.Copy(storage, index, storage, index + count, Length - index);
            values.CopyTo(storage.AsSpan(index));
            Length += count;
        }

        /// <inheritdoc />
        public void RemoveRange(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Count - count);
            this.ValidateArg(Buffer is not null);

            int end = Length - count;
            int remaining = end - start;
            Array.Copy(Buffer, start + count, Buffer, start, remaining);
            Array.Clear(Buffer, end, count);
            Length = end;
        }

        /// <inheritdoc />
        public void Sort(int start, int count, IComparer<T>? comparer = null)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            if (count != 0)
            {
                this.ValidateArg(Buffer is not null);
                if (comparer is null)
                {
                    Buffer.AsSpan(start, count).Sort();
                }
                else
                {
                    Buffer.AsSpan(start, count).Sort(comparer);
                }
            }
        }

        /// <inheritdoc />
        public void Reverse(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            if (count != 0)
            {
                this.ValidateArg(Buffer is not null);
                Array.Reverse(Buffer, start, count);
            }
        }

        /// <inheritdoc />
        public ReadOnlySpan<T> AsSpan(int start, int count)
        {
            return new(Buffer, start, count);
        }

        /// <inheritdoc />
        public int IndexOf(int start, int count, in T item, IEqualityComparer<T>? comparer = null)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);

            if (Buffer is null)
            {
                return -1;
            }

            int end = start + count;
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    for (int i = start; i < end; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, Buffer[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = start; i < end; i++)
            {
                if (!comparer.Equals(item, Buffer[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc />
        public int LastIndexOf(int start, int count, in T item, IEqualityComparer<T>? comparer = null)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start < Length - count);

            if (Buffer == null)
            {
                return -1;
            }

            int end = start + count;
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    for (int i = end - 1; i >= start; i--)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, Buffer[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = end - 1; i >= start; i--)
            {
                if (!comparer.Equals(item, Buffer[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc />
        public int BinarySearch(int start, int count, in T item, IComparer<T> comparer)
        {
            if (Buffer == null)
            {
                return -1;
            }
            return Array.BinarySearch(Buffer, start, count, item, comparer);
        }

        /// <inheritdoc />
        public bool TryCopyTo(Span<T> destination)
        {
            if (IsEmpty)
            {
                return true;
            }

            return Buffer.AsSpan().TryCopyTo(destination);
        }
    }
}