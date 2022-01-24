using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using HeaplessUtility.Common;

namespace HeaplessUtility
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items of the list.</typeparam>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    [DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
    public class Vec<T> : IVector<T>
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

        /// <inheritdoc />
        public int Capacity => Storage is null ? 0 : Storage.Length;

        /// <inheritdoc />
        public int Length
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(Storage == null || value <= Storage.Length);
                _count = value;
            }
        }

        /// <inheritdoc/>
        public int Count => Length;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => null!;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc />
        public bool IsEmpty => 0u >= (uint)_count;

        /// <inheritdoc/>
        public bool IsDefault => Storage is null;

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => Storage;

        /// <inheritdoc />
        public ref T this[int index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                this.ValidateArg(Storage is not null);
                Debug.Assert(index < _count);
                return ref Storage[index];
            }
        }

        ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => this[index];

        /// <inheritdoc />
        public ref T this[Index index]
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int offset = index.GetOffset(_count);
                offset.ValidateArg(offset >= 0 && offset < Length);
                return ref Storage![offset];
            }
        }

        ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index];

        /// <summary>
        ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The range of elements to get or set.</param>
        public ReadOnlySpan<T> this[Range range]
        {
            get
            {
                (int start, int count) = range.GetOffsetAndLength(_count);
                start.ValidateArg(start >= 0);
                count.ValidateArg(count >= 0);
                count.ValidateArg(start <= Length - count);
                return new ReadOnlySpan<T>(Storage, start, count);
            }
            set
            {
                (int start, int count) = range.GetOffsetAndLength(_count);
                start.ValidateArg(start >= 0);
                count.ValidateArg(count >= 0);
                count.ValidateArg(start <= Length - count);
                value.CopyTo(Storage.AsSpan(start, count));
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

        /// <inheritdoc />
        [Pure]
        public ReadOnlySpan<T> AsSpan(int start, int count)
        {
            return new(Storage, start, count);
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

        /// <inheritdoc />
        public int BinarySearch<C>(int start, int count, in T item, [AllowNull] in C comparer) where C : IComparer<T>
        {
            start.ValidateArgRange(start >= 0);
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
        public bool Contains(in T item) => this.IndexOf(item) >= 0;

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
        public bool Contains(in T item, IEqualityComparer<T>? comparer) => this.IndexOf(item, comparer) >= 0;

        /// <inheritdoc />
        public bool TryCopyTo(Span<T> destination)
        {
            if (IsEmpty)
            {
                return true;
            }

            return Storage.AsSpan().TryCopyTo(destination);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (Storage == null)
            {
                return;
            }
            Array.Copy(Storage, 0, array, arrayIndex, _count);
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        public int IndexOf<E>(int start, int count, in T item, in E comparer)
            where E : IEqualityComparer<T>
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);

            if (Storage is null)
            {
                return -1;
            }

            int end = start + count;
            if (comparer is null)
            {
                if (typeof(T).IsValueType)
                {
                    for (int i = start; i < end; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }
                else
                {
                    var defaultCmp = EqualityComparer<T>.Default;
                    for (int i = start; i < end; i++)
                    {
                        if (!defaultCmp.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }
                }
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    if (!comparer.Equals(item, Storage[i]))
                    {
                        continue;
                    }

                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        int IList<T>.IndexOf(T item) => this.IndexOf(item);

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
            Length += 1;
        }

        /// <inheritdoc />
        void IList<T>.Insert(int index, T item) => Insert(index, item);

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

            if (Storage == null || Length > Capacity - count)
            {
                Grow(count);
            }

            T[] storage = Storage!;
            Array.Copy(storage, index, storage, index + count, _count - index);
            span.CopyTo(storage.AsSpan(index));
            Length += count;
        }

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        [Pure]
        public int LastIndexOf<E>(int start, int count, in T item, [AllowNull] in E comparer)
            where E : IEqualityComparer<T>
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);

            if (Storage == null)
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
                        if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }

                    return -1;
                }
                else
                {
                    var defaultCmp = EqualityComparer<T>.Default;
                    for (int i = end - 1; i >= start; i--)
                    {
                        if (!defaultCmp.Equals(item, Storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }
                }
            }
            else
            {
                for (int i = end - 1; i >= start; i--)
                {
                    if (!comparer.Equals(item, Storage[i]))
                    {
                        continue;
                    }

                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Remove"/>
        [CLSCompliant(false)]
        public bool Remove(in T item) => this.Remove(item);

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item) => this.Remove(item);

        /// <inheritdoc cref="List{T}.RemoveAt"/>
        public void RemoveAt(int index)
        {
            this.ValidateArg(Storage is not null);
            int remaining = _count - index - 1;
            Array.Copy(Storage, index + 1, Storage, index, remaining);
            Storage[--_count] = default!;
        }

        /// <inheritdoc  />
        public void RemoveRange(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Count - count);
            this.ValidateArg(Storage is not null);

            int end = Length - count;
            int remaining = end - start;
            Array.Copy(Storage, start + count, Storage, start, remaining);
            Array.Clear(Storage, end, count);
            Length = end;
        }

        /// <inheritdoc  />
        public void Reverse(int start, int count)
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            if (count != 0)
            {
                this.ValidateArg(Storage is not null);
                Array.Reverse(Storage, start, count);
            }
        }

        /// <inheritdoc  />
        public void Sort<C>(int start, int count, [AllowNull] in C comparer)
            where C : IComparer<T>
        {
            start.ValidateArg(start >= 0);
            count.ValidateArg(count >= 0);
            count.ValidateArg(start <= Length - count);
            if (count != 0)
            {
                this.ValidateArg(Storage is not null);
                if (comparer is null)
                {
                    Storage.AsSpan(start, count).Sort();
                }
                else
                {
                    Storage.AsSpan(start, count).Sort(comparer);
                }
            }
        }

        /// <inheritdoc cref="Span{T}.ToArray"/>
        public T[] ToArray()
        {
            if (Storage is not null)
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
            this.ValidateArg(Storage is not null);
            List<T> list = new(_count);
            for (int i = 0; i < _count; i++)
            {
                list.Add(Storage[i]);
            }

            return list;
        }

        /// <inheritdoc />
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

            if (Storage is not null)
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
            if (IsEmpty)
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