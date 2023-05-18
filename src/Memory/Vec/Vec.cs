using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
[DebuggerDisplay("Length = {Count}"), DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
public class Vec<T> : IVector<T> {
    /// <summary>
    /// The internal storage.
    /// </summary>
    protected ArraySegment<T> Data;

    private int _count;

    /// <summary>
    ///     Initializes a new list.
    /// </summary>
    public Vec() {
    }

    /// <summary>
    ///     Initializes a new list with a initial buffer.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    public Vec(ArraySegment<T> initialBuffer) {
        Data = initialBuffer;
        _count = 0;
    }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    public Vec(int initialMinimumCapacity) {
        Data = new(new T[initialMinimumCapacity]);
        _count = 0;
    }

    /// <inheritdoc />
    public int Capacity => Data.Count;

    /// <inheritdoc />
    public int Length {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= Capacity);
            _count = value;
        }
    }

    /// <inheritdoc cref="IVector{T}.Count" />
    public int Count => Length;

    /// <inheritdoc/>
    bool ICollection.IsSynchronized => false;

    /// <inheritdoc/>
    object ICollection.SyncRoot => null!;

    /// <inheritdoc/>
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    public bool IsEmpty => 0u >= (uint)Length;

    /// <inheritdoc/>
    public bool IsDefault => Data.Array is null;

    /// <summary>
    ///     Returns the underlying RawStorage of the list.
    /// </summary>
    internal Span<T> RawStorage => Data;

    /// <inheritdoc />
    public ref T this[int index] {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            Debug.Assert(index < Length);
            return ref RawStorage[index];
        }
    }


    /// <inheritdoc/>
    ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

    /// <inheritdoc/>
    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

    /// <inheritdoc/>
    T IReadOnlyList<T>.this[int index] => this[index];

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc />
    public ref T this[Index index]
    {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int offset = index.GetOffset(Length);
            ThrowHelper.ArgumentInRange(offset, offset >= 0 && offset < Length);
            return ref RawStorage![offset];
        }
    }

    /// <inheritdoc/>
    ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index.GetOffset(Length)];

    /// <summary>
    ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range of elements to get or set.</param>
    public ReadOnlySpan<T> this[Range range]
    {
        get
        {
            (int start, int count) = range.GetOffsetAndLength(Length);
            GuardRange(range, start, count);
            return RawStorage.Slice(start, count);
        }
        set
        {
            (int start, int count) = range.GetOffsetAndLength(Length);
            GuardRange(range, start, count);
            value.CopyTo(Data.AsSpan(start, count));
        }
    }

    private void GuardRange(Range range, int start, int count)
    {
        ThrowHelper.ArgumentInRange(range, start >= 0);
        ThrowHelper.ArgumentInRange(range, count >= 0);
        ThrowHelper.ArgumentInRange(range, start <= Length - count);
    }
#endif

    /// <summary>
    ///     Ensures that the list has a minimum capacity.
    /// </summary>
    /// <param name="capacity">The minimum capacity.</param>
    /// <returns>The new capacity.</returns>
    public int EnsureCapacity(int capacity) {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)Capacity) {
            Grow(capacity - Length);
        }

        return Capacity;
    }

#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER

    /// <summary>
    ///     Get a pinnable reference to the list.
    ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
    ///     the explicit method call, and write eg "fixed (T* c = list)"
    /// </summary>
    [Pure]
    public unsafe ref T GetPinnableReference()
    {
        ref T ret = ref Unsafe.AsRef<T>(null);
        if (0 >= (uint)Capacity)
        {
            ret = ref RawStorage[0]!;
        }

        return ref ret!;
    }

#endif

    /// <inheritdoc />
    [Pure]
    public ReadOnlySpan<T> AsSpan(int start, int length) {
        GuardRange(start, length);
        return RawStorage.Slice(start, length);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item) {
        int pos = Length;

        if ((uint)pos < (uint)Capacity) {
            RawStorage[pos] = item;
            Length = pos + 1;
        } else {
            GrowAndAppend(item);
        }
    }

    /// <summary>
    ///     Appends a span to the list, and return the handle.
    /// </summary>
    /// <param name="length">The length of the span to add.</param>
    /// <returns>The span appended to the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan(int length) {
        int origPos = Length;
        if (origPos > Capacity - length) {
            Grow(length);
        }

        Length = origPos + length;
        return Data.AsSpan(origPos, length);
    }

    /// <inheritdoc />
    public int BinarySearch<C>(int start, int count, in T item, in C comparer)
        where C : IComparer<T> {
        GuardRange(start, count);
        return Data.AsSpan(start, count).BinarySearch(item, comparer);
    }

    /// <inheritdoc cref="List{T}.Clear"/>
    public void Clear() {
        RawStorage.Slice(0, Length).Clear();
        Length = 0;
    }

    /// <inheritdoc/>
    bool ICollection<T>.Contains(T item) => this.Contains(in item);

    /// <inheritdoc />
    public bool TryCopyTo(Span<T> destination) {
        return IsEmpty || Data.AsSpan().TryCopyTo(destination);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) {
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

    /// <inheritdoc />
    [Pure]
    public int IndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T> {
        GuardRange(start, count);
        int end = start + count;
        for (int i = start; i < end; i++) {
            if (!comparer.Equals(item, RawStorage[i])) {
                continue;
            }

            return i;
        }
        return -1;
    }

    private void GuardRange(int start, int count) {
        ThrowHelper.ArgumentInRange(start, start >= 0);
        ThrowHelper.ArgumentInRange(count, count >= 0);
        ThrowHelper.ArgumentInRange(count, start <= Length - count);
    }

    /// <inheritdoc />
    int IList<T>.IndexOf(T item) => this.IndexOf(in item);

    /// <inheritdoc cref="List{T}.Insert"/>
    [CLSCompliant(false)]
    public void Insert(int index, in T value) {
        ThrowHelper.ArgumentInRange(index, index >= 0 && index <= Length);
        if (Length > Capacity - 1) {
            Grow(1);
        }

        RawStorage.Slice(index, Length - index).CopyTo(RawStorage.Slice(index + 1));
        RawStorage[index] = value;
        Length += 1;
    }

    /// <inheritdoc />
    void IList<T>.Insert(int index, T item) => Insert(index, item);

    /// <inheritdoc cref="List{T}.InsertRange"/>
    public void InsertRange(int index, ReadOnlySpan<T> values) {
        ThrowHelper.ArgumentInRange(index, index >= 0);
        ThrowHelper.ArgumentInRange(index, index <= Length);

        int count = values.Length;
        if (count == 0) {
            return;
        }

        if (Length > Capacity - count) {
            Grow(count);
        }

        RawStorage.Slice(index, Length - index).CopyTo(RawStorage.Slice(index + count));
        values.CopyTo(RawStorage.Slice(index));
        Length += count;
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure]
    public int LastIndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T> {
        GuardRange(start, count);

        int end = start + count;
        for (int i = end - 1; i >= start; i--) {
            if (!comparer.Equals(item, RawStorage[i])) {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item) => this.Remove(in item);

    /// <inheritdoc cref="List{T}.RemoveAt"/>
    public void RemoveAt(int index) {
        ThrowHelper.ArgumentInRange(this, index >= 0 && index < Length);
        int last = Length - 1;
        int remaining = last - index;
        if (remaining != 0) {
            RawStorage.Slice(index + 1, remaining).CopyTo(RawStorage.Slice(index));
        }
        Length -= 1;
        RawStorage[last] = default!;
    }

    /// <inheritdoc  />
    public void RemoveRange(int start, int count) {
        GuardRange(start, count);

        if (count == 0) {
            return;
        }

        int end = Length - count;
        int remaining = end - start;
        RawStorage.Slice(start + count, remaining).CopyTo(RawStorage.Slice(start));
        RawStorage.Slice(end, count).Clear();
        Length = end;
    }

    /// <inheritdoc  />
    public void Reverse(int start, int count) {
        GuardRange(start, count);

        if (count != 0) {
            RawStorage.Slice(start, count).Reverse();
        }
    }

    /// <inheritdoc  />
    public void Sort<C>(int start, int count, in C comparer)
        where C : IComparer<T> {
        GuardRange(start, count);
        if (count != 0) {
            RawStorage.Slice(start, count).Sort(comparer);
        }
    }

    /// <inheritdoc cref="Span{T}.ToArray"/>
    public T[] ToArray() {
        T[] array = new T[Length];
        this.AsSpan().CopyTo(array);
        return array;
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> from a <see cref="RefVec{T}"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
    public List<T> ToList() => IsEmpty ? new() : new(this);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Reserve(int additionalCapacity) {
        int req = Length + additionalCapacity;
        if (req > Capacity) {
            Grow(additionalCapacity);
        }
        return Capacity;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(T value) {
        Grow(1);
        Add(value);
    }

    /// <summary>
    ///     Resize the internal buffer either by doubling current buffer size or
    ///     by adding <paramref name="additionalCapacityBeyondPos" /> to
    ///     <see cref="Length" /> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    ///     Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual void Grow(int additionalCapacityBeyondPos) {
        Debug.Assert(additionalCapacityBeyondPos > 0);

        if (Capacity != 0) {
            Debug.Assert(Length > Capacity - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            T[] temp = new T[(Length + additionalCapacityBeyondPos).Max(Capacity * 2)];
            RawStorage.Slice(0, Count).CopyTo(temp);
            Data = new(temp);
        } else {
            Data = new(new T[(int)Math.Max(16u, (uint)additionalCapacityBeyondPos)]);
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
    public void Sort(int start, int count) {
        GuardRange(start, count);
        if (count != 0) {
            Data.AsSpan(start, count).Sort();
        }
    }

    /// <inheritdoc/>
    public int IndexOf(int start, int count, in T item) {
        GuardRange(start, count);

        int end = start + count;
        if (typeof(T).IsValueType) {
            for (int i = start; i < end; i++) {
                if (!EqualityComparer<T>.Default.Equals(item, RawStorage[i])) {
                    continue;
                }

                return i;
            }
        } else {
            EqualityComparer<T> defaultCmp = EqualityComparer<T>.Default;
            for (int i = start; i < end; i++) {
                if (!defaultCmp.Equals(item, RawStorage[i])) {
                    continue;
                }

                return i;
            }
        }
        return -1;
    }

    /// <inheritdoc/>
    public int LastIndexOf(int start, int count, in T item) {
        GuardRange(start, count);

        int end = start + count;
        if (typeof(T).IsValueType) {
            for (int i = end - 1; i >= start; i--) {
                if (!EqualityComparer<T>.Default.Equals(item, RawStorage[i])) {
                    continue;
                }

                return i;
            }

        } else {
            EqualityComparer<T> defaultCmp = EqualityComparer<T>.Default;
            for (int i = end - 1; i >= start; i--) {
                if (!defaultCmp.Equals(item, RawStorage[i])) {
                    continue;
                }

                return i;
            }
        }
        return -1;
    }

    /// <inheritdoc/>
    public int BinarySearch(int start, int count, in T item) {
        ThrowHelper.ArgumentInRange(start, start >= 0);
        ThrowHelper.ArgumentInRange(count, count >= 0);
        ThrowHelper.ArgumentInRange(count, start <= Length - count);
        return Data.AsSpan(start, count).BinarySearch(item, Comparer<T>.Default);
    }

    /// <summary>Enumerates the elements of a <see cref="Vec{T}"/>.</summary>
    public struct Enumerator : IEnumerator<T> {
        private readonly Vec<T> _list;
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="list">The list to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(Vec<T> list) {
            _list = list;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            int index = _index + 1;
            if ((uint)index < (uint)_list.Count) {
                _index = index;
                return true;
            }

            return false;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public ref readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _list[_index];
        }

        T IEnumerator<T>.Current => Current;

        object? IEnumerator.Current => Current;

        /// <summary>Resets the enumerator to the initial state.</summary>
        public void Reset() {
            _index = -1;
        }

        /// <summary>Disposes the enumerator.</summary>
        public void Dispose() {
            this = default;
        }
    }
}
