using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Common;

namespace HeaplessUtility
{
    /// <summary>
    ///     Enumerates the elements of a segment of an array.
    /// </summary>
    /// <typeparam name="T">The type of items of the array.</typeparam>
    public struct VecIter<T>
        : IEnumerator<T>, IReadOnlyList<T>
    {
        private readonly T[]? _array;
        private readonly int _offset;
        private readonly int _count;
        private int _pos;

        /// <summary>
        ///     Initializes a new instance of the iterator.
        /// </summary>
        /// <param name="array">The array to iterate.</param>
        /// <param name="offset">The index of the first element to iterate.</param>
        /// <param name="count">The number of elements to iterate.</param>
        public VecIter(T[]? array, int offset, int count)
        {
            offset.ValidateArg(offset >= 0);
            count.ValidateArg(count >= 0);
            int arrayLength = array is null ? 0 : array.Length;
            count.ValidateArg(count <= arrayLength - offset);

            _array = array;
            _offset = offset;
            _count = count;
            _pos = -1;
        }

        /// <inheritdoc cref="ArraySegment{T}.Array"/>
        public T[]? Array => _array;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public ref readonly T Current => ref _array![_offset + _pos];

        /// <inheritdoc />
        T IEnumerator<T>.Current => Current;

        /// <inheritdoc />
        object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        public int Length => _count;

        int IReadOnlyCollection<T>.Count => Length;

        /// <summary>
        ///     The current position of the enumerator.
        /// </summary>
        public int Index => _pos;

        /// <summary>
        ///     Returns a value that indicates whether the current segment is empty.
        /// </summary>
        public bool IsEmpty => 0 >= (uint)_count;

        /// <inheritdoc cref="ArraySegment{T}.Offset"/>
        public int Offset => _offset;

        /// <inheritdoc cref="IReadOnlyList{T}.this" />
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                index.ValidateArg(index >= 0 && index < Length);

                return ref _array![_offset + index];
            }
        }

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                index.ValidateArg(index >= 0 && index < Length);

                return _array![_offset + index];
            }
        }


        /// <summary>
        ///     Returns the segment represented by the <see cref="VecIter{T}"/> as a span.
        /// </summary>
        /// <returns>The span representing the segment.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan() => new(_array, _offset, _count);

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VecIter<T> GetEnumerator()
        {
            if (_pos == -1)
            {
                return this;
            }

            return new VecIter<T>(_array, _offset, _count);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this = default;
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public bool MoveNext()
        {
            int index = _pos + 1;

            if ((uint)index < (uint)_count)
            {
                _pos = index;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _pos = -1;
        }

        /// <summary>
        ///     Instantiates a new <see cref="ArraySegment{T}"/> representing the same segment as the iterator. 
        /// </summary>
        /// <param name="segment">The iterator.</param>
        /// <returns></returns>
        public static implicit operator ArraySegment<T>(VecIter<T> segment) => segment.Array != null ? new(segment.Array, segment.Offset, segment.Length) : default;
    }
}