using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HeaplessUtility.DebuggerViews;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility
{
    /// <summary>
    ///     Reuseable <see cref="IBufferWriter{T}"/> consuming from the <see cref="ArrayPool{T}.Shared"/> intended for use as a thread-static singleton.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <remarks>
    ///     Usage:
    /// <code>
    ///     var obj = [...]
    /// <br/>
    ///     PoolBufferWriter&lt;byte&gt; writer = new();
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
    ///     PoolBufferWriter&lt;byte&gt; writer = new();
    /// <br/>
    ///     Serializer.Serialize(writer, obj);
    /// <br/>
    ///     return writer.ToArray(true);
    /// </code>
    /// </remarks>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(PoolBufferWriterDebuggerView<>))]
    internal sealed class PoolBufferWriter<T> :
        IBufferWriter<T>,
        IList<T>,
        ICollection,
        IReadOnlyList<T>,
        IDisposable
    {
        private T[]? _buffer;
        private readonly int _minimumCapacity;
        private int _index;

        public PoolBufferWriter()
        {
            _minimumCapacity = 16;
        }

        public PoolBufferWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessEqualZero(ExceptionArgument.initialCapacity);
            }

            _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
            _minimumCapacity = initialCapacity;
            _index = 0;
        }

        internal T[]? RawStorage => _buffer; 

        /// <inheritdoc cref="IReadOnlyList{T}.Count" />
        public int Count => _index;

        /// <inheritdoc />
        public bool IsSynchronized => false;

        /// <inheritdoc />
        public object SyncRoot
        {
            get
            {
                ThrowHelper.ThrowNotSupportedException();
                return default!;
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer != null ? _buffer.Length : -1;
        }

        /// <inheritdoc cref="IList{T}.this"/>
        public ref T this[int index]
        {
            get
            {
                if (_buffer == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_ObjectNotInitialized();
                }

                if ((uint)index >= (uint)_index)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument.index, index, _index);
                }

                return ref _buffer[index];
            }
        }

        /// <inheritdoc />
        T IReadOnlyList<T>.this[int index] => this[index];

        /// <inheritdoc />
        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        /// <inheritdoc />
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
            return _buffer.AsMemory(_index);
        }

        /// <inheritdoc />
        public Span<T> GetSpan(int sizeHint = 0)
        {
            if (_index > Capacity - sizeHint)
            {
                Grow(sizeHint);
            }

            Debug.Assert(Capacity > _index);
            return _buffer.AsSpan(_index);
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            if (_index >= Capacity - 1)
            {
                Grow(1);
            }

            _buffer![_index++] = item;
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (_buffer != null)
            {
                Array.Clear(_buffer, 0, _index);
            }

            _index = 0;
        }

        /// <inheritdoc />
        public bool Contains(T item) => IndexOf(item) >= 0;

        /// <inheritdoc cref="IList{T}.IndexOf(T)" />
        public int IndexOf(T item)
        {
            if (_buffer != null)
            {
                Array.IndexOf(_buffer, item, 0, _index);
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (_index >= Capacity - 1)
            {
                Grow(1);
            }

            int remaining = _index - index;
            
            if (remaining != 0)
            {
                Array.Copy(_buffer!, index, _buffer!, index + 1, remaining);
            }
            else
            {
                _buffer![_index] = item;
            }

            _index += 1;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            ThrowHelper.ThrowIfObjectDisposed(_buffer == null);

            int remaining = _index - index - 1;
            
            if (remaining != 0)
            {
                Array.Copy(_buffer, index + 1, _buffer, index, remaining);
            }
            
            _buffer![--_index] = default!;
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

            if (_buffer != null)
            {
                Array.Copy(_buffer!, 0, array, arrayIndex, _index);
            }
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[]) array, index);

        /// <summary>
        /// Returns the <see cref="Span{T}"/> representing the written / requested to portion of the buffer.
        /// </summary>
        /// <param name="leased">
        ///     The reference to the pool-array, to be returned to the pool when no longer needed.
        /// <br/>
        /// <code>
        ///     ArrayPool&lt;T&gt;.Shared.Return(leased);
        /// </code>
        /// </param>
        /// <returns>The <see cref="Span{T}"/> representing the written / requested to portion of the buffer.</returns>
        /// <remarks>
        ///     Resets the object, but does not return the pool-array.
        /// </remarks>
        public Span<T> ToSpan(out T[] leased)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_buffer == null);

            leased = _buffer;
            _buffer = null; // Ensure that reset doesn't return the buffer.

            Reset();

            return new Span<T>(leased, 0, _index);
        }

        /// <summary>
        /// Returns the <see cref="Memory{T}"/> representing the written / requested to portion of the buffer.
        /// </summary>
        /// <param name="leased">
        ///     The reference to the pool-array, to be returned to the pool when no longer needed.
        /// <br/>
        /// <code>
        ///     ArrayPool&lt;T&gt;.Shared.Return(leased);
        /// </code>
        /// </param>
        /// <returns>The <see cref="Memory{T}"/> representing the written / requested to portion of the buffer.</returns>
        /// <remarks>
        ///     Resets the object, but does not return the pool-array.
        /// </remarks>
        public Memory<T> ToMemory(out T[] leased)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_buffer == null);

            leased = _buffer;
            _buffer = null; // Ensure that reset doesn't return the buffer.

            Reset();

            return new Memory<T>(leased, 0, _index);
        }

        /// <summary>
        /// Returns a array containing a shallow-copy of the written / requested portion of the buffer.
        /// </summary>
        /// <returns>A array containing a shallow-copy of the written / requested portion of the buffer.</returns>
        /// <remarks>
        ///     Resets the object.
        /// </remarks>
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
            ThrowHelper.ThrowIfObjectNotInitialized(_buffer == null);
            
            T[] array = new T[_index];
            _buffer.AsSpan(0, _index).CopyTo(array);

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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Reset()
        {
            ThrowHelper.ThrowIfObjectDisposed(_index == -1);

            T[]? poolArray = _buffer;
            _index = 0;
            _buffer = null;

            if (poolArray != null)
            {
                ArrayPool<T>.Shared.Return(poolArray);
            }
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Dispose()
        {
            _index = -1;

            if (_buffer != null)
            {
                ArrayPool<T>.Shared.Return(_buffer);
                _buffer = null;
            }
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public ArraySegmentIterator<T> GetEnumerator() => new(_buffer, 0, _index);

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (_buffer != null)
            {
                Debug.Assert(_index > _buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");
                T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(_index + additionalCapacityBeyondPos, _buffer.Length * 2));
                _buffer.AsSpan(0, _index).CopyTo(poolArray);

                T[] toReturn = _buffer;
                _buffer = poolArray;
                ArrayPool<T>.Shared.Return(toReturn);
            }
            else
            {
                ThrowHelper.ThrowIfObjectDisposed(_index == -1);
                _buffer = ArrayPool<T>.Shared.Rent(Math.Max(additionalCapacityBeyondPos, _minimumCapacity));
            }
        }
        
        [DoesNotReturn]
        private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
        {
            throw new InvalidOperationException($"Cannot advance the buffer because the index would exceed the maximum capacity ({capacity}) of the buffer.");
        }
    }
}