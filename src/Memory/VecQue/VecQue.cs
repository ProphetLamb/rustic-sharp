using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory.Common;

using static Rustic.Option;

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

    private int _tail, _headVirtual;

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
        _headVirtual = _tail = 0;
    }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    public VecRing(int initialMinimumCapacity)
    {
        Storage = new T[initialMinimumCapacity];
        _headVirtual = _tail = 0;
    }


    /// <inheritdoc />
    public int Capacity => (Storage?.Length) ?? 0;

    /// <summary>
    ///     The virtual position of the head, no wrapping is performed. Hence head may exceed the capacity.
    /// </summary>
    public int HeadVirtual => _headVirtual;

    /// <summary>
    ///     The absolute position of the tail, always less then <see cref="HeadVirtual"/>.
    /// </summary>
    public int Tail => _tail;

    /// <summary>
    ///     The absolute position of the head, wrapped if necessary.
    /// </summary>
    public int Head => IndexAbsolute(HeadVirtual).Index;

    public int Top => Capacity - Tail;

    /// <summary>
    ///     Indicates whether the head is wrapped, or not.
    /// </summary>
    public bool IsWrapped
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IndexAbsolute(HeadVirtual).IsWrapped;
    }

    /// <inheritdoc cref="IVector{T}.Count" />
    public int Length
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _headVirtual - _tail;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(Storage is null || value + HeadVirtual <= Capacity);
            _tail = _headVirtual + value;
        }
    }


    /// <inheritdoc cref="IVector{T}.Count" />
    public int Count => Length;

    bool ICollection<T>.IsReadOnly => false;

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

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
            return ref Storage[IndexAbsolute(index).Index];
        }
    }

    T IReadOnlyList<T>.this[int index] => this[index];
    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

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

        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        // primary loop
        for (int i = primaryStart; i < primaryEnd; i++)
        {
            if (!comparer.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }
        // wrapped loop
        for (int i = wrappedStart; i < wrappedEnd; i++)
        {
            if (!comparer.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        return -1;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int Index, bool IsWrapped) IndexAbsolute(int virtualIndex)
    {
        int absolute = virtualIndex + Tail;
        bool isWrapped = absolute >= Capacity;
        return (absolute - (isWrapped ? Capacity : 0), isWrapped);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexVirtual(int absoluteIndex)
    {
        int relative = absoluteIndex - Tail;
        return relative + (relative < 0 ? Capacity : 0);
    }

    private (int PrimaryStart, int PrimaryEnd, int WrappedStart, int WrappedEnd) RangeAbsolute(int virtualStart, int count)
    {
        (int startIndex, bool startWrapped) = IndexAbsolute(virtualStart);
        (int endIndex, _) = IndexAbsolute(virtualStart + count);
        int primaryStart = startWrapped ? Top : startIndex;
        int primaryEnd = endIndex.Min(Top);
        int wrappedStart = startWrapped ? startIndex : Tail;
        int wrappedEnd = endIndex.Min(Tail);
        return (primaryStart, primaryEnd, wrappedStart, wrappedEnd);
    }

    /// <inheritdoc />
    [Pure]
    public int IndexOf(int start, int count, in T item)
    {
        if (typeof(T).IsValueType)
        {
            return IndexOfValueType(start, count, in item);
        }

        return IndexOf<IEqualityComparer<T>>(start, count, in item, EqualityComparer<T>.Default);
    }

    private int IndexOfValueType(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        // primary loop
        for (int i = primaryStart; i < primaryEnd; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        // wrapped loop
        for (int i = wrappedStart; i < wrappedEnd; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        return -1;
    }


    /// <inheritdoc cref="IList{T}.IndexOf(T)"/>
    [CLSCompliant(false)]
    public int IndexOf(in T item) => IndexOf(0, Length, in item);

    int IList<T>.IndexOf(T item) => IndexOf(in item);

    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <inheritdoc />
    [Pure]
    public int LastIndexOf<E>(int start, int count, in T item, in E comparer) where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        // wrapped loop
        for (int i = wrappedEnd - 1; i >= wrappedStart; i--)
        {
            if (!comparer.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        // primary loop
        for (int i = primaryEnd - 1; i >= primaryStart; i--)
        {
            if (!comparer.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        return -1;
    }

    /// <inheritdoc />
    [Pure]
    public int LastIndexOf(int start, int count, in T item)
    {
        if (typeof(T).IsValueType)
        {
            return LastIndexOfValueType(start, count, in item);
        }

        return LastIndexOf<IEqualityComparer<T>>(start, count, in item, EqualityComparer<T>.Default);
    }

    private int LastIndexOfValueType(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        // wrapped loop
        for (int i = wrappedEnd - 1; i >= wrappedStart; i--)
        {
            if (!EqualityComparer<T>.Default.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        // primary loop
        for (int i = primaryEnd - 1; i >= primaryStart; i--)
        {
            if (!EqualityComparer<T>.Default.Equals(Storage[i], item))
            {
                continue;
            }

            return IndexVirtual(i);
        }

        return -1;
    }

    /// <inheritdoc />
    [Pure]
    public int BinarySearch<C>(int start, int count, in T item, in C comparer) where C : IComparer<T>
    {
        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        int primaryPos = Storage.AsSpan(primaryStart, primaryEnd - primaryStart).BinarySearch(item, comparer);
        if (primaryPos >= 0)
        {
            return IndexVirtual(primaryStart + primaryPos);
        }
        int wrappedPos = Storage.AsSpan(wrappedStart, wrappedEnd - wrappedStart).BinarySearch(item, comparer);
        if (wrappedPos >= 0)
        {
            return IndexVirtual(wrappedStart + wrappedPos);
        }
        // Ensure that the return value is the correct negative if not found.
        int primaryComp = ~primaryPos;
        if (primaryComp < primaryEnd)
        {
            return ~IndexVirtual(primaryStart + primaryComp);
        }
        int wrappedComp = ~wrappedPos;
        return ~IndexVirtual(wrappedStart + wrappedComp);
    }

    /// <inheritdoc />
    [Pure]
    public int BinarySearch(int start, int count, in T item)
    {
        return BinarySearch<IComparer<T>>(start, count, in item, Comparer<T>.Default);
    }

    /// <inheritdoc />
    [Pure]
    public bool TryCopyTo(Span<T> destination)
    {
        if (Storage is null || destination.Length < Length)
        {
            return false;
        }
        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(0, Length);
        int primaryCount = primaryEnd - primaryStart;
        int wrappedCount = wrappedEnd - wrappedStart;
        return Storage.AsSpan(primaryStart, primaryCount).TryCopyTo(destination.Slice(0, primaryCount))
            && Storage.AsSpan(wrappedStart, wrappedCount).TryCopyTo(destination.Slice(wrappedCount));
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    public void CopyTo(Array array, int index)
    {
        CopyTo((T[])array, index);
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    [Pure]
    public MemIter<T> GetEnumerator()
    {
        return new(Storage, _headVirtual, Length);
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


    public void AddBack(in T item)
    {
    }

    public void AddFront(in T item)
    {
        Reserve(1);
        int headVirtual = _headVirtual;
        Storage![headVirtual] = item;
        _headVirtual = headVirtual + 1;
    }

    public void Add(T item) => AddBack(in item);

    public void Clear()
    {
        if (Storage is null)
        {
            return;
        }
        _tail = 0;
        _headVirtual = 0;
    }

    public void AddRange(ReadOnlySpan<T> items)
    {
        Reserve(items.Length);
        int headVirtual = _headVirtual;
        var (head, _) = IndexAbsolute(headVirtual);
        items.CopyTo(Storage.AsSpan(head, items.Length));
        _headVirtual = headVirtual + items.Length;
    }

    /// <summary>
    ///     Appends a span to the list, and return the handle.
    /// </summary>
    /// <param name="length">The length of the span to add.</param>
    /// <returns>The span appended to the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan(int length)
    {
        var headVirtual = _headVirtual;
        Reserve(length);
        _headVirtual = headVirtual + length;

        var (head , _) = IndexAbsolute(headVirtual);
        return Storage.AsSpan(head, length);
    }


    /// <inheritdoc />
    public void Insert(int index, in T value)
    {
        index.ValidateArgRange((uint)index <= (uint)Length);
        if (index == 0)
        {
            AddBack(in value);
            return;
        }

        if (index == Length)
        {
            AddFront(in value);
            return;
        }

        AmortizeInsert(index, 1);
        Storage![IndexAbsolute(index).Index] = value;
    }

    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        throw new NotImplementedException();
    }

    private void AmortizeInsert(int start, int count)
    {

        Reserve(count);
        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
        throw new NotImplementedException()
    }

    void IList<T>.Insert(int index, T item) => Insert(index, in item);

    public Option<T> RemoveBack()
    {
        if (IsEmpty)
        {
            return default;
        }
        T result = Storage![0];
        _tail += 1;
        return Some(result);
    }

    public Option<T> RemoveFront()
    {
        if (IsEmpty)
        {
            return default;
        }
        int headVirtual = _headVirtual;
        T result = Storage![IndexAbsolute(headVirtual).Index];
        _headVirtual = headVirtual - 1;
        return Some(result);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0)
        {
            return false;
        }
        RemoveAt(index);
        return true;
    }

    /// <inheritdoc />
    public void RemoveAt(int index) => RemoveRange(index, 1);

    public void RemoveRange(int start, int count)
    {
        GuardRange(start, count);
        if (start == 0)
        {
            RemoveBack(count);
            return;
        }
        if (start + count == Length)
        {
            RemoveFront(count);
            return;
        }
        RemoveInternal(start, count);
    }

    private void RemoveInternal(int start, int count)
    {
        var (primaryStart, primaryEnd, wrappedStart, wrappedEnd) = RangeAbsolute(start, count);
        int primaryCount = primaryEnd - primaryStart;
        if (primaryCount != 0)
        {
            // move [Tail, primaryStart) to [Tail+primaryCount,primaryEnd) to remove [primaryStart, primaryEnd)
            int primaryOuterCount = primaryStart - Tail;
            Storage.AsSpan(Tail, primaryOuterCount).CopyTo(Storage.AsSpan(Tail + primaryCount, primaryOuterCount));
        }

        int wrappedCount = wrappedEnd - wrappedStart;
        if (wrappedCount != 0)
        {
            // move [wrappedEnd,Head) to [wrappedStart,Head-wrappedCount) to remove [wrappedStart, wrappedEnd)
            int wrappedOuterCount = Head - wrappedEnd;
            Storage.AsSpan(wrappedEnd, wrappedOuterCount).CopyTo(Storage.AsSpan(wrappedStart, wrappedOuterCount));
        }
    }

    public void RemoveBack(int count)
    {
        count.ValidateArgRange((uint)count < (uint)Length);
        _tail += count;
    }

    public void RemoveFront(int count)
    {
        count.ValidateArgRange((uint)count < (uint)Length);
        _headVirtual -= count;
    }

    public int Reserve(int additionalCapacity)
    {
        int cap = Capacity;
        int reqIndex = HeadVirtual + additionalCapacity;
        if (reqIndex <= cap)
        {
            return cap; // Sufficient capacity available from Head to Capacity.
        }

        // Insufficient capacity available from Head to Capacity.
        if (!IsWrapped)
        {
            // Reshape the buffer to make room for the additional capacity.
            GrowAndUnwrap(additionalCapacity);
            return Capacity;
        }

        int wrappedIndex = reqIndex - Capacity;
        if (wrappedIndex <= Tail)
        {
            return cap; // Sufficient capacity available from wrapped Head to Tail.
        }

        // Insufficient capacity available from wrapped Head to Tail.
        GrowAndUnwrap(additionalCapacity);
        return Capacity;
    }

    private void GrowAndUnwrap(int additionalCapacity)
    {
        if (Storage is null)
        {
            Storage = new T[additionalCapacity];
            return;
        }

        int newCapacity = (Capacity*2).Max(Capacity + additionalCapacity);
        var newStorage = new T[newCapacity];

        if (!TryCopyTo(newStorage.AsSpan(0, Length)))
        {
            throw new InvalidOperationException("Failed to copy the contents of the buffer to the new storage.");
        }
        Storage = newStorage;
    }
}
