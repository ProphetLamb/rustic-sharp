using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using HeaplessUtility.DebuggerViews;
using HeaplessUtility.Exceptions;
using HeaplessUtility.Interfaces;

namespace HeaplessUtility.Pool
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>. 
    /// </summary>
    /// <typeparam name="T">The type of items of the list.</typeparam>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    [DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
    public class RefList<T> :
        IList<T>,
        ICollection,
        IReadOnlyList<T>,
        IStrongEnumerable<T, RefList<T>.Enumerator>,
        IDisposable
    {
        private T[]? _storage;
        private readonly ushort _minimumCapacity;
        private int _count;
        private bool _isHeapBound;

        /// <summary>
        ///     Initializes a new list.
        /// </summary>
        public RefList()
        {
            _minimumCapacity = 16;
        }

        /// <summary>
        ///     Initializes a new list with a initial buffer.
        /// </summary>
        /// <param name="initialBuffer">The initial buffer.</param>
        public RefList(T[] initialBuffer)
        {
            _storage = initialBuffer;
            _minimumCapacity = 16;
            _count = 0;
            _isHeapBound = false;
        }
        
        /// <summary>
        ///     Initializes a new list with a specified minimum initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The minimum initial capacity.</param>
        public RefList(int initialCapacity)
        {
            ushort minimumCapacity = (ushort)initialCapacity;
            if (minimumCapacity != initialCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument.initialCapacity, initialCapacity, UInt16.MaxValue);
            }
            
            _storage = null;
            _minimumCapacity = minimumCapacity;
            _count = 0;
            _isHeapBound = false;
        }
        
        /// <inheritdoc cref="List{T}.Capacity"/>
        public int Capacity
        {
            get
            {
                ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
                return _storage!.Length;
            }
        }

        /// <inheritdoc cref="List{T}.Count"/>
        public int Count
        {
            get => _count;
            set
            {
                ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
                if ((uint) value >= (uint)_storage!.Length)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.value);
                }
                _count = value;
            }
        }

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => null!;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///     Specifies whether the list is bound to the shared array-pool or not.
        /// </summary>
        /// <remarks>
        ///     If changed to false, releases the array from the pool.
        /// <br/>
        ///     If changed to true, draws the array from the pool on the next resize.
        /// </remarks>
        public bool IsPoolBound
        {
            get => !_isHeapBound;
            set
            {
                if (!(_isHeapBound || value || _storage == null))
                {
                    T[] storage = new T[Math.Max(16, (uint) _count)];
                    Array.Copy(_storage, 0, storage, 0, _count);
                    ArrayPool<T>.Shared.Return(_storage);
                    _storage = storage;
                }

                _isHeapBound = value;
            }
        }

        /// <inheritdoc cref="Span{T}.IsEmpty"/>
        public bool IsEmpty => 0 >= (uint)_count;

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        internal Span<T> RawStorage => _storage;

        /// <inheritdoc cref="List{T}.this"/>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.index, index);
                }
                ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
                return ref _storage![index];
            }
        }

        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }
        
        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => this[index];

#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
        /// <summary>
        ///     Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        public ref T this[Index index]
        {
            get
            {
                int offset = index.GetOffset(_count);
                if ((uint)offset >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.index);
                }
                return ref _storage![offset];
            }
        }

        /// <summary>
        ///     Gets or sets a span of elements of elements from the specified <paramref name="range"/>.
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
                return new ReadOnlySpan<T>(_storage, start, length);
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
                FillRange(start, value.Slice(0, length));
            }
        }
#endif

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
            if (_storage == null || (uint)capacity > (uint)_storage.Length)
            {
                Grow(capacity - _count);
            }

            return Capacity;
        }

#if NET5_0 || NETCOREAPP3_1
        /// <summary>
        ///     Get a pinnable reference to the list.
        ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
        ///     the explicit method call, and write eg "fixed (T* c = list)"
        /// </summary>
        public unsafe ref T GetPinnableReference()
        {
            ref T ret = ref Unsafe.AsRef<T>(null);
            if (_storage != null && 0 >= (uint)_storage.Length)
            {
                ret = ref _storage[0];
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
            return new(_storage, 0, _count);
        }
        
        /// <summary>
        ///     Returns a span around a portion of the contents of the list.
        /// </summary>
        /// <param name="start">The zero-based index of the first element.</param>
        /// <returns>The span representing the content.</returns>
        [Pure]
        public ReadOnlySpan<T> AsSpan(int start)
        {
            return new(_storage, start, _count - start);
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
            return new(_storage, start, length);
        }

        /// <inheritdoc cref="List{T}.Add"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            int pos = _count;

            if (_storage != null && (uint)pos < (uint)_storage.Length)
            {
                _storage[pos] = value;
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
            if (_storage == null || pos > _storage.Length - value.Length)
            {
                Grow(value.Length);
            }
            
            value.CopyTo(_storage.AsSpan(_count));
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
            if (_storage == null || origPos > _storage.Length - length)
            {
                Grow(length);
            }

            _count = origPos + length;
            return _storage.AsSpan(origPos, length);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        public int BinarySearch(in T item)
        {
            if (_storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(_storage, item, Comparer<T>.Default);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        [Pure]
        public int BinarySearch(in T item, IComparer<T> comparer)
        {
            if (_storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(_storage, item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(int, int, T, IComparer{T})"/>
        [Pure]
        public int BinarySearch(int index, int count, in T item, IComparer<T> comparer)
        {
            if (_storage == null)
            {
                return -1;
            }
            return Array.BinarySearch(_storage, index, count, item, comparer);
        }

        /// <inheritdoc cref="List{T}.Clear"/>
        public void Clear()
        {
            if (_storage != null)
            {
                Array.Clear(_storage, 0, _count);
            }
            _count = 0;
        }

        /// <summary>
        ///     Fill the segment of the list with the value. --or-- Clears the segment of the list if the <paramref name="element"/> is <see langword="default!"/>.
        /// </summary>
        /// <param name="element">The element with which to fill the segment.</param>
        /// <param name="start">The zero-based index of the first element in the list.</param>
        public void Fill(T element, int start) => Fill(element, start, Count - start);
        
        /// <summary>
        ///     Fill the segment of the list with the value. --or-- Clears the segment of the list if the <paramref name="element"/> is <see langword="default!"/>.
        /// </summary>
        /// <param name="element">The element with which to fill the segment.</param>
        /// <param name="start">The zero-based index of the first element in the list.</param>
        /// <param name="count">The number of elements after the first element.</param>
        public void Fill(T element, int start, int count)
        {
            if (start < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.start);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.count);
            if (_count - start < count)
                ThrowHelper.ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument.count);
            
            if (_storage != null)
            {
                if (EqualityComparer<T>.Default.Equals(default!, element))
                {
                    Array.Clear(_storage, start, count);
                }
                else
                {
                    _storage.AsSpan(start, count).Fill(element);   
                }
            }
        }

        /// <summary>
        ///     Fills a segment of the list at the <paramref name="offset"/> with <paramref name="elements"/>.
        /// </summary>
        /// <param name="offset">The zero-based offset of the first element.</param>
        /// <param name="elements">The elements to fill the segment with.</param>
        public void FillRange(int offset, ReadOnlySpan<T> elements)
        {
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.offset);
            if (_count - offset < elements.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.elements, offset + elements.Length);

            elements.CopyTo(_storage.AsSpan(offset));
        }

        /// <inheritdoc cref="List{T}.Contains"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            _storage?.CopyTo(destination);
        }
        
        /// <inheritdoc cref="ICollection{T}.CopyTo"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_storage == null)
            {
                return;
            }
            Array.Copy(_storage, 0, array, arrayIndex, _count);
        }
        
        /// <inheritdoc cref="List{T}.CopyTo(int,T[],int,int)"/>
        public void CopyTo(int offset, T[] destination, int start, int count)
        {
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.offset);
            if (_count - offset < count)
                ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.elements, offset + count);
            
            if (count == 0)
            {
                return;
            }
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);

            _storage.AsSpan(offset).CopyTo(destination.AsSpan(start, count));
        }
        
        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);
        
        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item) => IndexOf(item, null);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        [Pure]
        public int IndexOf(in T item, IEqualityComparer<T>? comparer)
        {
            if (_storage == null)
            {
                return - 1;
            }
            
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    
                    for (int i = 0; i < _count; i++)
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
            
            for (int i = 0; i < _count; i++)
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
        public void Insert(int index, in T value)
        {
            if (_storage == null || _count > _storage.Length - 1)
            {
                Grow(1);
            }

            T[] storage = _storage!;
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

            if (_storage == null || _count > _storage.Length - count)
            {
                Grow(count);
            }

            T[] storage = _storage!;
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
            if (_storage == null)
            {
                return -1;
            }
            
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    
                    for (int i = _count - 1; i >= 0; i--)
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
            
            for (int i = _count - 1; i >= 0; i--)
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

        /// <inheritdoc />
        int IList<T>.IndexOf(T item) => IndexOf(item);

        /// <inheritdoc cref="List{T}.RemoveAt"/>
        public void RemoveAt(int index)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
            int remaining = _count - index - 1;
            Array.Copy(_storage, index + 1, _storage, index, remaining);
            _storage[--_count] = default!;
        }

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        public void RemoveRange(int index, int count)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
            int end = _count - count;
            int remaining = end - index;
            Array.Copy(_storage, index + count, _storage, index, remaining);
            Array.Clear(_storage, end, count);
            _count = end;
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        public void Reverse()
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
            Array.Reverse(_storage, 0, _count);
        }

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        public void Reverse(int start, int count)
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
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
            Array.Reverse(_storage, start, count);
        }

        /// <inheritdoc cref="List{T}.Sort()"/>
        public void Sort()
        {
            _storage.AsSpan(0, _count).Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        public void Sort(Comparison<T> comparison)
        {
            _storage.AsSpan(0, _count).Sort(comparison);
        }

        /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
        public void Sort(IComparer<T> comparer)
        {
            _storage.AsSpan(0, _count).Sort(comparer);
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
            _storage.AsSpan(start, count).Sort(comparer);
        }
        
        /// <inheritdoc cref="Span{T}.ToArray"/>
        public T[] ToArray()
        {
            if (_storage != null)
            {
                T[] array = new T[_count];
                Array.Copy(_storage, 0, array, 0, _count);
                return array;
            }

            return Array.Empty<T>();
        }
        
        /// <summary>
        /// Creates a <see cref="List{T}"/> from a <see cref="ValueList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
        public List<T> ToList()
        {
            ThrowHelper.ThrowIfObjectNotInitialized(_storage == null);
            List<T> list = new(_count);
            for (int i = 0; i < _count; i++)
            {
                list.Add(_storage[i]);
            }

            return list;
        }
        
        /// <inheritdoc cref="IDisposable.Dispose"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            T[]? toReturn = _storage;
            _storage = null;
            _count = 0;
            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
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
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);

            if (!_isHeapBound)
            {
                if (_storage != null)
                {
                    Debug.Assert(_count > _storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

                    // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
                    T[] poolArray = ArrayPool<T>.Shared.Rent((int)Math.Max((uint)(_count + additionalCapacityBeyondPos), (uint)_storage.Length * 2));
            
                    Array.Copy(_storage, 0, poolArray, 0, _count);

                    T[]? toReturn = _storage;
                    _storage = _storage = poolArray;
                    if (toReturn != null)
                    {
                        ArrayPool<T>.Shared.Return(toReturn);
                    }
                }
                else
                {
                    _storage = ArrayPool<T>.Shared.Rent((int)Math.Max(_minimumCapacity, (uint)additionalCapacityBeyondPos));
                }
            }
            else
            {
                if (_storage != null)
                {
                    Debug.Assert(_count > _storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

                    T[] storage = new T[(int)Math.Max((uint)(_count + additionalCapacityBeyondPos), (uint)_storage.Length * 2)];
                    Array.Copy(_storage, 0, storage, 0, _count);
                    _storage = storage;
                }
                else
                {
                    _storage = new T[Math.Max(_minimumCapacity, (uint)additionalCapacityBeyondPos)];
                }
            }
        }

        private string GetDebuggerDisplay()
        {
            if (_storage == null || _count == 0)
                return "Count = 0";
            StringBuilder sb = new(256);
            sb.Append("Count = ").Append(_count);
            return sb.ToString();
        }
        
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates the elements of a <see cref="RefList{T}"/>.</summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly RefList<T> _list;
            private int _index;
            
            /// <summary>Initialize the enumerator.</summary>
            /// <param name="list">The list to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(RefList<T> list)
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