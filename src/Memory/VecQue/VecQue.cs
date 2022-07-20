using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory.Common;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed FILO list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
[DebuggerDisplay("Length = {Count}")]
[DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
public class VecRing<T> : IVector<T>
{
    /// <summary>
    /// The internal storage.
    /// </summary>
    protected T[]? Storage;

    private int _tail, _head;
    private int _count;

    /// <summary>
    ///     Initializes a new list.
    /// </summary>
    public VecRing()
    {
    }

    /// <summary>
    ///     Initializes a new list with a initial buffer.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    public VecRing(T[] initialBuffer)
    {
        Storage = initialBuffer;
        _head = _tail = 0;
    }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    public VecRing(int initialMinimumCapacity)
    {
        Storage = new T[initialMinimumCapacity];
        _head = _tail = 0;
    }


    /// <inheritdoc />
    public int Capacity => (Storage?.Length) ?? 0;

    public int Head => _head;

    public int Tail => _tail;

    public int Ceiling => Capacity - _head;

    /// <inheritdoc />
    public int Length
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tail - _head;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(Storage is null || value + Head <= Capacity);
            _tail = _head + value;
        }
    }

    /// <inheritdoc />
    public int Count => Length;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    public bool IsEmpty => 0 >= Length;

    /// <inheritdoc />
    public bool IsDefault => Storage is null;


    /// <inheritdoc />
    public ref T this[int index]
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Storage.ValidateArg(Storage is not null);
            Debug.Assert(index + HeadVirtual < Tail);
            return ref Storage[GetIndex(index)];
        }
    }

    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }
    T IReadOnlyList<T>.this[int index] => this[index];

    ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

    public ref T this[Index index] => ref this[index.GetOffset(Length)];

    ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index];

    private void GuardRange(int start, int count)
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
    }

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

        start += _head;
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

    /// <inheritdoc />
    [Pure]
    public int IndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        start += _head;
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

    /// <inheritdoc cref="IList{T}.IndexOf(T)"/>
    public int IndexOf(in T item) => IndexOf(0, Length, in item);

    int IList<T>.IndexOf(T item) => IndexOf(in item);

    /// <inheritdoc />
    [Pure]
    public int LastIndexOf<E>(int start, int count, in T item, in E comparer) where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        start += _head;
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

    /// <inheritdoc />
    [Pure]
    public int LastIndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        start += _head;
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

    /// <inheritdoc />
    [Pure]
    public int BinarySearch<C>(int start, int count, in T item, in C comparer) where C : IComparer<T>
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
        return Storage.AsSpan(start + _head, count).BinarySearch(item, comparer);
    }

    /// <inheritdoc />
    [Pure]
    public int BinarySearch(int start, int count, in T item)
    {
        start.ValidateArgRange(start >= 0);
        count.ValidateArgRange(count >= 0);
        count.ValidateArgRange(start <= Length - count);
        return Storage.AsSpan(start + _head, count).BinarySearch(item, Comparer<T>.Default);
    }

    /// <inheritdoc />
    [Pure]
    public bool TryCopyTo(Span<T> destination)
    {
        return IsEmpty || Storage.AsSpan(_head, Length).TryCopyTo(destination);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <inheritdoc />
    public void CopyTo(Array array, int index) => CopyTo((T[])array, index);


    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    [Pure]
    public MemIter<T> GetEnumerator()
    {
        return new(Storage, _head, Length);
    }

    [Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    [Pure]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public int Reserve(int additionalCapacity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void RemoveRange(int start, int count)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Sort<C>(int start, int count, in C comparer) where C : IComparer<T>
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Sort(int start, int count)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Reverse(int start, int count)
    {
        throw new NotImplementedException();
    }
}
