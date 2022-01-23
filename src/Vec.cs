using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

using HeaplessUtility.DebugViews;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items of the list.</typeparam>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    [DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
    public class Vec<T> :
        IList<T>,
        ICollection,
        IReadOnlyList<T>
    {
        /// <summary>
        /// The internal storage.
        /// </summary>
        protected T[]? Storage;

        private int _count;

        /// <summary>
        ///     Initializes a new list.
        /// </summary>
        public Vec()
        {
        }

        /// <summary>
        ///     Initializes a new list with a initial buffer.
        /// </summary>
        /// <param name="initialBuffer">The initial buffer.</param>
        public Vec(T[] initialBuffer)
        {
            Storage = initialBuffer;
            _count = 0;
        }

        /// <summary>
        ///     Initializes a new list with a specified minimum initial capacity.
        /// </summary>
        /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
        public Vec(int initialMinimumCapacity)
        {
            Storage = new T[initialMinimumCapacity];
            _count = 0;
        }

        /// <inheritdoc cref="List{T}.Capacity"/>
        public int Capacity
        {
            get
            {
                ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
                return Storage.Length;
            }
        }

        /// <inheritdoc cref="List{T}.Count"/>
        public int Count
        {
            get => _count;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(Storage == null || value <= Storage.Length);
                _count = value;
            }
        }

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => null!;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc cref="Span{T}.IsEmpty"/>
        public bool IsEmpty => 0 >= (uint)_count;

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => Storage;

        /// <inheritdoc cref="List{T}.this"/>
        public ref T this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
                Debug.Assert(index < _count);
                return ref Storage[index];
            }
        }

        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => this[index];

        /// <summary>
        ///     Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        public ref T this[Index index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int offset = index.GetOffset(_count);
                if ((uint)offset >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.index);
                }
                return ref Storage![offset];
            }
        }

        /// <summary>
        ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The range of elements to get or set.</param>
        public ReadOnlySpan<T> this[Range range]
        {
            get
            {
                (int start, int length) = range.GetOffsetAndLength(_count);
                if (_count - start < length)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.range);
                }
                return new ReadOnlySpan<T>(Storage, start, length);
            }
            set
            {
                (int start, int length) = range.GetOffsetAndLength(_count);
                if (_count - start < length)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.range);
                }
                if (value.Length != length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, "The length of the span is not equal to the lenght of the range.");
                }
                value.CopyTo(Storage.AsSpan(start, length));
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
            if (Storage == null || (uint)capacity > (uint)Storage.Length)
            {
                Grow(capacity - _count);
            }

            return Capacity;
        }

#if NET50_OR_GREATER || NETCOREAPP3_1

        /// <summary>
        ///     Get a pinnable reference to the list.
        ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
        ///     the explicit method call, and write eg "fixed (T* c = list)"
        /// </summary>
        [Pure]
        public unsafe ref T GetPinnableReference()
        {
            ref T ret = ref Unsafe.AsRef<T>(null);
            if (Storage != null && 0 >= (uint)Storage.Length)
            {
                ret = ref Storage[0];
            }

            return ref ret;
        }

#endif

        /// <summary>
        ///     Returns a span around the contents of the list.
        /// </summary>
        [Pure]
        public ReadOnlySpan<T> AsSpan()
        {
            return new(Storage, 0, _count);
        }

        /// <summary>
        ///     Returns a span around a portion of the contents of the list.
        /// </summary>
        /// <param name="start">The zero-based index of the first element.</param>
        /// <returns>The span representing the content.</returns>
        [Pure]
        public ReadOnlySpan<T> AsSpan(int start)
        {
            return new(Storage, start, _count - start);
        }

        /// <summary>
        ///     Returns a span around a portion of the contents of the list.
        /// </summary>
        /// <param name="start">The zero-based index of the first element.</param>
        /// <param name="length">The number of elements from the <paramref name="start"/>.</param>
        /// <returns></returns>
        [Pure]
        public ReadOnlySpan<T> AsSpan(int start, int length)
        {
            return new(Storage, start, length);
        }

        /// <inheritdoc cref="List{T}.Add"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            int pos = _count;

            if (Storage != null && (uint)pos < (uint)Storage.Length)
            {
                Storage[pos] = value;
                _count = pos + 1;
            }
            else
            {
                GrowAndAppend(value);
            }
        }

        /// <inheritdoc cref="List{T}.AddRange"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(ReadOnlySpan<T> value)
        {
            int pos = _count;
            if (Storage == null || pos > Storage.Length - value.Length)
            {
                Grow(value.Length);
            }

            value.CopyTo(Storage.AsSpan(_count));
            _count += value.Length;
        }

        /// <summary>
        ///     Appends a span to the list, and return the handle.
        /// </summary>
        /// <param name="length">The length of the span to add.</param>
        /// <returns>The span appended to the list.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AppendSpan(int length)
        {
            int origPos = _count;
            if (Storage == null || origPos > Storage.Length - length)
            {
                Grow(length);
            }

            _count = origPos + length;
            return Storage.AsSpan(origPos, length);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(in T item)
        {
            if (Storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(Storage, item, Comparer<T>.Default);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(in T item, IComparer<T> comparer)
        {
            if (Storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(Storage, item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(int, int, T, IComparer{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, in T item, IComparer<T> comparer)
        {
            if (Storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(Storage, index, count, item, comparer);
        }

        /// <inheritdoc cref="List{T}.Clear"/>
        public void Clear()
        {
            if (Storage != null)
            {
                Array.Clear(Storage, 0, _count);
            }
            _count = 0;
        }

        /// <inheritdoc cref="List{T}.Contains"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public bool Contains(in T item) => IndexOf(item, null) >= 0;

        /// <inheritdoc/>
        bool ICollection<T>.Contains(T item) => Contains(item);

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
            Storage.CopyTo(destination);
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (Storage == null)
            {
                return;
            }
            Array.Copy(Storage, 0, array, arrayIndex, _count);
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public int IndexOf(in T item) => IndexOf(item, null);

        /// <inheritdoc />
        int IList<T>.IndexOf(T item) => IndexOf(item);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        public int IndexOf(in T item, IEqualityComparer<T>? comparer)
        {
            if (Storage == null)
            {
                return -1;
            }

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    for (int i = 0; i < _count; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = 0; i < _count; i++)
            {
                if (!comparer.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Insert"/>
        [CLSCompliant(false)]
        public void Insert(int index, in T value)
        {
            if (Storage == null || _count > Storage.Length - 1)
            {
                Grow(1);
            }

            T[] storage = Storage!;
            Array.Copy(storage, index, storage, index + 1, _count - index);
            storage[index] = value;
            _count += 1;
        }

        /// <inheritdoc />
        void IList<T>.Insert(int index, T item) => Insert(index, item);

        /// <inheritdoc cref="List{T}.InsertRange"/>
        public void InsertRange(int index, ReadOnlySpan<T> span)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.index);
            }
            if (index > _count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.index, index);
            }

            int count = span.Length;
            if (count == 0)
            {
                return;
            }

            if (Storage == null || _count > Storage.Length - count)
            {
                Grow(count);
            }

            T[] storage = Storage!;
            Array.Copy(storage, index, storage, index + count, _count - index);
            span.CopyTo(storage.AsSpan(index));
            _count += count;
        }

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item) => LastIndexOf(item, null);

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        public int LastIndexOf(in T item, IEqualityComparer<T>? comparer)
        {
            if (Storage == null)
            {
                return -1;
            }

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    for (int i = _count - 1; i >= 0; i--)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }

                comparer = EqualityComparer<T>.Default;
            }

            for (int i = _count - 1; i >= 0; i--)
            {
                if (!comparer.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Remove"/>
        [CLSCompliant(false)]
        public bool Remove(in T item) => Remove(item, null);

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item) => Remove(item, null);

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
            ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
            int remaining = _count - index - 1;
            Array.Copy(Storage, index + 1, Storage, index, remaining);
            Storage[--_count] = default!;
        }

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        public void RemoveRange(int index, int count)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
            int end = _count - count;
            int remaining = end - index;
            Array.Copy(Storage, index + count, Storage, index, remaining);
            Array.Clear(Storage, end, count);
            _count = end;
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        public void Reverse()
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
            Array.Reverse(Storage, 0, _count);
        }

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        public void Reverse(int start, int count)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
            if ((uint)start >= (uint)_count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.start, start);
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.count);
            }
            if (_count - start < count)
            {
                ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.value);
            }
            Array.Reverse(Storage, start, count);
        }

        /// <inheritdoc cref="List{T}.Sort()"/>
        public void Sort()
        {
            Storage.AsSpan(0, _count).Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        public void Sort(Comparison<T> comparison)
        {
            Storage.AsSpan(0, _count).Sort(comparison);
        }

        /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
        public void Sort(IComparer<T> comparer)
        {
            Storage.AsSpan(0, _count).Sort(comparer);
        }

        /// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})"/>
        public void Sort(int start, int count, IComparer<T> comparer)
        {
            if ((uint)start >= (uint)_count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.start, start);
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.count);
            }
            if (_count - start < count)
            {
                ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.value);
            }
            Storage.AsSpan(start, count).Sort(comparer);
        }

        /// <inheritdoc cref="Span{T}.ToArray"/>
        public T[] ToArray()
        {
            if (Storage != null)
            {
                T[] array = new T[_count];
                Array.Copy(Storage, 0, array, 0, _count);
                return array;
            }

            return Array.Empty<T>();
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from a <see cref="RefVec{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
        public List<T> ToList()
        {
            ThrowHelper.ThrowIfObjectNotInitialized(Storage == null);
            List<T> list = new(_count);
            for (int i = 0; i < _count; i++)
            {
                list.Add(Storage[i]);
            }

            return list;
        }

        /// <summary>
        /// Ensures that the collection can contain at least <paramref name="additionalCapacity"/> more elements.
        /// </summary>
        /// <param name="additionalCapacity">The number of additional elements the collection must be able to contain.</param>
        /// <returns>The capacity of the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Reserve(int additionalCapacity)
        {
            int req = Count + additionalCapacity;
            if (req > Capacity)
            {
                Grow(additionalCapacity);
            }
            return Capacity;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowAndAppend(T value)
        {
            Grow(1);
            Add(value);
        }

        /// <summary>
        ///     Resize the internal buffer either by doubling current buffer size or
        ///     by adding <paramref name="additionalCapacityBeyondPos" /> to
        ///     <see cref="_count" /> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        ///     Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (Storage != null)
            {
                Debug.Assert(_count > Storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

                T[] temp = new T[(int)Math.Max((uint)(_count + additionalCapacityBeyondPos), (uint)Storage.Length * 2)];
                Array.Copy(Storage, 0, temp, 0, _count);
                Storage = temp;
            }
            else
            {
                Storage = new T[(int)Math.Max(16u, (uint)additionalCapacityBeyondPos)];
            }
        }

        private string GetDebuggerDisplay()
        {
            if (Storage == null || _count == 0)
                return "Count = 0";
            StringBuilder sb = new(256);
            sb.Append("Count = ").Append(_count);
            return sb.ToString();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [Pure]
        public Enumerator GetEnumerator() => new(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates the elements of a <see cref="Vec{T}"/>.</summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Vec<T> _list;
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="list">The list to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(Vec<T> list)
            {
                _list = list;
                _index = -1;
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _index + 1;
                if ((uint)index < (uint)_list.Count)
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

            T IEnumerator<T>.Current => Current;

            object? IEnumerator.Current => Current;

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