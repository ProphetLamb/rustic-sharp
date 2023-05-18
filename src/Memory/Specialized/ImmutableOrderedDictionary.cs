using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rustic.Memory.Specialized;

/// <summary>Represents an immutable dictionary accessible by insertion index.</summary>
/// <typeparam name="K">The type of the keys.</typeparam>
/// <typeparam name="V">The type of the values.</typeparam>
public sealed class ImmutableOrderedDictionary<K, V> : IReadOnlyDictionary<K, V>, IDictionary<K, V>
    where K : notnull {
    internal readonly IEqualityComparer<K>? _keyComparer;
    private KeyValuePair<K, V>[]? _entries;
    private int _count;
    private ICollection<K>? _keyCollectionBoxed;
    private ICollection<V>? _valueCollectionBoxed;

    /// <summary>Initializes a new empty <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    internal ImmutableOrderedDictionary() : this(null) {
    }

    /// <summary>Initializes a new empty <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> with the specified <paramref name="keyComparer"/>.</summary>
    public ImmutableOrderedDictionary(IEqualityComparer<K>? keyComparer) {
        _keyComparer = keyComparer;
    }

    /// <summary>Initializes a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> from the specified <paramref name="entries"/>.</summary>
    public ImmutableOrderedDictionary(
        IEnumerable<KeyValuePair<K, V>> entries,
        IEqualityComparer<K>? keyComparer = null) : this(keyComparer) {
        foreach (KeyValuePair<K, V> entry in entries) {
            Add(entry);
        }
    }

    internal ImmutableOrderedDictionary(
        KeyValuePair<K, V>[] entries,
        int count,
        IEqualityComparer<K>? keyComparer = null) : this(keyComparer) {
        _entries = entries;
        _count = count;
    }

    /// <summary>Gets the empty <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public static ImmutableOrderedDictionary<K, V> Empty { get; } = new();

    private Span<KeyValuePair<K, V>> Entries => _entries.AsSpan(0, _count);

    private void EnsureCapacity(int capacity) {
        if (_entries is null) {
            _entries = new KeyValuePair<K, V>[capacity];
        } else if (_entries.Length < capacity) {
            KeyValuePair<K, V>[] newEntries = new KeyValuePair<K, V>[capacity];
            Entries.CopyTo(newEntries);
            _entries = newEntries;
        }
    }

    internal void Add(KeyValuePair<K, V> item) {
        ref KeyValuePair<K, V> entry = ref GetOrCreateUninitializedRef(item.Key, out int indexIfExists);
        if (indexIfExists == -1) {
            entry = item;
            return;
        }

        throw new ArgumentException("An item with the same key has already been added.", nameof(item.Key));
    }

    internal unsafe ref KeyValuePair<K, V> TryGetEntry(K key, out int indexIfExists) {
        ref KeyValuePair<K, V> entry = ref Unsafe.AsRef<KeyValuePair<K, V>>(null);
        if (_keyComparer is not { } keyComparer) {
            if (typeof(K).IsValueType) {
                for (indexIfExists = 0; indexIfExists < _count; indexIfExists++) {
                    ref KeyValuePair<K, V> cur = ref _entries![indexIfExists];
                    if (EqualityComparer<K>.Default.Equals(cur.Key, key)) {
                        return ref cur;
                    }
                }

                indexIfExists = -1;
                return ref entry;
            }

            keyComparer = EqualityComparer<K>.Default;
        }

        for (indexIfExists = 0; indexIfExists < _count; indexIfExists++) {
            ref KeyValuePair<K, V> cur = ref _entries![indexIfExists];
            if (keyComparer.Equals(cur.Key, key)) {
                return ref cur;
            }
        }

        indexIfExists = -1;
        return ref entry;
    }

    internal ref KeyValuePair<K, V> GetOrCreateUninitializedRef(K key, out int index) {
        ref KeyValuePair<K, V> entry = ref TryGetEntry(key, out index);
        if (index == -1) {
            int nextCount = _count + 1;
            EnsureCapacity(nextCount);
            entry = ref _entries![_count];
            _count = nextCount;
        }

        return ref entry;
    }

    /// <inheritdoc />
    public V this[K key] {
        get {
            ref KeyValuePair<K, V> entry = ref TryGetEntry(key, out int indexIfExists);
            if (indexIfExists == -1) {
                throw new KeyNotFoundException();
            }

            return entry.Value;
        }
    }

    /// <summary>Gets an <see cref="OrderedIndexer"/> that can be used to access the dictionary by insertion index.</summary>
    public OrderedIndexer Indexer => new(this);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
    public KeyCollection Keys => new(this);

    IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => _keyCollectionBoxed ??= Keys;
    ICollection<K> IDictionary<K, V>.Keys => _keyCollectionBoxed ??= Keys;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Values"/>
    public ValueCollection Values => new(this);

    IEnumerable<V> IReadOnlyDictionary<K, V>.Values => _valueCollectionBoxed ??= Values;

    ICollection<V> IDictionary<K, V>.Values => _valueCollectionBoxed ??= Values;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    public int Count => _count;

    bool ICollection<KeyValuePair<K, V>>.IsReadOnly => true;

    V IDictionary<K, V>.this[K key] {
        get => this[key];
        set => throw new NotSupportedException();
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey" />
    public bool ContainsKey(K key) {
        TryGetEntry(key, out int indexIfExists);
        return indexIfExists != -1;
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue" />
    public bool TryGetValue(K key, out V value) {
        ref KeyValuePair<K, V> entry = ref TryGetEntry(key, out int indexIfExists);
        if (indexIfExists == -1) {
            value = default!;
            return false;
        }

        value = entry.Value;
        return true;
    }

    /// <summary>Gets a <see cref="ReadOnlySpan{T}"/> of the entries.</summary>
    public ReadOnlySpan<KeyValuePair<K, V>> AsSpan() {
        return Entries;
    }

    /// <inheritdoc />
    public Enumerator GetEnumerator() {
        return new(this);
    }

    IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    void IDictionary<K, V>.Add(K key, V value) {
        throw new NotSupportedException();
    }

    bool IDictionary<K, V>.Remove(K key) {
        throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) {
        throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<K, V>>.Clear() {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<K, V> item) {
        ref KeyValuePair<K, V> entry = ref TryGetEntry(item.Key, out int indexIfExists);
        return indexIfExists != -1 && EqualityComparer<V>.Default.Equals(entry.Value, item.Value);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
        Entries.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <summary>Copies the entries to the specified <paramref name="span"/>.</summary>
    /// <param name="span">The span to copy to.</param>
    public void CopyTo(Span<KeyValuePair<K, V>> span) {
        Entries.CopyTo(span);
    }

    bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item) {
        throw new NotSupportedException();
    }

    /// <summary>Enumerates the entries of an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public struct Enumerator : IEnumerator<KeyValuePair<K, V>> {
        private readonly ImmutableOrderedDictionary<K, V> _dictionary;
        private int _index;

        internal Enumerator(ImmutableOrderedDictionary<K, V> dictionary) {
            _dictionary = dictionary;
            _index = -1;
        }

        /// <inheritdoc />
        public KeyValuePair<K, V> Current => _dictionary._entries![_index];

        object? IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose() {
        }

        /// <inheritdoc />
        public bool MoveNext() {
            return ++_index < _dictionary._count;
        }

        /// <inheritdoc />
        public void Reset() {
            _index = -1;
        }
    }

    /// <summary>Represents a collection of keys in an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public readonly struct KeyCollection : IReadOnlyCollection<K>, ICollection<K> {
        private readonly ImmutableOrderedDictionary<K, V> _dictionary;

        internal KeyCollection(ImmutableOrderedDictionary<K, V> dictionary) {
            _dictionary = dictionary;
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
        public int Count => _dictionary.Count;

        bool ICollection<K>.IsReadOnly => true;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public Enumerator GetEnumerator() {
            return new Enumerator(_dictionary);
        }

        void ICollection<K>.Add(K item) {
            throw new NotSupportedException();
        }

        void ICollection<K>.Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(K item) {
            return _dictionary.ContainsKey(item);
        }

        /// <inheritdoc/>
        public void CopyTo(K[] array, int arrayIndex) {
            CopyTo(array.AsSpan(arrayIndex));
        }

        /// <summary>Copies the keys to the specified <paramref name="span"/>.</summary>
        /// <param name="span">The span to copy to.</param>
        public void CopyTo(Span<K> span) {
            int spanIndex = 0;
            foreach (K key in this) {
                span[spanIndex++] = key;
            }
        }

        IEnumerator<K> IEnumerable<K>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        bool ICollection<K>.Remove(K item) {
            throw new NotSupportedException();
        }

        /// <summary>Enumerates the keys of an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
        public struct Enumerator : IEnumerator<K> {
            private ImmutableOrderedDictionary<K, V>.Enumerator _en;

            internal Enumerator(ImmutableOrderedDictionary<K, V> dictionary) {
                _en = dictionary.GetEnumerator();
            }

            /// <inheritdoc/>
            public K Current => _en.Current.Key;

            object? IEnumerator.Current => Current;

            /// <inheritdoc/>
            public void Dispose() {
                _en.Dispose();
            }

            /// <inheritdoc/>
            public bool MoveNext() {
                return _en.MoveNext();
            }

            /// <inheritdoc/>
            public void Reset() {
                _en.Reset();
            }
        }
    }

    /// <summary>Represents a collection of values in an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public readonly struct ValueCollection : IReadOnlyCollection<V>, ICollection<V> {
        private readonly ImmutableOrderedDictionary<K, V> _dictionary;

        internal ValueCollection(ImmutableOrderedDictionary<K, V> dictionary) {
            _dictionary = dictionary;
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
        public int Count => _dictionary.Count;

        bool ICollection<V>.IsReadOnly => true;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public Enumerator GetEnumerator() {
            return new Enumerator(_dictionary);
        }

        void ICollection<V>.Add(V item) {
            throw new NotSupportedException();
        }

        void ICollection<V>.Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(V item) {
            foreach (V value in this) {
                if (EqualityComparer<V>.Default.Equals(value, item)) {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void CopyTo(V[] array, int arrayIndex) {
            CopyTo(array.AsSpan(arrayIndex));
        }

        /// <summary>Copies the values to the specified <paramref name="span"/>.</summary>
        /// <param name="span">The span to copy to.</param>
        public void CopyTo(Span<V> span) {
            int spanIndex = 0;
            foreach (V value in this) {
                span[spanIndex++] = value;
            }
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        bool ICollection<V>.Remove(V item) {
            throw new NotSupportedException();
        }

        /// <summary>Enumerates the values of an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
        public struct Enumerator : IEnumerator<V> {
            private ImmutableOrderedDictionary<K, V>.Enumerator _en;

            internal Enumerator(ImmutableOrderedDictionary<K, V> dictionary) {
                _en = dictionary.GetEnumerator();
            }

            /// <inheritdoc/>
            public V Current => _en.Current.Value;

            object? IEnumerator.Current => Current;

            /// <inheritdoc/>
            public void Dispose() {
                _en.Dispose();
            }

            /// <inheritdoc/>
            public bool MoveNext() {
                return _en.MoveNext();
            }

            /// <inheritdoc/>
            public void Reset() {
                _en.Reset();
            }
        }
    }

    /// <summary>Represents an indexer for an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public readonly struct OrderedIndexer : IReadOnlyList<KeyValuePair<K, V>>, ICollection<KeyValuePair<K, V>> {
        private readonly ImmutableOrderedDictionary<K, V> _dictionary;

        internal OrderedIndexer(ImmutableOrderedDictionary<K, V> dictionary) {
            _dictionary = dictionary;
        }

        /// <inheritdoc/>
        public KeyValuePair<K, V> this[int index] => _dictionary._entries![index];

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly => true;

        /// <inheritdoc/>
        public Enumerator GetEnumerator() {
            return new Enumerator(_dictionary);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<K, V>>.Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<K, V> item) {
            return _dictionary.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            _dictionary.Entries.CopyTo(array.AsSpan(arrayIndex));
        }

        /// <summary>Copies the entries to the specified <paramref name="span"/>.</summary>
        /// <param name="span">The span to copy to.</param>
        public void CopyTo(Span<KeyValuePair<K, V>> span) {
            _dictionary.Entries.CopyTo(span);
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item) {
            throw new NotSupportedException();
        }
    }

    /// <summary>Represents a builder for an <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</summary>
    public sealed class Builder : IReadOnlyDictionary<K, V>, IDictionary<K, V>, IDisposable {
        private ImmutableOrderedDictionary<K, V>? _dictionary;

        /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> with the specified <paramref name="keyComparer"/>.</summary>
        /// <param name="keyComparer"></param>
        public Builder(IEqualityComparer<K>? keyComparer = null) {
            _dictionary = new(keyComparer);
        }

        /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> from the specified <paramref name="entries"/>.</summary>
        /// <param name="entries">The entries to copy.</param>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{T}"/> to use for comparing keys.</param>
        public Builder(IEnumerable<KeyValuePair<K, V>> entries, IEqualityComparer<K>? keyComparer = null) {
            _dictionary = new(entries, keyComparer);
        }

        /// <summary>Adds a new entry to the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/>.</summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add.</param>
        public void Add(K key, V value) {
            (_dictionary ??= new()).Add(new KeyValuePair<K, V>(key, value));
        }

        /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> from the entries in the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> and clears the builder.</summary>
        /// <returns>The new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>Usually the data is not copied.</remarks>
        public ImmutableOrderedDictionary<K, V> MoveToImmutable() {
            ImmutableOrderedDictionary<K, V> dictionary = _dictionary ?? Empty;
            Dispose();
            return dictionary;
        }

        /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> from the entries in the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/>.</summary>
        /// <returns>The new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>The data is copied.</remarks>
        /// <remarks>The builder is not cleared.</remarks>
        public ImmutableOrderedDictionary<K, V> ToImmutable() {
            if (_dictionary is not null) {
                return new(_dictionary, _dictionary._keyComparer);
            }

            return Empty;
        }

        /// <inheritdoc />
        public void Dispose() {
            _dictionary = default;
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Values" />
        public KeyCollection Keys => _dictionary?.Keys ?? default;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Values" />
        public ValueCollection Values => _dictionary?.Values ?? default;

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
        public int Count => _dictionary?.Count ?? 0;

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Keys;

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Values;

        ICollection<K> IDictionary<K, V>.Keys => Keys;

        ICollection<V> IDictionary<K, V>.Values => Values;

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly => throw new NotImplementedException();

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.this" />
        public V this[K key] {
            get => _dictionary is not null ? _dictionary[key] : throw new KeyNotFoundException();
            set {
                ref KeyValuePair<K, V> entry = ref (_dictionary ??= new()).GetOrCreateUninitializedRef(key, out _);
                entry = new KeyValuePair<K, V>(key, value);
            }
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey" />
        public bool ContainsKey(K key) {
            return _dictionary?.ContainsKey(key) ?? false;
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue" />
        public bool TryGetValue(K key, out V value) {
            bool res = false;
            if (_dictionary is not null && _dictionary.TryGetValue(key, out value)) {
                res = true;
            } else {
                value = default!;
            }

            return res;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public Enumerator GetEnumerator() {
            return _dictionary?.GetEnumerator() ?? default;
        }

        /// <summary>Removes the entry at the specified <paramref name="index"/>.</summary>
        /// <param name="index">The index of the entry to remove.</param>
        /// <exception cref="IndexOutOfRangeException">The specified <paramref name="index"/> is out of range.</exception>
        public void RemoveAt(int index) {
            if (_dictionary is not null && (uint) index < (uint) _dictionary._count) {
                Span<KeyValuePair<K, V>> span = _dictionary.Entries;
                span.Slice(index + 1).CopyTo(span.Slice(index));
                _dictionary._count--;
                return;
            }

            throw new IndexOutOfRangeException();
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<K, V> item) {
            (_dictionary ??= new()).Add(item);
        }

        /// <inheritdoc />
        public bool Remove(K key) {
            bool res = false;
            if (_dictionary is not null) {
                ref KeyValuePair<K, V> entry = ref _dictionary.TryGetEntry(key, out int indexIfExists);
                if (indexIfExists != -1) {
                    RemoveAt(indexIfExists);
                    res = true;
                }
            }

            return res;
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<K, V> item) {
            bool res = false;
            if (_dictionary is not null) {
                ref KeyValuePair<K, V> entry = ref _dictionary.TryGetEntry(item.Key, out int indexIfExists);
                if (indexIfExists != -1 && EqualityComparer<V>.Default.Equals(entry.Value, item.Value)) {
                    RemoveAt(indexIfExists);
                    res = true;
                }
            }

            return res;
        }

        /// <inheritdoc />
        public void Clear() {
            if (_dictionary is not null) {
                _dictionary.Entries.Clear();
                _dictionary._count = 0;
            }
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<K, V> item) {
            bool res = false;
            if (_dictionary is not null) {
                res = _dictionary.Contains(item);
            }

            return res;
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            _dictionary?.Entries.CopyTo(array.AsSpan(arrayIndex));
        }

        /// <summary>Copies the entries to the specified <paramref name="span"/>.</summary>
        /// <param name="span">The span to copy the entries to.</param>
        public void CopyTo(Span<KeyValuePair<K, V>> span) {
            _dictionary?.Entries.CopyTo(span);
        }

        /// <summary>Sorts the entries in the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> using the default</summary>
        public void Sort() {
            _dictionary?.Entries.Sort();
        }

        /// <summary>Sorts the entries in the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> using the specified <paramref name="comparer"/>.</summary>
        /// <param name="comparer">The comparer to use for sorting.</param>
        public void Sort(IComparer<KeyValuePair<K, V>> comparer) {
            _dictionary?.Entries.Sort(comparer);
        }

        /// <summary>Sorts the entries in the <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/> using the specified <paramref name="comparison"/>.</summary>
        /// <param name="comparison">The comparison to use for sorting.</param>
        public void Sort(Comparison<KeyValuePair<K, V>> comparison) {
            _dictionary?.Entries.Sort(comparison);
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}

/// <summary>Allows for easier construction of <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> annd <see cref="ImmutableOrderedDictionary{TKey, TValue}.Builder"/>.</summary>
public static class ImmutableOrderedDictionary {
    /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> with the specified <paramref name="keyComparer"/>.</summary>
    /// <param name="keyComparer">The <see cref="IEqualityComparer{T}"/> to use for comparing keys.</param>
    public static ImmutableOrderedDictionary<K, V>.Builder CreateBuilder<K, V>(IEqualityComparer<K>? keyComparer = null)
        where K : notnull {
        return new ImmutableOrderedDictionary<K, V>.Builder(keyComparer);
    }
}

/// <summary>
/// Provides collection methods for <see cref="ImmutableOrderedDictionary{TKey, TValue}"/>.
/// </summary>
public static class ImmutableOrderedDictionaryMarshal {
    /// <summary>Initializes a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> from the specified <paramref name="array"/> without copying the array.</summary>
    /// <param name="array">The array.</param>
    /// <param name="count">The number of items from 0 to <paramref name="array"/>.Length to use.</param>
    /// <param name="keyComparer">The <see cref="IEqualityComparer{T}"/> to use for comparing keys.</param>
    public static ImmutableOrderedDictionary<K, V> CreateFromPinnedArray<K, V>(
        KeyValuePair<K, V>[] array,
        int count,
        IEqualityComparer<K>? keyComparer = null)
        where K : notnull {
        return new(array, count, keyComparer);
    }

    /// <summary>Returns the reference to the entry with the specified <paramref name="key"/> if it exists; otherwise returns a null-reference.</summary>
    /// <param name="dict">The dictionary.</param>
    /// <param name="key">The key of the entry to get.</param>
    /// <param name="indexIfExists">The index of the entry if it exists; otherwise -1.</param>
    public static ref readonly KeyValuePair<K, V> TryGetEntry<K, V>(
        ImmutableOrderedDictionary<K, V> dict,
        K key,
        out int indexIfExists)
        where K : notnull {
        return ref dict.TryGetEntry(key, out indexIfExists);
    }

    /// <summary>Creates a new <see cref="ImmutableOrderedDictionary{TKey, TValue}"/> from the specified <paramref name="entries"/>.</summary>
    /// <param name="entries">The entries to use.</param>
    /// <param name="keyComparer">The <see cref="IEqualityComparer{T}"/> to use for comparing keys.</param>
    public static ImmutableOrderedDictionary<K, V> ToImmutableOrderedDictionary<K, V>(
        this IEnumerable<KeyValuePair<K, V>> entries,
        IEqualityComparer<K>? keyComparer = null)
        where K : notnull {
        if (entries is ImmutableOrderedDictionary<K, V> dict && dict._keyComparer == keyComparer) {
            return dict;
        }

        return new(entries, keyComparer);
    }
}
