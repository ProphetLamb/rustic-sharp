using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility
{
    /// <summary>
    ///     Enumerates the elements of a segment of an array.
    /// </summary>
    /// <typeparam name="T">The type of items of the array.</typeparam>
    public struct ArraySegmentIterator<T> : 
        IEnumerator<T>,
        IReadOnlyList<T>
    {
        private readonly T[]? _array;
        private readonly int _offset;
        private readonly int _count;
        private int _index;

        /// <summary>
        ///     Initializes a new instance of the iterator.
        /// </summary>
        /// <param name="array">The array to iterate.</param>
        /// <param name="offset">The index of the first element to iterate.</param>
        /// <param name="count">The number of elements to iterate.</param>
        public ArraySegmentIterator(T[]? array, int offset, int count)
        {
            if (offset < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.startIndex);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.count);
            }

            if (array != null)
            {
                if (array.Length - offset > count)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.array);
                }
            }
            else
            {
                if (offset != 0 || count != 0)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.array);
                }
            }

            _array = array;
            _offset = offset;
            _count = count;
            _index = -1;
        }

        public T[]? Array => _array;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public ref readonly T CurrentRef => ref _array![_offset + _index];

        /// <inheritdoc />
        public T Current => _array![_offset + _index];

        /// <inheritdoc />
        object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        public int Count => _count;
        
        /// <summary>
        ///     The current position of the enumerator.
        /// </summary>
        public int Index => _index;
        
        /// <summary>
        ///     Returns a value that indicates whether the current segment is empty.
        /// </summary>
        public bool IsEmpty => 0 >= (uint)_count;

        public int Offset => _offset;

        /// <inheritdoc cref="IReadOnlyList{T}.this" />
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint) index >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.index, index);
                }

                return ref _array![_offset + index];
            }
        }
    
        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint) index >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.index, index);
                }

                return _array![_offset + index];
            }
        }
    
        [Pure]
        public ReadOnlySpan<T> AsSpan() => new(_array, _offset, _count);

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        [Pure]
        public ArraySegmentIterator<T> GetEnumerator()
        {
            if (_index == -1)
            {
                return this;
            }

            return new ArraySegmentIterator<T>(_array, _offset, _count);
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
            int index = _index + 1;
            
            if ((uint)index < (uint)_count)
            {
                _index = index;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _index = -1;
        }

        public static explicit operator ArraySegment<T>(ArraySegmentIterator<T> segment) => new(segment.Array!, segment.Offset, segment.Count);
    }
}