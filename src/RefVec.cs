using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HeaplessUtility.Common;

namespace HeaplessUtility
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>. 
    /// </summary>
    /// <typeparam name="T">The type of items of the list.</typeparam>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public ref struct RefVec<T>
    {
        private T[]? _arrayToReturnToPool;
        private Span<T> _storage;
        private int _pos;

        /// <summary>
        ///     Initializes a new list with a initial buffer.
        /// </summary>
        /// <param name="initialBuffer">The initial buffer.</param>
        public RefVec(Span<T> initialBuffer)
        {
            _arrayToReturnToPool = null;
            _storage = initialBuffer;
            _pos = 0;
        }

        /// <summary>
        ///     Initializes a new list with a specified minimum initial capacity.
        /// </summary>
        /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
        public RefVec(int initialMinimumCapacity)
        {
            _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialMinimumCapacity);
            _storage = _arrayToReturnToPool;
            _pos = 0;
        }

        /// <inheritdoc cref="List{T}.Capacity"/>
        public int Capacity => _storage.Length;

        /// <inheritdoc cref="List{T}.Count"/>
        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _storage.Length);
                _pos = value;
            }
        }

        /// <inheritdoc cref="List{T}.Count"/>
        public int Count => _pos;

        /// <inheritdoc cref="Span{T}.IsEmpty"/>
        public bool IsEmpty => 0 >= (uint)_pos;

        /// <inheritdoc cref="List{T}.this"/>
        public ref T this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(index < _pos);
                return ref _storage[index];
            }
        }

        /// <summary>
        ///     Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        public ref T this[Index index]
        {
            [Pure]
            get
            {
                int offset = index.GetOffset(_pos);
                index.ValidateArg(offset >= 0 && offset < Length);
                return ref _storage[offset];
            }
        }

        /// <summary>
        ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The range of elements to get or set.</param>
        public ReadOnlySpan<T> this[Range range]
        {
            [Pure]
            get
            {
                (int start, int length) = range.GetOffsetAndLength(_pos);
                range.ValidateArgRange(start >= 0);
                range.ValidateArgRange(length >= 0);
                range.ValidateArgRange(start <= Length - length);
                return _storage.Slice(start, length);
            }
            set
            {
                (int start, int length) = range.GetOffsetAndLength(_pos);
                range.ValidateArgRange(start >= 0);
                range.ValidateArgRange(length >= 0);
                range.ValidateArgRange(start <= Length - length);
                range.ValidateArgRange(length == value.Length);
                value.CopyTo(_storage.Slice(start, length));
            }
        }

        /// <summary>
        ///     Ensures that the list has a minimum capacity. 
        /// </summary>
        /// <param name="capacity">The minimum capacity.</param>
        /// <returns>The new capacity.</returns>
        public int EnsureCapacity(int capacity)
        {
            // This is not expected to be called this with negative capacity
            Debug.Assert(capacity >= 0);

            // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
            if ((uint)capacity > (uint)_storage.Length)
            {
                Grow(capacity - _pos);
            }

            return Capacity;
        }

        /// <summary>
        ///     Get a pinnable reference to the list.
        ///     Does not ensure there is a null T after <see cref="Length" />
        ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
        ///     the explicit method call, and write eg "fixed (T* c = list)"
        /// </summary>
        [Pure]
        public ref T GetPinnableReference()
        {
            return ref MemoryMarshal.GetReference(_storage);
        }

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => _storage;

        /// <summary>
        ///     Returns a span around the contents of the list.
        /// </summary>
        public ReadOnlySpan<T> AsSpan()
        {
            return _storage.Slice(0, _pos);
        }

        /// <summary>
        ///     Returns a span around a portion of the contents of the list.
        /// </summary>
        /// <param name="start">The zero-based index of the first element.</param>
        /// <returns>The span representing the content.</returns>
        public ReadOnlySpan<T> AsSpan(int start)
        {
            return _storage.Slice(start, _pos - start);
        }

        /// <summary>
        ///     Returns a span around a portion of the contents of the list. 
        /// </summary>
        /// <param name="start">The zero-based index of the first element.</param>
        /// <param name="length">The number of elements from the <paramref name="start"/>.</param>
        /// <returns></returns>
        public ReadOnlySpan<T> AsSpan(int start, int length)
        {
            return _storage.Slice(start, length);
        }

        /// <inheritdoc cref="List{T}.Add"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T value)
        {
            int pos = _pos;

            if ((uint)pos < (uint)_storage.Length)
            {
                _storage[pos] = value;
                _pos = pos + 1;
            }
            else
            {
                GrowAndAppend(value);
            }
        }


        /// <inheritdoc cref="List{T}.AddRange"/>
        public void AddRange(ReadOnlySpan<T> values)
        {
            InsertRange(Length, values);
        }

        /// <summary>
        ///     Appends a span to the list, and return the handle.
        /// </summary>
        /// <param name="length">The length of the span to add.</param>
        /// <returns>The span appended to the list.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AppendSpan(int length)
        {
            int origPos = _pos;
            if (origPos > _storage.Length - length)
            {
                Grow(length);
            }

            _pos = origPos + length;
            return _storage.Slice(origPos, length);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(in T item)
        {
            return _storage.BinarySearch(item, Comparer<T>.Default);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(in T item, IComparer<T> comparer)
        {
            return _storage.BinarySearch(item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(int, int, T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, in T item, IComparer<T> comparer)
        {
            return _storage.Slice(index, count).BinarySearch(item, comparer);
        }

        /// <inheritdoc cref="List{T}.Clear"/>
        public void Clear()
        {
            if (_arrayToReturnToPool != null)
            {
                Array.Clear(_arrayToReturnToPool, 0, _pos);
            }
            else
            {
                _storage.Clear();
            }

            _pos = 0;
        }

        /// <inheritdoc cref="List{T}.Contains"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in T item) => IndexOf(item, null) >= 0;

        /// <summary>
        ///     Determines whether an element is in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list. The value can be null for reference types.</param>
        /// <param name="comparer">The comparer used to determine whether two items are equal.</param>
        /// <returns><see langword="true"/> if item is found in the list; otherwise, <see langword="false"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in T item, IEqualityComparer<T>? comparer) => IndexOf(item, comparer) >= 0;

        /// <inheritdoc cref="Span{T}.CopyTo"/>
        public void CopyTo(Span<T> destination)
        {
            _storage.CopyTo(destination);
        }

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item) => IndexOf(item, null);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        public int IndexOf(in T item, IEqualityComparer<T>? comparer)
        {
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {

                    for (int i = 0; i < _pos; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, _storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = 0; i < _pos; i++)
            {
                if (!comparer.Equals(item, _storage[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Insert"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, in T value)
        {
            if (_pos > _storage.Length - 1)
            {
                Grow(1);
            }

            int remaining = _pos - index;
            _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + 1));
            _storage[index] = value;
            _pos += 1;
        }

        /// <inheritdoc cref="List{T}.InsertRange"/>
        public void InsertRange(int index, ReadOnlySpan<T> span)
        {
            index.ValidateArg(index >= 0);
            index.ValidateArg(index <= Length);
            int count = span.Length;
            if (count == 0)
            {
                return;
            }

            if (_pos > _storage.Length - count)
            {
                Grow(count);
            }

            int remaining = _pos - index;
            _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + count));
            span.CopyTo(_storage.Slice(index));
            _pos += count;
        }

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item) => LastIndexOf(item, null);

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        public int LastIndexOf(in T item, IEqualityComparer<T>? comparer)
        {
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {

                    for (int i = _pos - 1; i >= 0; i--)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, _storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = _pos - 1; i >= 0; i--)
            {
                if (!comparer.Equals(item, _storage[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Remove"/>
        public bool Remove(in T item) => Remove(item, null);

        /// <inheritdoc cref="List{T}.Remove"/>
        public bool Remove(in T item, IEqualityComparer<T>? comparer)
        {
            int index = IndexOf(item, comparer);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="List{T}.RemoveAt"/>
        public void RemoveAt(int index)
        {
            int remaining = _pos - index - 1;
            _storage.Slice(index + 1, remaining).CopyTo(_storage.Slice(index));
            _storage[--_pos] = default!;
        }

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        public void RemoveRange(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);

            int end = _pos - count;
            int remaining = end - start;
            _storage.Slice(start + count, remaining).CopyTo(_storage.Slice(start));

            if (_arrayToReturnToPool != null)
            {
                Array.Clear(_arrayToReturnToPool, end, count);
            }
            else
            {
                _storage.Slice(end).Clear();
            }

            _pos = end;
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        public void Reverse() => _storage.Slice(0, _pos).Reverse();

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        public void Reverse(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            _storage.Slice(start, count).Reverse();
        }

        /// <inheritdoc cref="List{T}.Sort()"/>
        public void Sort()
        {
            _storage.Slice(0, _pos).Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        public void Sort(Comparison<T> comparison)
        {
            _storage.Slice(0, _pos).Sort(comparison);
        }

        /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
        public void Sort(IComparer<T> comparer)
        {
            _storage.Slice(0, _pos).Sort(comparer);
        }

        /// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})"/>
        public void Sort(int start, int count, IComparer<T> comparer)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            _storage.Slice(start, count).Sort(comparer);
        }

        /// <inheritdoc cref="Span{T}.ToArray"/>
        public T[] ToArray()
        {
            T[] array = new T[_pos];
            _storage.Slice(0, _pos).CopyTo(array.AsSpan());
            return array;
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from a <see cref="RefVec{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
        public List<T> ToList()
        {
            List<T> list = new(_pos);
            for (int i = 0; i < _pos; i++)
            {
                list.Add(_storage[i]);
            }

            return list;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            T[]? toReturn = _arrayToReturnToPool;
            this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowAndAppend(in T value)
        {
            Grow(1);
            Add(value);
        }

        /// <summary>
        ///     Resize the internal buffer either by doubling current buffer size or
        ///     by adding <paramref name="additionalCapacityBeyondPos" /> to
        ///     <see cref="_pos" /> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        ///     Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(_pos > _storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
            T[] poolArray = ArrayPool<T>.Shared.Rent((int)Math.Max((uint)(_pos + additionalCapacityBeyondPos), (uint)_storage.Length * 2));

            _storage.Slice(0, _pos).CopyTo(poolArray);

            T[]? toReturn = _arrayToReturnToPool;
            _storage = _arrayToReturnToPool = poolArray;
            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

        private string GetDebuggerDisplay()
        {
            if (Length == 0)
                return "Length = 0, Values = []";
            StringBuilder sb = new(256);
            sb.Append("Length = ").Append(_pos);
            sb.Append(", Values = [");
            if (Length < 10)
            {
                int last = _pos - 1;
                for (int i = 0; i < last; i++)
                {
                    sb.Append(_storage[i]);
                    sb.Append(", ");
                }

                if (Length > 0)
                {
                    sb.Append(_storage[last]);
                }
            }
            else
            {
                sb.Append("...");
            }
            sb.Append(']');
            return sb.ToString();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        /// <summary>Enumerates the elements of a <see cref="RefVec{T}"/>.</summary>
        public ref struct Enumerator
        {
            private readonly RefVec<T> _list;
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="list">The list to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(RefVec<T> list)
            {
                _list = list;
                _index = -1;
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _index + 1;
                if ((uint)index < (uint)_list.Length)
                {
                    _index = index;
                    return true;
                }

                return false;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public ref readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _list[_index];
            }

            /// <summary>Resets the enumerator to the initial state.</summary>
            public void Reset()
            {
                _index = -1;
            }

            /// <summary>Disposes the enumerator.</summary>
            public void Dispose()
            {
                this = default;
            }
        }
    }
}