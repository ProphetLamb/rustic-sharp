using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory.Common;

using static Rustic.Option;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed LIFO list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
[DebuggerDisplay("Length = {Count}")]
[DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
public class VecQue<T> : IVector<T>
{
    /// <summary>
    ///     Defines the behavior of the <see cref="VecQue{T}"/> when the capacity is exceeded.
    /// </summary>
    protected readonly GrowthStrategy GrowthStrategy;

    /// <summary>
    ///     The internal storage
    /// </summary>
    protected T[]? Storage;

    private int _tail, _length;

    /// <summary>
    ///     Initializes a new list.
    /// </summary>
    /// <param name="growthStrategy">The growth strategy</param>
    public VecQue(GrowthStrategy growthStrategy)
    {
        GrowthStrategy = growthStrategy;
    }

    /// <summary>
    ///     Initializes a new list.
    /// </summary>
    public VecQue()
        : this(GrowthStrategy.Exponential)
    {
    }

    /// <summary>
    ///     Initializes a new list with a initial buffer.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    /// <param name="growthStrategy">The growth strategy</param>
    public VecQue(T[] initialBuffer, GrowthStrategy growthStrategy)
        : this(growthStrategy)
    {
        Storage = initialBuffer;
    }
    /// <summary>
    ///     Initializes a new list with a initial buffer.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    public VecQue(T[] initialBuffer)
        : this(initialBuffer, GrowthStrategy.Exponential) { }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    /// <param name="growthStrategy">The growth strategy</param>
    public VecQue(int initialMinimumCapacity, GrowthStrategy growthStrategy)
        : this(growthStrategy)
    {
        Grow(initialMinimumCapacity);
    }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    public VecQue(int initialMinimumCapacity)
        : this(initialMinimumCapacity, GrowthStrategy.Exponential) { }

    /// <inheritdoc />
    public int Capacity => (Storage?.Length) ?? 0;

    /// <summary>
    ///     The absolute position of the tail
    /// </summary>
    public int Tail
    {
        get => _tail;
    }

    /// <summary>
    ///     Assigns the Tail field the specified value in range [0..Capacity)
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is outside of [0..Capacity)</exception>
    protected void SetTailValue(in int value)
    {
        if ((nuint)value >= (nuint)Capacity)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(value), value, "Must be in range [0..Capacity)");
        }

        _tail = value;
    }

    /// <summary>
    ///     The absolute position of the head, wrapped if necessary.
    /// </summary>
    public int Head => IndexAbsolute(Length).Index;

    /// <summary>
    ///     The virtual position of the Top, no wrapping is performed. Hence top may exceed the capacity.
    /// </summary>
    /// <remarks>
    ///     Computes `Tail + Length`.
    /// </remarks>
    public int HeadVirtual => Tail + Length;

    /// <summary>
    ///     The capacity in the primary partition of the ringbuffer.
    /// </summary>
    /// <remarks>
    ///    Computes `Capacity - Tail`.
    /// </remarks>
    public int TopCapacity => Capacity - Tail;

    /// <inheritdoc cref="IVector{T}.Count" />
    public int Length
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(Storage is null || value <= Capacity);
            _length = value;
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
            Debug.Assert(index + Length < Tail);
            return ref Storage[IndexAbsolute(index).Index];
        }
    }

    T IReadOnlyList<T>.this[int index] => this[index];
    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

    ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

    /// <inheritdoc />
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

        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
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

    /// <summary>
    /// Uses `IndexAbsolute` to compute where the range is located inside the storage.
    /// </summary>
    /// <remarks>
    ///     The partition from Tail to min(Top, Head) is returned as primary; otherwise, PrimaryStart == PrimaryEnd == Top.
    ///
    ///     The partition from Top to max(Top, Head) is returned as wrapped; otherwise, WrappedStart == WrappedEnd == 0.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int PrimaryStart, int PrimaryEnd, int WrappedStart, int WrappedEnd) RangeAbsolute(int virtualStart, int count)
    {
        int virtualEnd = virtualStart + count;
        (int startIndex, bool startWrapped) = IndexAbsolute(virtualStart);
        (int endIndex, bool endWrapped) = IndexAbsolute(virtualEnd);
        int primaryStart = startWrapped ? TopCapacity : startIndex;
        int primaryEnd = endIndex.Min(TopCapacity);
        int wrappedStart = startWrapped ? startIndex : 0;
        int wrappedEnd = endWrapped ? endIndex : 0;
        return (primaryStart, primaryEnd, wrappedStart, wrappedEnd);
    }

    /// <summary>
    ///     Same as `RangeAbsolute`, but produces counts instead of end indices.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int PrimaryStart, int PrimaryCount, int WrappedStart, int WrappedCount) RangeAbsoluteWithCount(
        int virtualStart, int count)
    {
        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(virtualStart, count);
        return (primaryStart, primaryEnd - primaryStart, wrappedStart, wrappedEnd - wrappedStart);
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

    [Pure]
    private int IndexOfValueType(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
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
    [Pure]
    public int IndexOf(in T item) => IndexOf(0, Length, in item);

    [Pure]
    int IList<T>.IndexOf(T item) => IndexOf(in item);

    [Pure]
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

        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
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

    [Pure]
    private int LastIndexOfValueType(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Storage is null)
        {
            return -1;
        }

        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
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
        (int primaryStart, int primaryCount, int wrappedStart, int wrappedCount) = RangeAbsoluteWithCount(start, count);
        int primaryPos = Storage.AsSpan(primaryStart, primaryCount).BinarySearch(item, comparer);
        if (primaryPos >= 0)
        {
            return IndexVirtual(primaryStart + primaryPos);
        }
        int wrappedPos = Storage.AsSpan(wrappedStart, wrappedCount).BinarySearch(item, comparer);
        if (wrappedPos >= 0)
        {
            return IndexVirtual(wrappedStart + wrappedPos);
        }
        // Ensure that the return value is the correct negative if not found.
        int primaryComp = primaryStart + ~primaryPos;
        if (primaryComp < primaryCount)
        {
            return ~IndexVirtual(primaryComp);
        }
        int wrappedComp = wrappedStart + ~wrappedPos;
        return ~IndexVirtual(wrappedComp);
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
        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(0, Length);
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

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((T[])array, index);
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    [Pure]
    public MemIter<T> GetEnumerator()
    {
        return new(Storage, Tail, Length);
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
    public void Add(T item) => Insert(Length, in item);

    /// <inheritdoc />
    public void Clear()
    {
        if (Storage is null)
        {
            return;
        }
        _tail = 0;
        _length = 0;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    public void AddRange(ReadOnlySpan<T> items)
    {
        (int head, _) = ReserveInternal(items.Length);
        items.CopyTo(Storage.AsSpan(head, items.Length));
        _length += items.Length;
    }

    /// <summary>
    ///     Appends a span to the list, and return the handle.
    /// </summary>
    /// <param name="length">The length of the span to add.</param>
    /// <returns>The span appended to the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan(int length)
    {
        (int head, _) = ReserveInternal(length);
        _length += length;

        return Storage.AsSpan(head, length);
    }

    /// <inheritdoc />
    public void Insert(int index, in T value)
    {
        int absoluteIndex = AmortizeInsert(index, 1);
        Storage![absoluteIndex] = value;
    }


    void IList<T>.Insert(int index, T item) => Insert(index, in item);

    /// <inheritdoc />
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        int absoluteIndex = AmortizeInsert(index, values.Length);
        values.CopyTo(Storage.AsSpan(absoluteIndex, values.Length));
    }

    private int AmortizeInsert(int start, int count)
    {
        start.ValidateArgRange((uint)start <= (uint)Length);
        _= ReserveInternal(count);
        MoveLeftVirtual(start, Length - start, count);
        return IndexAbsolute(start).Index;
    }

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

    public void RemoveBack(int count)
    {
        count.ValidateArgRange((uint)count < (uint)Length);
        _tail += count;
    }

    public Option<T> RemoveFront()
    {
        if (IsEmpty)
        {
            return default;
        }
        int len = _length;
        T result = Storage![IndexAbsolute(len).Index];
        _length = len - 1;
        return Some(result);
    }

    public void RemoveFront(int count)
    {
        count.ValidateArgRange((uint)count < (uint)Length);
        _length -= count;
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

    /// <inheritdoc />
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

    public void Swap(int i, int j)
    {
        i.ValidateArgRange((uint)i < (uint)Length);
        j.ValidateArgRange((uint)j < (uint)Length);
        ref T lhs = ref this[i];
        ref T rhs = ref this[j];
        (rhs, lhs) = (lhs, rhs);
    }

    private void RemoveInternal(int start, int count)
    {
        (int primaryStart, int primaryEnd, int wrappedStart, int wrappedEnd) = RangeAbsolute(start, count);
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

    /// <inheritdoc />
    public int Reserve(int additionalCapacity)
    {
        if (additionalCapacity > 0)
        {
            ReserveInternal(additionalCapacity);
        }
        return Capacity;
    }

    private (int IndexAbsolute, bool IsWrapped) ReserveInternal(int additionalCapacity)
    {
        int cap = Capacity;
        // the absolute index of the head with no wrapping
        int reqIndex = HeadVirtual + additionalCapacity;
        if (reqIndex <= cap)
        {
            return (reqIndex - additionalCapacity, false); // Sufficient capacity available from Tail to Top.
        }

        int wrappedReqIndex = reqIndex - cap;
        if (wrappedReqIndex <= Tail)
        {
            return (wrappedReqIndex - additionalCapacity, true); // Sufficient capacity available from Head to Tail.
        }

        // Insufficient capacity available
        Grow(additionalCapacity, true);
        int idx = GetContiguousCapacityIndexVirtual(additionalCapacity, out int wrapping);
        return (idx, wrapping >= 0);
    }

    /// <summary>
    ///     Grows the buffer by at least additionalCapacity.
    /// </summary>
    /// <remarks>
    ///    Reforms the buffer, so that Tail is 0 and HeadVirtual is Head.
    /// </remarks>
    protected virtual void GrowContiguous(int additionalCapacity)
    {
        if (Storage is null)
        {
            Storage = new T[additionalCapacity];
            return;
        }

        int newCapacity = (Capacity * 2).Max(Capacity + additionalCapacity);
        var newStorage = new T[newCapacity];

        bool success = TryCopyTo(newStorage.AsSpan(0, Length));
        Debug.Assert(success, "This can never fail!");

        Storage = newStorage;
        _tail = 0;
    }

    /// <summary>
    ///     If <paramref name="requireContiguous"/> rotates the buffer to make <paramref name="additionalCapacity"/> available as a contiguous block of memory.;
    ///     if sufficient capacity is available does nothing;
    ///     otherwise throws.
    /// </summary>
    protected virtual void GrowFixedSize(int additionalCapacity, bool requireContiguous)
    {
        if (Storage is null)
        {
            Storage = new T[additionalCapacity];
            return;
        }

        if (additionalCapacity + Length > Capacity)
        {
            ThrowGrowFixedSize(nameof(Capacity));
        }

        if (!requireContiguous)
        {
            return;
        }

        if (GetContiguousCapacityIndexVirtual(additionalCapacity, out _) >= 0)
        {
            return;
        }

        MoveLeftVirtual(Tail, Length, Tail);
        _tail = 0;
    }

    /// <summary>
    ///     Returns whether the ringbuffer contains amount consecutive elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int GetContiguousCapacityIndexVirtual(int amount, out int wrapping)
    {
        wrapping = HeadVirtual - Capacity;
        if (wrapping < 0 && -wrapping >= amount) // Sufficient capacity, unwrapped head
        {
            return Length + wrapping;
        }

        if (wrapping >= 0 && Tail - wrapping >= amount) // Sufficient capacity, wrapped head
        {
            return wrapping;
        }

        return -1;
    }

    /// <summary>
    ///     Grows the buffer according to the growth stragety.
    /// </summary>
    /// <paramref name="requireContiguous" >
    ///     Indicates whether the <paramref name="additionalCapacity"/> must be available as a contiguous block of memory.
    /// </paramref>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow(int additionalCapacity, bool requireContiguous = false)
    {
        if (Length + additionalCapacity < Capacity)
        {
            if (!requireContiguous)
            {
                // Sufficient non-contiguous capacity available
                return;
            }

            if (GetContiguousCapacityIndexVirtual(additionalCapacity, out _) >= 0)
            {
                // Sufficient contiguous capacity available
                return;
            }
        }

        switch (GrowthStrategy)
        {
            case GrowthStrategy.Exponential:
                GrowContiguous(additionalCapacity);
                break;
            case GrowthStrategy.FixedSizeAlign:
                GrowFixedSize(additionalCapacity, requireContiguous);
                break;
            default:
                ThrowUnknownGrowthStrategy();
                break;
        }
    }

    /// <summary>
    ///     Transforms the ringbuffer into a contiguous region.
    /// </summary>
    /// <paramref name="onlyIfCheap">If true and the operation is not cheap returns default, otherwise makes contiguous.</paramref>
    /// <returns>The span representing the contiguous region</returns>
    /// <remarks>
    ///     Does not necessarily guarantee, that Tail = 0, but that `IsWrapping` is false.
    ///
    ///     If `IsWrapped` is false then the operation is cheap; otherwise
    ///     - if `onlyIfCheap` returns `default`; if not, the operation is expensive.
    ///
    ///     - Attempts to rotate the ringbuffer to to right/tail, into a contiguous region, if it is not already.
    ///     Only possible, when at a at least a specific amount of elements are available in the ringbuffer,
    ///     by rotating to the right by at least HeadVirtual - Capacity and at max Tail.
    ///
    ///     - If that fails, reallocates the buffer to a new contiguous region, this guarantees `Tail` = 0.
    ///     This results in an increase in capacity, but is still cheaper then a 2nd temporary buffer.
    /// </remarks>
    public Span<T> MakeContiguous(bool onlyIfCheap = false)
    {
        int wrapping = HeadVirtual - Capacity;
        if (wrapping <= 0)
        {
            return Storage.AsSpan(Tail, Length);
        }

        if (onlyIfCheap)
        {
            return default;
        }

        int growReq = GetShiftRequirements(wrapping);
        if (growReq != 0)
        {
            Grow(growReq, true);
        }
        else
        {
            MoveLeftVirtual(Tail, Length, wrapping);
            _tail = 0;
        }

        return Storage.AsSpan(Tail, Length);
    }

    protected void MoveLeftVirtual(int startIndex, int count, int amount)
    {
        Debug.Assert(Length + amount <= Capacity, "Insufficient overhead capacity available");

        var (primaryStart, primaryCount, wrappedStart, wrappedCount) = RangeAbsoluteWithCount(startIndex, count);
        if (wrappedCount > 0)
        {
            // Inside the wrapped partition
            // Move to accomodate for primary partition, if any
            Storage.AsSpan(wrappedStart, wrappedCount).CopyTo(Storage.AsSpan(wrappedStart + amount, wrappedCount));
        }

        if (primaryCount <= 0)
        {
            return;
        }

        // Inside the primary/unwrapped partition
        // Compute destination for primary window, this may require wrapping.
        var (dstPrimaryStart, dstPrimaryCount, dstWrappedStart, dstWrappedCount)
            = RangeAbsoluteWithCount(primaryStart + amount, primaryCount);
        if (dstWrappedCount > 0)
        {
            // Move primary partition to wrapping partition, if any

            // We know that [primaryStart..primaryEnd) in the primary partition, so [primaryStart..primaryEnd)+amount
            // may never overlap with Tail, because amount is defined as the minimum available capacity.
            Storage.AsSpan(primaryStart + dstPrimaryCount, dstWrappedCount).CopyTo(Storage.AsSpan(dstWrappedStart, dstWrappedCount));
        }

        if (dstPrimaryCount > 0)
        {
            // Move primary partition, if any
            Storage.AsSpan(primaryStart, dstPrimaryCount).CopyTo(Storage.AsSpan(dstPrimaryStart, dstPrimaryCount));
        }
    }

    /// <summary>
    ///     Returns the requirements for a rotation by amount
    /// </summary>
    /// <remarks>
    ///     If the rotation is cheap returns 0, otherwise the amount of elements that would be required to rotate.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetShiftRequirements(int amount)
    {
        int available = Capacity - Length;
        int growReq = Math.Max(amount - available, 0);
        return growReq;
    }

    /// <inheritdoc />
    public void Sort<C>(int start, int count, in C comparer) where C : IComparer<T>
    {
        var storage = MakeContiguous(); // maybe expensive
        storage.Slice(start, count).Sort(comparer);
    }

    /// <inheritdoc />
    public void Sort(int start, int count)
    {
        var storage = MakeContiguous(); // maybe expensive
        storage.Slice(start, count).Sort();
    }

    /// <inheritdoc />
    public void Reverse(int start, int count)
    {
        var storage = MakeContiguous(); // maybe expensive
        storage.Slice(start, count).Reverse();
    }

    [DoesNotReturn]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowGrowFixedSize(string max)
    {
        throw new ArgumentOutOfRangeException("additionalCapacity",
            $"Cannot grow the fixed size buffer by more than {max} by rotating");
    }

    [DoesNotReturn]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowUnknownGrowthStrategy()
    {
        throw new InvalidOperationException("Unknown growth strategy");
    }

}
