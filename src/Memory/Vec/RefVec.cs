using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public ref struct RefVec<T> {
    private T[]? _arrayToReturnToPool;
    private Span<T> _storage;
    private int _pos;

    /// <summary>
    ///     Initializes a new list with a initial buffer.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefVec(Span<T> initialBuffer) {
        _arrayToReturnToPool = null;
        _storage = initialBuffer;
        _pos = 0;
    }

    /// <summary>
    ///     Initializes a new list with a specified minimum initial capacity.
    /// </summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefVec(int initialMinimumCapacity) {
        _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialMinimumCapacity);
        _storage = _arrayToReturnToPool;
        _pos = 0;
    }

    /// <inheritdoc cref="List{T}.Capacity"/>
    public int Capacity => _storage.Length;

    /// <inheritdoc cref="List{T}.Count"/>
    public int Length {
        get => _pos;
        set {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= Capacity);
            _pos = value;
        }
    }

    /// <inheritdoc cref="List{T}.Count"/>
    public int Count => _pos;

    /// <inheritdoc cref="Span{T}.IsEmpty"/>
    public bool IsEmpty {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0 >= (uint) _pos;
    }

    /// <inheritdoc cref="List{T}.this"/>
    public ref T this[int index] {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
        get {
            Debug.Assert(index < _pos);
            return ref _storage[index];
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    ///     Gets or sets the element at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the element to get or set.</param>
    public ref T this[in Index index]
    {
        [Pure,MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var offset = index.GetOffset(_pos);
            ThrowHelper.ArgumentInRange(index, offset >= 0 && offset < Length);
            return ref _storage[offset];
        }
    }

    /// <summary>
    ///     Gets a span of elements of elements from the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range of elements to get or set.</param>
    public ReadOnlySpan<T> this[in Range range]
    {
        [Pure,MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (var start, var count) = range.GetOffsetAndLength(_pos);
            GuardRange(range, start, count);
            return _storage.Slice(start, count);
        }
        set
        {
            (var start, var count) = range.GetOffsetAndLength(_pos);
            GuardRange(range, start, count);
            ThrowHelper.ArgumentInRange(range, count == value.Length);
            value.CopyTo(_storage.Slice(start, count));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GuardRange(in Range range, int start, int count)
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
        if ((uint) capacity > (uint) _storage.Length) {
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
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public ref T GetPinnableReference() {
        return ref MemoryMarshal.GetReference(_storage);
    }

    /// <summary>
    ///     Returns the underlying storage of the list.
    /// </summary>
    internal Span<T> RawStorage => _storage;

    /// <summary>
    ///     Returns a span around the contents of the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan() {
        return _storage.Slice(0, _pos);
    }

    /// <summary>
    ///     Returns a span around a portion of the contents of the list.
    /// </summary>
    /// <param name="start">The zero-based index of the first element.</param>
    /// <returns>The span representing the content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan(int start) {
        return _storage.Slice(start, _pos - start);
    }

    /// <summary>
    ///     Returns a span around a portion of the contents of the list.
    /// </summary>
    /// <param name="start">The zero-based index of the first element.</param>
    /// <param name="length">The number of elements from the <paramref name="start"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan(int start, int length) {
        return _storage.Slice(start, length);
    }

    /// <inheritdoc cref="List{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(in T value) {
        int pos = _pos;

        if ((uint) pos < (uint) _storage.Length) {
            _storage[pos] = value;
            _pos = pos + 1;
        } else {
            GrowAndAppend(value);
        }
    }


    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(ReadOnlySpan<T> values) {
        InsertRange(Length, values);
    }

    /// <summary>
    ///     Appends a span to the list, and return the handle.
    /// </summary>
    /// <param name="length">The length of the span to add.</param>
    /// <returns>The span appended to the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan(int length) {
        int origPos = _pos;
        if (origPos > _storage.Length - length) {
            Grow(length);
        }

        _pos = origPos + length;
        return _storage.Slice(origPos, length);
    }

    /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public int BinarySearch(in T item) {
        return _storage.BinarySearch(item, Comparer<T>.Default);
    }

    /// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public int BinarySearch(in T item, IComparer<T> comparer) {
        return _storage.BinarySearch(item, comparer);
    }

    /// <inheritdoc cref="List{T}.BinarySearch(Int32, Int32, T, IComparer{T})"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public int BinarySearch(int index, int count, in T item, IComparer<T> comparer) {
        return _storage.Slice(index, count).BinarySearch(item, comparer);
    }

    /// <inheritdoc cref="List{T}.Clear"/>
    public void Clear() {
        if (_arrayToReturnToPool is not null) {
            Array.Clear(_arrayToReturnToPool, 0, _pos);
        } else {
            _storage.Clear();
        }

        _pos = 0;
    }

    /// <inheritdoc cref="List{T}.Contains"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public bool Contains(in T item) {
        return IndexOf(item, null) >= 0;
    }

    /// <summary>
    ///     Determines whether an element is in the list.
    /// </summary>
    /// <param name="item">The object to locate in the list. The value can be null for reference types.</param>
    /// <param name="comparer">The comparer used to determine whether two items are equal.</param>
    /// <returns><see langword="true"/> if item is found in the list; otherwise, <see langword="false"/>.</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public bool Contains(in T item, IEqualityComparer<T>? comparer) {
        return IndexOf(item, comparer) >= 0;
    }

    /// <inheritdoc cref="Span{T}.CopyTo"/>
    public void CopyTo(Span<T> destination) {
        _storage.CopyTo(destination);
    }

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public int IndexOf(in T item) {
        return IndexOf(item, null);
    }

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [Pure]
    public int IndexOf(in T item, IEqualityComparer<T>? comparer) {
        if (comparer is null) {
            if (typeof(T).IsValueType) {
                for (int i = 0; i < _pos; i++) {
                    if (!EqualityComparer<T>.Default.Equals(item, _storage[i])) {
                        continue;
                    }

                    return i;
                }

                return -1;
            }

            comparer = EqualityComparer<T>.Default;
        }

        for (int i = 0; i < _pos; i++) {
            if (!comparer.Equals(item, _storage[i])) {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc cref="List{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, in T value) {
        if (_pos > _storage.Length - 1) {
            Grow(1);
        }

        int remaining = _pos - index;
        _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + 1));
        _storage[index] = value;
        _pos += 1;
    }

    /// <inheritdoc cref="List{T}.InsertRange"/>
    public void InsertRange(int index, ReadOnlySpan<T> span) {
        ThrowHelper.ArgumentInRange(index, index >= 0);
        ThrowHelper.ArgumentInRange(index, index <= Length);
        int count = span.Length;

        if (_pos > _storage.Length - count) {
            Grow(count);
        }

        int remaining = _pos - index;
        _storage.Slice(index, remaining).CopyTo(_storage.Slice(index + count));
        span.CopyTo(_storage.Slice(index));
        _pos += count;
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public int LastIndexOf(in T item) {
        return LastIndexOf(item, null);
    }

    /// <inheritdoc cref="List{T}.LastIndexOf(T)"/>
    [Pure]
    public int LastIndexOf(in T item, IEqualityComparer<T>? comparer) {
        if (comparer is null) {
            if (typeof(T).IsValueType) {
                for (int i = _pos - 1; i >= 0; i--) {
                    if (!EqualityComparer<T>.Default.Equals(item, _storage[i])) {
                        continue;
                    }

                    return i;
                }

                return -1;
            }

            comparer = EqualityComparer<T>.Default;
        }

        for (int i = _pos - 1; i >= 0; i--) {
            if (!comparer.Equals(item, _storage[i])) {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc cref="List{T}.Remove"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(in T item) {
        return Remove(item, null);
    }

    /// <inheritdoc cref="List{T}.Remove"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(in T item, IEqualityComparer<T>? comparer) {
        int index = IndexOf(item, comparer);
        if (index >= 0) {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="List{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAt(int index) {
        int remaining = _pos - index - 1;
        _storage.Slice(index + 1, remaining).CopyTo(_storage.Slice(index));
        _storage[--_pos] = default!;
    }

    /// <inheritdoc cref="List{T}.RemoveRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveRange(int start, int count) {
        GuardRange(start, count);

        int end = _pos - count;
        int remaining = end - start;
        _storage.Slice(start + count, remaining).CopyTo(_storage.Slice(start));

        if (_arrayToReturnToPool is not null) {
            Array.Clear(_arrayToReturnToPool, end, count);
        } else {
            _storage.Slice(end).Clear();
        }

        _pos = end;
    }

    private void GuardRange(int start, int count) {
        ThrowHelper.ArgumentInRange(start, start >= 0);
        ThrowHelper.ArgumentInRange(count, count >= 0);
        ThrowHelper.ArgumentInRange(count, start <= Length - count);
    }

    /// <inheritdoc cref="List{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reverse() {
        _storage.Slice(0, _pos).Reverse();
    }

    /// <inheritdoc cref="List{T}.Reverse(Int32, Int32)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reverse(int start, int count) {
        GuardRange(start, count);
        _storage.Slice(start, count).Reverse();
    }

    /// <inheritdoc cref="List{T}.Sort()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort() {
        _storage.Slice(0, _pos).Sort();
    }

    /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(Comparison<T> comparison) {
        _storage.Slice(0, _pos).Sort(comparison);
    }

    /// <inheritdoc cref="List{T}.Sort(IComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(IComparer<T> comparer) {
        _storage.Slice(0, _pos).Sort(comparer);
    }

    /// <inheritdoc cref="List{T}.Sort(Int32, Int32, IComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(int start, int count, IComparer<T> comparer) {
        GuardRange(start, count);
        _storage.Slice(start, count).Sort(comparer);
    }

    /// <inheritdoc cref="Span{T}.ToArray"/>
    public T[] ToArray() {
        T[]? array = new T[_pos];
        AsSpan().CopyTo(array.AsSpan());
        return array;
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> from a <see cref="RefVec{T}"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> that contains elements form the input sequence.</returns>
    public List<T> ToList() {
        List<T> list = new(_pos);
        for (int i = 0; i < _pos; i++) {
            list.Add(_storage[i]);
        }

        return list;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn is not null) {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(in T value) {
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
    private void Grow(int additionalCapacityBeyondPos) {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(
            _pos > _storage.Length - additionalCapacityBeyondPos,
            "Grow called incorrectly, no resize is needed."
        );

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
        T[]? poolArray = ArrayPool<T>.Shared.Rent((_pos + additionalCapacityBeyondPos).Max(_storage.Length * 2));

        AsSpan().CopyTo(poolArray);

        T[]? toReturn = _arrayToReturnToPool;
        _storage = _arrayToReturnToPool = poolArray;
        if (toReturn is not null) {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    private string GetDebuggerDisplay() {
        if (Length == 0) {
            return "Length = 0, Values = []";
        }

        StringBuilder sb = new(256);
        sb.Append("Length = ").Append(_pos);
        sb.Append(", Values = [");
        if (Length < 10) {
            int last = _pos - 1;
            for (int i = 0; i < last; i++) {
                sb.Append(_storage[i]);
                sb.Append(", ");
            }

            if (Length > 0) {
                sb.Append(_storage[last]);
            }
        } else {
            sb.Append("...");
        }

        sb.Append(']');
        return sb.ToString();
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() {
        return new(this);
    }

    /// <summary>Enumerates the elements of a <see cref="RefVec{T}"/>.</summary>
    public ref struct Enumerator {
        private RefVec<T> _list;
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="list">The list to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(RefVec<T> list) {
            _list = list;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            int index = _index + 1;
            if ((uint) index < (uint) _list.Length) {
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

/// <summary>Collection of extensions and utility functions related to <see cref="RefVec{T}"/>.</summary>
public static class RefVecExtensions {
    /// <summary>
    /// Determines whether two lists are equal by comparing the elements using IEquatable{T}.Equals(T).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static bool SequenceEqual<T>(this RefVec<T> list, RefVec<T> other)
        where T : IEquatable<T> {
        int count = list.Length;
        if (count != other.Length) {
            return false;
        }

        return list.AsSpan().SequenceEqual(other.AsSpan());
    }

    /// <summary>
    /// Determines the relative order of the lists being compared by comparing the elements using IComparable{T}.CompareTo(T).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int SequenceCompareTo<T>(this RefVec<T> list, RefVec<T> other)
        where T : IComparable<T> {
        return list.AsSpan().SequenceCompareTo(other.AsSpan());
    }

    internal static int SequenceCompareHelper<T>(
        ref T first,
        int firstLength,
        ref T second,
        int secondLength,
        IComparer<T> comparer) {
        Debug.Assert(firstLength >= 0);
        Debug.Assert(secondLength >= 0);

        int minLength = firstLength;
        if (minLength > secondLength) {
            minLength = secondLength;
        }

        for (int i = 0; i < minLength; i++) {
            int result = comparer.Compare(Unsafe.Add(ref first, i), Unsafe.Add(ref second, i));
            if (result != 0) {
                return result;
            }
        }

        return firstLength.CompareTo(secondLength);
    }
}
