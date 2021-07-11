using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility
{
    /// <summary>
    ///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public ref struct RefList<T>
    {
        private T[]? _arrayToReturnToPool;
        private Span<T> _storage;
        private int _count;

        public RefList(Span<T> initialBuffer)
        {
            _arrayToReturnToPool = null;
            _storage = initialBuffer;
            _count = 0;
        }

        public RefList(int initialCapacity)
        {
            _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
            _storage = _arrayToReturnToPool;
            _count = 0;
        }

        public int Capacity => _storage.Length;

        public int Count
        {
            get => _count;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _storage.Length);
                _count = value;
            }
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index < _count);
                return ref _storage[index];
            }
        }

        public void EnsureCapacity(int capacity)
        {
            // This is not expected to be called this with negative capacity
            Debug.Assert(capacity >= 0);

            // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
            if ((uint) capacity > (uint) _storage.Length)
            {
                Grow(capacity - _count);
            }
        }

        /// <summary>
        ///     Get a pinnable reference to the list.
        ///     Does not ensure there is a null T after <see cref="Count" />
        ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
        ///     the explicit method call, and write eg "fixed (T* c = list)"
        /// </summary>
        public ref T GetPinnableReference()
        {
            return ref MemoryMarshal.GetReference(_storage);
        }

        public override string ToString()
        {
            var s = _storage.Slice(0, _count).ToString();
            Dispose();
            return s;
        }

        private string GetDebuggerDisplay()
        {
            if (Count == 0)
                return "Length = 0, Values = []";
            StringBuilder sb = new(256);
            sb.Append("Length = ").Append(_count);
            sb.Append(", Values = [");

            int last = _count - 1;
            for (int i = 0; i < last; i++)
            {
                sb.Append(_storage[i]);
                sb.Append(", ");
            }

            if (Count > 0)
            {
                sb.Append(_storage[last]);
            }
            
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        ///     Returns the underlying storage of the list.
        /// </summary>
        public Span<T> RawStorage => _storage;

        /// <summary>
        ///     Returns a span around the contents of the list.
        /// </summary>
        public ReadOnlySpan<T> AsSpan()
        {
            return _storage.Slice(0, _count);
        }

        public ReadOnlySpan<T> AsSpan(int start)
        {
            return _storage.Slice(start, _count - start);
        }

        public ReadOnlySpan<T> AsSpan(int start, int length)
        {
            return _storage.Slice(start, length);
        }

        /// <inheritdoc cref="List{T}.Add"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            int pos = _count;

            if ((uint) pos < (uint) _storage.Length)
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
            if (pos > _storage.Length - value.Length)
            {
                Grow(value.Length);
            }

            value.CopyTo(_storage.Slice(_count));
            _count += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AppendSpan(int length)
        {
            int origPos = _count;
            if (origPos > _storage.Length - length)
            {
                Grow(length);
            }

            _count = origPos + length;
            return _storage.Slice(origPos, length);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        public int BinarySearch(T item)
        {
            return _storage.BinarySearch(item, Comparer<T>.Default);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return _storage.BinarySearch(item, comparer);
        }

        /// <inheritdoc cref="List{T}.BinarySearch(int, int, T, IComparer{T})"/>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _storage.Slice(index, count).BinarySearch(item, comparer);
        }

        /// <inheritdoc cref="List{T}.Clear"/>
        public void Clear()
        {
            if (_arrayToReturnToPool != null)
            {
                Array.Clear(_arrayToReturnToPool, 0, _count);
            }
            else
            {
                _storage.Clear();
            }

            _count = 0;
        }

        /// <inheritdoc cref="List{T}.Contains"/>
        public bool Contains(T item) => IndexOf(item, null) >= 0;
        
        /// <summary>
        ///     Determines whether an element is in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list. The value can be null for reference types.</param>
        /// <param name="comparer">The comparer used to determine whether two items are equal.</param>
        /// <returns><see langword="true"/> if item is found in the list; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item, IEqualityComparer<T>? comparer) => IndexOf(item, comparer) >= 0;

        /// <inheritdoc cref="Span{T}.CopyTo"/>
        public void CopyTo(Span<T> destination)
        {
            _storage.CopyTo(destination);
        }
        
        /// <inheritdoc cref="IList{T}.IndexOf"/>
        public int IndexOf(T item) => IndexOf(item, null);

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        public int IndexOf(T item, IEqualityComparer<T>? comparer)
        {
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
                }
                else
                {
                    comparer = EqualityComparer<T>.Default;
                    
                    for (int i = 0; i < _count; i++)
                    {
                        if (!comparer.Equals(item, _storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {
                    if (!comparer.Equals(item, _storage[i]))
                    {
                        continue;
                    }

                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Insert"/>
        public void Insert(int index, T value)
        {
            if (_count > _storage.Length - 1)
            {
                Grow(1);
            }

            int remaining = _count - index;
            _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + 1));
            _storage[index] = value;
            _count += 1;
        }
        
        /// <inheritdoc cref="List{T}.InsertRange"/>
        public void InsertRange(int index, ReadOnlySpan<T> span)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument.index);
            }

            int count = span.Length;

            if (_count > _storage.Length - count)
            {
                Grow(count);
            }

            int remaining = _count - index;
            _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + count));
            span.CopyTo(_storage.Slice(index));
            _count += count;
        }
        
        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        public int LastIndexOf(T item) => LastIndexOf(item, null);

        /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
        public int LastIndexOf(T item, IEqualityComparer<T>? comparer)
        {
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
                }
                else
                {
                    comparer = EqualityComparer<T>.Default;
                    
                    for (int i = _count - 1; i >= 0; i--)
                    {
                        if (!comparer.Equals(item, _storage[i]))
                        {
                            continue;
                        }

                        return i;
                    }
                }
            }
            else
            {
                for (int i = _count - 1; i >= 0; i--)
                {
                    if (!comparer.Equals(item, _storage[i]))
                    {
                        continue;
                    }

                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="List{T}.Remove"/>
        public bool Remove(T item) => Remove(item, null);

        /// <inheritdoc cref="List{T}.Remove"/>
        public bool Remove(T item, IEqualityComparer<T>? comparer)
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
            int remaining = _count - index - 1;
            _storage.Slice(index + 1, remaining).CopyTo(_storage.Slice(index));
            _storage[--_count] = default!;
        }

        /// <inheritdoc cref="List{T}.RemoveRange"/>
        public void RemoveRange(int index, int count)
        {
            int end = _count - count;
            int remaining = end - index;
            _storage.Slice(index + count, remaining).CopyTo(_storage.Slice(index));

            if (_arrayToReturnToPool != null)
            {
                Array.Clear(_arrayToReturnToPool, end, count);
            }
            else
            {
                _storage.Slice(end).Clear(); 
            }

            _count = end;
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        public void Reverse() => _storage.Reverse();

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        public void Reverse(int start, int count) => _storage.Slice(start, count).Reverse();

        /// <inheritdoc cref="List{T}.Sort()"/>
        public void Sort()
        {
            _storage.Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        public void Sort(Comparison<T> comparison)
        {
            _storage.Sort(comparison);
        }

        /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
        public void Sort(IComparer<T> comparer)
        {
            _storage.Sort(comparer);
        }

        /// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})"/>
        public void Sort(int start, int count, IComparer<T> comparer)
        {
            _storage.Slice(start, count).Sort(comparer);
        }
        
        /// <inheritdoc cref="Span{T}.TryCopyTo"/>
        public bool TryCopyTo(Span<T> destination, out int charsWritten)
        {
            if (_storage.Slice(0, _count).TryCopyTo(destination))
            {
                charsWritten = _count;
                Dispose();
                return true;
            }

            charsWritten = 0;
            Dispose();
            return false;
        }

        /// <inheritdoc cref="Span{T}.ToArray"/>
        public T[] ToArray()
        {
            T[] array = new T[_count];
            _storage.CopyTo(array.AsSpan());
            return array;
        }
        
        /// <summary>
        /// Creates a <see cref="List{T}"/> from a <see cref="RefList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
        public List<T> ToList()
        {
            List<T> list = new(_count);
            for (int i = 0; i < _count; i++)
            {
                list.Add(_storage[i]);
            }

            return list;
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
            Debug.Assert(_count > _storage.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
            T[]? poolArray = ArrayPool<T>.Shared.Rent((int) Math.Max((uint) (_count + additionalCapacityBeyondPos), (uint) _storage.Length * 2));

            _storage.Slice(0, _count).CopyTo(poolArray);

            T[]? toReturn = _arrayToReturnToPool;
            _storage = _arrayToReturnToPool = poolArray;
            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

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
    }
}