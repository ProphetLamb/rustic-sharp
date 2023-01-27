using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rustic.Memory;

internal sealed class MultiDictionaryDebugView<K, V> where K : notnull {
    private readonly WeakReference<MultiDictionary<K, V>> _dict;

    public MultiDictionaryDebugView(MultiDictionary<K, V> dictionary) {
        _dict = new(dictionary);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<K, V[]>[] Items {
        get {
            if (!_dict.TryGetTarget(out var dict)) {
                return Array.Empty<KeyValuePair<K, V[]>>();
            }

            KeyValuePair<K, V[]>[] items = new KeyValuePair<K, V[]>[dict.KeyCount];
            int i = 0;
            foreach ((K key, ReadOnlySpan<V> values) in dict) {
                if (i >= items.Length) {
                    return items;
                }
                items[i] = new(key, values.ToArray());
                i++;
            }

            return items;
        }
    }
}


internal sealed class DictionaryKeyCollectionDebugView<K, V> {
    private readonly WeakReference<IReadOnlyCollection<K>> _collection;

    public DictionaryKeyCollectionDebugView(IReadOnlyCollection<K> collection) {
        _collection = new(collection);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public K[] Items {
        get {
            if (!_collection.TryGetTarget(out IReadOnlyCollection<K>? collection)) {
                return Array.Empty<K>();
            }

            K[] items = new K[collection.Count];
            int i = 0;
            foreach (K key in collection) {
                if (i >= items.Length) {
                    return items;
                }

                items[i] = key;
                i++;
            }
            return items;
        }
    }
}

internal sealed class DictionaryValueCollectionDebugView<K, V> {
    private readonly WeakReference<IReadOnlyCollection<V>> _collection;

    public DictionaryValueCollectionDebugView(IReadOnlyCollection<V> collection) {
        _collection = new(collection);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public V[] Items {
        get {
            if (!_collection.TryGetTarget(out IReadOnlyCollection<V>? collection)) {
                return Array.Empty<V>();
            }

            V[] items = new V[collection.Count];
            int i = 0;
            foreach (V value in collection) {
                if (i >= items.Length) {
                    return items;
                }

                items[i] = value;
                i++;
            }
            return items;
        }
    }
}
