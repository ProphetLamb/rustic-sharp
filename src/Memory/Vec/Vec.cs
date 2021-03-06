using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

using Rustic.Memory.Common;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
[DebuggerDisplay("Length = {Count}")]
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
    public int Capacity => (Storage?.Length) ?? 0;

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
            Debug.Assert(Storage is null || value <= Capacity);
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

    /// <inheritdoc/>
    ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

    /// <inheritdoc/>
    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

    /// <inheritdoc/>
    T IReadOnlyList<T>.this[int index] => this[index];

#if NETSTANDARD2_1 || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public ref T this[Index index]
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var offset = index.GetOffset(_count);
            offset.ValidateArgRange(offset >= 0 && offset < Length);
            return ref Storage![offset];
        }
    }

    /// <inheritdoc/>
    ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index];

    /// <summary>
    ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range of elements to get or set.</param>
    public ReadOnlySpan<T> this[Range range]
    {
        get
        {
            (var start, var count) = range.GetOffsetAndLength(_count);
            GuardRange(range, start, count);
            return new ReadOnlySpan<T>(Storage, start, count);
        }
        set
        {
            (var start, var count) = range.GetOffsetAndLength(_count);
            GuardRange(range, start, count);
            value.CopyTo(Storage.AsSpan(start, count));
        }
    }

    private void GuardRange(Range range, int start, int count)
    {
        range.ValidateArgRange(start >= 0);
        range.ValidateArgRange(count >= 0);
        range.ValidateArgRange(start <= Length - count);
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
        if (Storage is null || (uint)capacity > (uint)Storage.Length)
        {
            Grow(capacity - _count);
        }

        return Capacity;
    }

#if NET5_0_OR_GREATER || NETCOREAPP3_1

    /// <summary>
    ///     Get a pinnable reference to the list.
    ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
    ///     the explicit method call, and write eg "fixed (T* c = list)"
    /// </summary>
    [Pure]
    public unsafe ref T GetPinnableReference()
    {
        ref var ret = ref Unsafe.AsRef<T>(null);
        if (Storage is not null && 0 >= (uint)Storage.Length)
        {
            ret = ref Storage[0]!;
        }

        return ref ret!;
    }

#endif

    /// <inheritdoc />
    [Pure]
    public ReadOnlySpan<T> AsSpan(int start, int length)
    {
        return new(Storage!, start, length);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        var pos = _count;

        if (Storage is not null && (uint)pos < (uint)Storage.Length)
        {
            Storage[pos] = item;
            _count = pos + 1;
        }
        else
        {
            GrowAndAppend(item);
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
        var origPos = _count;
        if (Storage is null || origPos > Storage.Length - length)
        {
            Grow(length);
        }

        _count = origPos + length;
        return Storage.AsSpan(origPos, length);
    }

    /// <inheritdoc />
    public int BinarySearch<C>(int start, int count, in T item, in C comparer)
        where C : IComparer<T>
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
        return Storage.AsSpan(start, count).BinarySearch(item, comparer);
    }

    /// <inheritdoc cref="List{T}.Clear"/>
    public void Clear()
    {
        if (Storage is not null)
        {
            Array.Clear(Storage, 0, _count);
        }
        _count = 0;
    }

    /// <inheritdoc/>
    bool ICollection<T>.Contains(T item) => this.Contains(in item);

    /// <inheritdoc />
    public bool TryCopyTo(Span<T> destination)
    {
        return IsEmpty || Storage.AsSpan().TryCopyTo(destination);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

    /// <inheritdoc />
    [Pure]
    public int IndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var end = start + count;
        for (var i = start; i < end; i++)
        {
            if (!comparer.Equals(item, Storage[i]))
            {
                continue;
            }

            return i;
        }
        return -1;
    }

    private void GuardRange(int start, int count)
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
    }

    /// <inheritdoc />
    int IList<T>.IndexOf(T item) => this.IndexOf(in item);

    /// <inheritdoc cref="List{T}.Insert"/>
    [CLSCompliant(false)]
    public void Insert(int index, in T value)
    {
        if (Storage is null || _count > Storage.Length - 1)
        {
            Grow(1);
        }
        Debug.Assert(Storage is not null);

        var storage = Storage!;
        Array.Copy(storage, index, storage, index + 1, _count - index);
        storage[index] = value;
        Length += 1;
    }

    /// <inheritdoc />
    void IList<T>.Insert(int index, T item) => Insert(index, item);

    /// <inheritdoc cref="List{T}.InsertRange"/>
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        index.ValidateArgRange(index >= 0);
        index.ValidateArgRange(index <= Length);

        var count = values.Length;
        if (count == 0)
        {
            return;
        }

        if (Storage is null || Length > Capacity - count)
        {
            Grow(count);
        }

        Debug.Assert(Storage is not null);
        var storage = Storage;
        Array.Copy(storage, index, storage, index + count, _count - index);
        values.CopyTo(storage.AsSpan(index));
        Length += count;
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure]
    public int LastIndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var end = start + count;
        for (var i = end - 1; i >= start; i--)
        {
            if (!comparer.Equals(item, Storage[i]))
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item) => this.Remove(in item);

    /// <inheritdoc cref="List{T}.RemoveAt"/>
    public void RemoveAt(int index)
    {
        this.ValidateArg(Storage is not null);
        var remaining = _count - index - 1;
        Array.Copy(Storage, index + 1, Storage, index, remaining);
        Storage[--_count] = default!;
    }

    /// <inheritdoc  />
    public void RemoveRange(int start, int count)
    {
        GuardRange(start, count);

        if (count != 0)
        {
            Debug.Assert(Storage is not null);
            var end = Length - count;
            var remaining = end - start;
            Array.Copy(Storage, start + count, Storage, start, remaining);
            Array.Clear(Storage, end, count);
            Length = end;
        }
    }

    /// <inheritdoc  />
    public void Reverse(int start, int count)
    {
        GuardRange(start, count);
        if (count != 0)
        {
            Debug.Assert(Storage is not null);
            Array.Reverse(Storage, start, count);
        }
    }

    /// <inheritdoc  />
    public void Sort<C>(int start, int count, in C comparer)
        where C : IComparer<T>
    {
        GuardRange(start, count);
        if (count != 0)
        {
            Debug.Assert(Storage is not null);
            Storage.AsSpan(start, count).Sort(comparer);
        }
    }

    /// <inheritdoc cref="Span{T}.ToArray"/>
    public T[] ToArray()
    {
        if (Storage is not null)
        {
            var array = new T[_count];
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
        List<T> list = new(Count);
        if (Storage is null)
        {
            return list;
        }

        for (var i = 0; i < Count; i++)
        {
            list.Add(Storage[i]);
        }

        return list;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Reserve(int additionalCapacity)
    {
        var req = Count + additionalCapacity;
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

            var temp = new T[(_count + additionalCapacityBeyondPos).Max(Storage.Length * 2)];
            Array.Copy(Storage, 0, temp, 0, _count);
            Storage = temp;
        }
        else
        {
            Storage = new T[(int)Math.Max(16u, (uint)additionalCapacityBeyondPos)];
        }
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public void Sort(int start, int count)
    {
        GuardRange(start, count);

        if (count != 0)
        {
            Debug.Assert(Storage is not null);
            Storage.AsSpan(start, count).Sort();
        }
    }

    /// <inheritdoc/>
    public int IndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var end = start + count;
        if (typeof(T).IsValueType)
        {
            for (var i = start; i < end; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }
        }
        else
        {
            var defaultCmp = EqualityComparer<T>.Default;
            for (var i = start; i < end; i++)
            {
                if (!defaultCmp.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }
        }
        return -1;
    }

    /// <inheritdoc/>
    public int LastIndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var end = start + count;
        if (typeof(T).IsValueType)
        {
            for (var i = end - 1; i >= start; i--)
            {
                if (!EqualityComparer<T>.Default.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }

        }
        else
        {
            var defaultCmp = EqualityComparer<T>.Default;
            for (var i = end - 1; i >= start; i--)
            {
                if (!defaultCmp.Equals(item, Storage[i]))
                {
                    continue;
                }

                return i;
            }
        }
        return -1;
    }

    /// <inheritdoc/>
    public int BinarySearch(int start, int count, in T item)
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
        return Storage.AsSpan(start, count).BinarySearch(item, Comparer<T>.Default);
    }

    /// <summary>Enumerates the elements of a <see cref="Vec{T}"/>.</summary>
    public struct Enumerator : IEnumerator<T>
    {
        private Vec<T> _list;
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
            var index = _index + 1;
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
