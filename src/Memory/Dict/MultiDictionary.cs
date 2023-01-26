using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Memory;

/// <summary>A dictionary that can hold more than one distinct value with the same key.</summary>
/// <typeparam name="K">Type of key</typeparam>
/// <typeparam name="V">Type of value</typeparam>
/// <remarks>If only insertions, and no removals are performed the order of values for a key is stable; otherwise the order may change.</remarks>
/// <remarks>For .NET 6.0 or greater this implementation requires no additional object, or array allocation for 1-1 key-value relations.</remarks>
[DebuggerDisplay("Keys={KeyCount} Values={ValueCount}"), DebuggerTypeProxy(typeof(MultiDictionaryDebugView<,>))]
public sealed class MultiDictionary<K, V> : IEnumerable<KeyValuePair<K, IReadOnlyCollection<V>>>
    where K : notnull {
    /// <summary>Backing dictionary.</summary>
#if NET6_0_OR_GREATER
    private readonly Dictionary<K, TinyVec<V>> _backing;
#else
    private readonly Dictionary<K, Vec<V>> _backing;
#endif

    private int _keyCount;
    private int _valueCount;

    /// <summary>Initializes a new instance of <see cref="MultiDictionary{K,V}"/>.</summary>
    public MultiDictionary() : this(0, null) {}

    /// <summary>Initializes a new instance of <see cref="MultiDictionary{K,V}"/>.</summary>
    /// <param name="capacity">The initial key capacity of the dictionary.</param>
    /// <param name="keyComparer">The comparer used to determine if keys are equal.</param>
    public MultiDictionary(int capacity, IEqualityComparer<K>? keyComparer) {
        _backing = new(capacity, keyComparer);
        Keys = new(this);
        Values = new(this);
    }

    /// <summary>Number of keys</summary>
    public int KeyCount => _keyCount;

    /// <summary>Number of values over all keys</summary>
    public int ValueCount => _valueCount;

    /// <summary>A collection of all keys in the <see cref="MultiDictionary{K,V}"/></summary>
    public KeyCollection Keys { get; }

    /// <summary>A collection of values in the <see cref="MultiDictionary{K,V}"/></summary>
    public ValueCollection Values { get; }

    /// <summary>Span over values that have the specified key, empty if the key is not present.</summary>
    public ReadOnlySpan<V> this[K key] {
        get {
#if NET6_0_OR_GREATER
            ref TinyVec<V> entry = ref CollectionsMarshal.GetValueRefOrAddDefault(_backing, key, out bool exists);
            return exists ? entry.AsSpan() : default;
#else
            return _backing.TryGetValue(key, out Vec<V> entry) ? entry.AsSpan() : default;
#endif
        }
    }

    /// <summary>
    /// Add a single value under the specified key.
    /// Value may not be null.
    /// </summary>
    public void Add(K key, V value) {
#if NET6_0_OR_GREATER
        ref TinyVec<V> entry = ref CollectionsMarshal.GetValueRefOrAddDefault(_backing, key, out bool exists);
        if (exists) {
            if (entry.IsEmpty) {
                // empty entries do not count towards the key count
                _keyCount++;
            }
            entry.Add(value);
        }
        else {
            entry = new(value);
            _keyCount++;
        }
#else
        if (!_backing.TryGetValue(key, out Vec<V> entry)) {
            entry = new();
            _backing.Add(key, entry);
        }
        entry.Add(value);
#endif
    }

    /// <summary>
    /// Add multiple values under the specified key.
    /// Value may not be null.
    /// </summary>
    public void AddRange(K key, ReadOnlySpan<V> values) {
#if NET6_0_OR_GREATER
        ref TinyVec<V> entry = ref CollectionsMarshal.GetValueRefOrAddDefault(_backing, key, out bool exists);
        if (exists) {
            if (entry.IsEmpty) {
                // empty entries do not count towards the key count
                _keyCount++;
            }
            entry.AddRange(values);
        }
        else {
            entry = new(values.Length);
            entry.AddRange(values);
            _keyCount++;
        }
#else
        if (!_backing.TryGetValue(key, out Vec<V> entry)) {
            entry = new(values.Length);
            _backing.Add(key, entry);
        }
        entry.AddRange(values);
#endif
    }

    /// <summary>
    /// Removes all values associated with a specified key.
    /// </summary>
    /// <param name="key">The key to drop.</param>
    /// <returns><c>true</c> if values have been removed; otherwise <c>false</c>.</returns>
    public bool Remove(K key) {
#if NET6_0_OR_GREATER
        ref TinyVec<V> entry = ref CollectionsMarshal.GetValueRefOrNullRef(_backing, key);
        if (Unsafe.IsNullRef(ref entry)) {
            return false;
        }
#else
        if (!_backing.TryGetValue(key, out Vec<V> entry)) {
            return false;
        }
#endif

        if (entry.IsEmpty) {
            return false;
        }

        entry.Clear();
        return true;
    }

    /// <summary>
    /// Removes an entry with the specified key and value.
    /// Returns true if found, false otherwise.
    /// </summary>
    /// <returns><c>true</c> if keys have been removed; otherwise <c>false</c>.</returns>
    public bool Remove(K key, V value) {
#if NET6_0_OR_GREATER
        ref TinyVec<V> entry = ref CollectionsMarshal.GetValueRefOrNullRef(_backing, key);
        if (Unsafe.IsNullRef(ref entry)) {
            return false;
        }
#else
        if (!_backing.TryGetValue(key, out Vec<V> entry)) {
            return false;
        }
#endif
        int removeAt = entry.IndexOf(value);
        if (removeAt == -1) {
            return false;
        }

        entry.SwapRemove(removeAt);

        if (entry.IsEmpty)
        {
            _keyCount--;
        }
        _valueCount--;

        return true;
    }

    /// <summary>
    /// Empty the collection
    /// </summary>
    public void Clear()
    {
        _backing.Clear();
        _valueCount = 0;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<K, IReadOnlyCollection<V>>> IEnumerable<KeyValuePair<K, IReadOnlyCollection<V>>>.GetEnumerator() {
        // use var and do not deconstruct, bc of Value type Vec vs TinyVec.
        foreach (var kvp in _backing) {
            if (!kvp.Value.IsEmpty) {
                yield return new(kvp.Key, kvp.Value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<K, IReadOnlyCollection<V>>>)this).GetEnumerator();

    /// <inheritdoc cref="Dictionary{K, V}.KeyCollection"/>
    [DebuggerDisplay("Count={Count}"), DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
    public readonly struct KeyCollection : IReadOnlyCollection<K>, ICollection<K> {
        private readonly MultiDictionary<K, V> _dict;
        internal KeyCollection(MultiDictionary<K, V> dict) {
            _dict = dict;
        }

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count => _dict.KeyCount;

        /// <inheritdoc />
        public void CopyTo(K[] array, int arrayIndex) {
            ThrowHelper.ArgumentInRange(arrayIndex, array.Length - arrayIndex >= Count);
            int i = arrayIndex;
            foreach (K key in this) {
                array[i] = key;
                i++;
            }
        }

        /// <inheritdoc cref="ICollection{K}.Contains" />
        public bool Contains(K item) => !_dict[item].IsEmpty;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public Enumerator GetEnumerator() => new(_dict);

        IEnumerator<K> IEnumerable<K>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#region Readonly hidden members
        bool ICollection<K>.IsReadOnly => true;

        /// <inheritdoc />
        void ICollection<K>.Add(K item) => ((ICollection<K>)_dict._backing.Keys).Add(item);

        /// <inheritdoc />
        void ICollection<K>.Clear() => ((ICollection<K>)_dict._backing.Keys).Clear();

        /// <inheritdoc />
        bool ICollection<K>.Remove(K item) => ((ICollection<K>)_dict.Keys).Remove(item);
#endregion

        /// <inheritdoc cref="Dictionary{K, V}.KeyCollection.Enumerator"/>
        public struct Enumerator : IEnumerator<K> {
            private readonly MultiDictionary<K, V> _dict;
#if NET6_0_OR_GREATER
            private Dictionary<K, TinyVec<V>>.KeyCollection.Enumerator _keys;
#else
            private Dictionary<K, Vec<V>>.KeyCollection.Enumerator _keys;
#endif

            internal Enumerator(MultiDictionary<K, V> dict) {
                _dict = dict;
                _keys = dict._backing.Keys.GetEnumerator();
            }

            /// <inheritdoc />
            public bool MoveNext() {
                while (_keys.MoveNext()) {
                    if (!_dict[_keys.Current!].IsEmpty) {
                        return true;
                    }
                }

                return false;
            }

            /// <inheritdoc />
            public void Reset() => _keys = _dict._backing.Keys.GetEnumerator();

            /// <inheritdoc />
            public readonly K Current => _keys.Current!;

            readonly object IEnumerator.Current => ((IEnumerator)_keys).Current!;

            /// <inheritdoc />
            public void Dispose() {
            }
        }
    }

    /// <inheritdoc cref="Dictionary{K, V}.ValueCollection"/>
    [DebuggerDisplay("Count={Count}"), DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
    public readonly struct ValueCollection : IReadOnlyCollection<IEnumerable<V>> {
        private readonly MultiDictionary<K, V> _dict;

        internal ValueCollection(MultiDictionary<K, V> dict) {
            _dict = dict;
        }

        /// <summary>The number of values over all keys. Equal to <see cref="MultiDictionary{K,V}.ValueCount"/></summary>
        public int ValuesCount => _dict.ValueCount;

        /// <summary>The number of value collections. Equal to <see cref="MultiDictionary{K,V}.KeyCount"/></summary>
        public int Count => _dict.KeyCount;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public Enumerator GetEnumerator() => new(_dict);

        IEnumerator<IEnumerable<V>> IEnumerable<IEnumerable<V>>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="Dictionary{K, V}.ValueCollection.Enumerator"/>
        public struct Enumerator : IEnumerator<IEnumerable<V>> {
            private readonly MultiDictionary<K, V> _dict;
            private KeyCollection.Enumerator _keys;

            public Enumerator(MultiDictionary<K, V> dict) {
                _dict = dict;
                _keys = dict.Keys.GetEnumerator();
            }

            /// <inheritdoc />
            public bool MoveNext() => _keys.MoveNext();

            /// <inheritdoc />
            public void Reset() => _keys = _dict.Keys.GetEnumerator();

            /// <inheritdoc cref="IEnumerator{T}.Current" />
            public ReadOnlySpan<V> Current => _dict[_keys.Current];

            IEnumerable<V> IEnumerator<IEnumerable<V>>.Current => new ReadOnlyCollection<V>(_dict._backing[_keys.Current]);

            object IEnumerator.Current => ((IEnumerator<IEnumerable<V>>)this).Current!;

            /// <inheritdoc />
            public void Dispose() { }
        }
    }

    /// <inheritdoc cref="Dictionary{K, V}.Enumerator"/>
    public ref struct Enumerator {
        private readonly MultiDictionary<K, V> _dict;
        private KeyCollection.Enumerator _keys;
        private KeyValuesPair _current;

        internal Enumerator(MultiDictionary<K, V> dict) {
            _dict = dict;
            _keys = dict.Keys.GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerator{T}.MoveNext"/>
        public bool MoveNext() {
            if (_keys.MoveNext() && !_dict[_keys.Current!].IsEmpty) {
                _current = new(_keys.Current!, _dict[_keys.Current!]);
                return true;
            }

            _current = default;
            return false;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly KeyValuesPair Current => _current;

        /// <inheritdoc cref="IEnumerator{T}.Reset"/>
        public void Reset() {
            _keys = _dict.Keys.GetEnumerator();
            _current = default;
        }
   }

    /// <inheritdoc cref="KeyValuePair{K,ReadOnlySpan}"/>
    public readonly ref struct KeyValuesPair {
        /// <inheritdoc cref="KeyValuePair{K,ReadOnlySpan}.Key"/>
        public readonly K Key;
        /// <inheritdoc cref="KeyValuePair{K,ReadOnlySpan}.Value"/>
        public readonly ReadOnlySpan<V> Values;

        /// <inheritdoc cref="KeyValuePair{K,ReadOnlySpan}()"/>
        public KeyValuesPair(K key, ReadOnlySpan<V> values) {
            Key = key;
            Values = values;
        }

        /// <summary>Returns the <see cref="Key"/> and <see cref="Values"/>.</summary>
        public void Deconstruct(out K key, out ReadOnlySpan<V> values) {
            key = Key;
            values = Values;
        }
    }

}
