using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Memory;

/// <summary>Partially inlined mutable collection of items.</summary>
/// <remarks>Use with care, because <see cref="TinyVec{T}"/> is a struct, and thus requires a reference to mutate.</remarks>
/// <example>A usage example is usage as <see cref="Dictionary{TKey,TValue}"/> values when interacting via the CollectionsMarshal methods.</example>
public struct TinyVec<T> : IReadOnlyList<T>, IList<T> {
    [AllowNull] private T _singleValue;
    private Vec<T>? _values;

    /// Always empty list indicating that the TinyVec has exactly one element in _singleValue.
    /// The guard is used in this manner to ensure that `TinyVec{T} v = default` is valid.
    private static readonly Vec<T> SingleValueGuard = new();


    /// <summary>
    /// Initializes a new <see cref="TinyVec{T}"/> with a single value.
    /// </summary>
    /// <param name="item">The single item populating this list</param>
    public TinyVec(T item) {
        _singleValue = item;
        _values = SingleValueGuard;
    }

    /// <summary>
    /// Initializes a new <see cref="TinyVec{T}"/> with the specified capacity.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    public TinyVec(int capacity) {
        _singleValue = default;
        _values = capacity >= 2 ? new(capacity) : default;
    }

    /// <inheritdoc cref="IReadOnlyVector{T}.IsEmpty"/>
    public bool IsEmpty => Count == 0;

    bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Number of entries in this collections.
    /// </summary>
    public int Count => ReferenceEquals(_values, SingleValueGuard) ? 1 : _values?.Length ?? 0;

    /// <summary>Returns the maximum number of elements the vector can hold before resizing.</summary>
    public int Capacity => ReferenceEquals(_values, SingleValueGuard) ? 1 : _values?.Capacity ?? 1;

    /// <inheritdoc />
    public T this[int index] {
        get {
            if (ReferenceEquals(_values, SingleValueGuard))
            {
                ThrowHelper.ArgumentInRange(index, index == 0);
                return _singleValue;
            }
            if (_values is not null) {
                // this will throw since the list is EmptyListGuard
                return _values[index];
            }
            ThrowHelper.ArgumentInRange(index, index>=0 && index < Count);
            Debug.Fail("Unreachable, expected to throw above.");
            return default!;
        }
        set {
            if (ReferenceEquals(_values, SingleValueGuard))
            {
                ThrowHelper.ArgumentInRange(index, index == 0);
                _singleValue = value;
            }
            if (_values is not null) {
                // this will throw since the list is EmptyListGuard
                _values[index] = value;
                return;
            }

            ThrowHelper.ArgumentInRange(index, index>=0 && index < Count);
            Debug.Fail("Unreachable, expected to throw above.");
        }
    }

    /// <inheritdoc cref="Vec{T}.GetEnumerator" />
    public ReadOnlySpan<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    /// <inheritdoc />
    public void Add(T value)
    {
        if (ReferenceEquals(_values, SingleValueGuard))
        {
            // convert to a list boxing _singleValue and _values
            _values = new() { _singleValue, value };
            _singleValue = default; // set to default so that the GC may collect if T is a reference type.
            return;
        }
        if (_values is not null) {
            _values.Add(value);
            return;
        }

        _values = SingleValueGuard;
        _singleValue = value;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    public void AddRange(ReadOnlySpan<T> values) => InsertRange(Count, values);

    /// <inheritdoc />
    public bool Remove(T value)
    {
        int index = IndexOf(value);
        if (index == -1)
        {
            return false;
        }
        RemoveAt(index);
        return true;
    }

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        if (ReferenceEquals(_values, SingleValueGuard))
        {
            return EqualityComparer<T>.Default.Equals(_singleValue, item) ? 0 : -1;
        }

        if (_values is not null) {
            return _values.IndexOf(item);
        }

        return -1;
    }

    /// <inheritdoc />
    public void Insert(int index, T item) {
        if (ReferenceEquals(_values, SingleValueGuard)) {
            ThrowHelper.ArgumentInRange(index, index is >= 0 and <= 1);
            _values = index == 0 ? new() { item, _singleValue } : new() { _singleValue, item };
            _singleValue = default;
            return;
        }

        if (_values is not null) {
            _values.Insert(index, item);
            return;
        }

        ThrowHelper.ArgumentInRange(index, index == 0);
        _values = SingleValueGuard;
        _singleValue = item;
    }

    /// <inheritdoc cref="IVector{T}.InsertRange"/>
    public void InsertRange(int index, ReadOnlySpan<T> values) {
        if (values.IsEmpty) {
            return;
        }
        if (values.Length == 1) {
            Insert(index, values[0]);
            return;
        }
        // we will always need to construct a list, bc we will have at least two elements.
        // other conditions are handled above
        if (ReferenceEquals(_values, SingleValueGuard)) {
            ThrowHelper.ArgumentInRange(index, index is >= 0 and <= 1);
            _values = new(values.Length + 1);
            if (index == 0) {
                _values.Add(_singleValue);
            }
            _values.AddRange(values);
            if (index != 0) {
                _values.Add(_singleValue);
            }

            _singleValue = default;
            return;
        }

        if (_values is not null) {
            _values.InsertRange(index, values);
            return;
        }

        ThrowHelper.ArgumentInRange(index, index == 0);
        _values = new();
        _values.AddRange(values);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if (ReferenceEquals(_values, SingleValueGuard))
        {
            ThrowHelper.ArgumentInRange(index, index == 0);
            _values = default;
            _singleValue = default;
            return;
        }

        if (_values is not null) {
            _values.RemoveAt(index);
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index), index);
    }

    /// <summary>Removes the element at the specified <paramref name="index"/> from the vector by over-writing it with the last element.</summary>
    /// <remarks>No block of elements in moved. The order of the vector is disturbed.</remarks>
    public void SwapRemove(int index) {
        if (ReferenceEquals(_values, SingleValueGuard)) {
            ThrowHelper.ArgumentInRange(index, index == 0);
            _values = default;
            _singleValue = default;
            return;
        }

        if (_values is not null) {
            _values.SwapRemove(index);
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index), index);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (_values is not null && !ReferenceEquals(_values, SingleValueGuard)) {
            _values.Clear();
            return;
        }

        _values = default;
        _singleValue = default;
    }

    /// <inheritdoc />
    public bool Contains(T item) => IndexOf(item) != -1;

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) {
        if (!TryCopyTo(array.AsSpan(arrayIndex))) {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(array), array);
        }
    }

    /// <inheritdoc cref="IReadOnlyVector{T}.TryCopyTo(Span{T})"/>
    public bool TryCopyTo(Span<T> span) {
        if (span.Length < Count) {
            return false;
        }
        switch (_values) {
        case null:
            return true;
        case T existing:
            span[0] = existing;
            return true;
        default:
            return _values.TryCopyTo(span);
        }
    }

    /// <summary>
    /// Returns a span view over the values in the collection.
    /// </summary>
    /// <param name="start">The start index.</param>
    /// <param name="length">The length of the span.</param>
    /// <remarks>Do not modify the length of the collection while handling the span!</remarks>
    public ReadOnlySpan<T> AsSpan(int start, int length)
    {
        ThrowHelper.ArgumentInRange(start, start >= 0);
        ThrowHelper.ArgumentInRange(length, length >= 0 && start + length <= Count);
        if (_values is null)
        {
            return default;
        }
        if (_values is T existing)
        {
#if NETSTANDARD2_1_OR_GREATER ||NET5_0_OR_GREATER
            return MemoryMarshal.CreateReadOnlySpan(ref existing, 1);
#else
            // box the entry in a vector and replace _entry with the box
            Vec<T> entryBox = new(1) { existing };
            _values = entryBox;
            Debug.Assert(entryBox.Length == length);
            return entryBox.AsSpan();
#endif
        }

        Vec<T> list = Unsafe.As<Vec<T>>(_values);
        return list.AsSpan(start, length);
    }

    /// <inhertidoc cref="AsSpan(int, int)"/>
    public ReadOnlySpan<T> AsSpan(int start) => AsSpan(start, Count - start);

    /// <inhertidoc cref="AsSpan(int, int)"/>
    public ReadOnlySpan<T> AsSpan() => AsSpan(0, Count);

    private sealed class Enumerator : IEnumerator<T> {
        private readonly TinyVec<T> _source;
        private int _index = -1;

        public Enumerator(TinyVec<T> source) {
            _source = source;
        }

        public T Current => _source[_index];

        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            int index = _index + 1;

            if ((nuint)index >= (nuint)_source.Count)
            {
                _index = -1;
                return false;
            }

            _index = index;
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            _index = -2;
        }
    }
}
