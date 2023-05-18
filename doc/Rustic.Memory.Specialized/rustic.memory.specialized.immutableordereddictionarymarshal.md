# ImmutableOrderedDictionaryMarshal

Namespace: Rustic.Memory.Specialized

Provides collection methods for [ImmutableOrderedDictionary&lt;K, V&gt;](./rustic.memory.specialized.immutableordereddictionary-2.md).

```csharp
public static class ImmutableOrderedDictionaryMarshal
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ImmutableOrderedDictionaryMarshal](./rustic.memory.specialized.immutableordereddictionarymarshal.md)

## Methods

### **CreateFromPinnedArray&lt;K, V&gt;(KeyValuePair`2[], Int32, IEqualityComparer&lt;K&gt;)**

Initializes a new [ImmutableOrderedDictionary&lt;K, V&gt;](./rustic.memory.specialized.immutableordereddictionary-2.md) from the specified  without copying the array.

```csharp
public static ImmutableOrderedDictionary<K, V> CreateFromPinnedArray<K, V>(KeyValuePair`2[] array, int count, IEqualityComparer<K> keyComparer)
```

#### Type Parameters

`K`<br>

`V`<br>

#### Parameters

`array` KeyValuePair`2[]<br>
The array.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of items from 0 to .Length to use.

`keyComparer` IEqualityComparer&lt;K&gt;<br>
The  to use for comparing keys.

#### Returns

ImmutableOrderedDictionary&lt;K, V&gt;<br>

### **TryGetEntry&lt;K, V&gt;(ImmutableOrderedDictionary&lt;K, V&gt;, K, Int32&)**

Returns the reference to the entry with the specified  if it exists; otherwise returns a null-reference.

```csharp
public static KeyValuePair`2& TryGetEntry<K, V>(ImmutableOrderedDictionary<K, V> dict, K key, Int32& indexIfExists)
```

#### Type Parameters

`K`<br>

`V`<br>

#### Parameters

`dict` ImmutableOrderedDictionary&lt;K, V&gt;<br>
The dictionary.

`key` K<br>
The key of the entry to get.

`indexIfExists` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The index of the entry if it exists; otherwise -1.

#### Returns

KeyValuePair`2&<br>

### **ToImmutableOrderedDictionary&lt;K, V&gt;(IEnumerable&lt;KeyValuePair&lt;K, V&gt;&gt;, IEqualityComparer&lt;K&gt;)**

Creates a new [ImmutableOrderedDictionary&lt;K, V&gt;](./rustic.memory.specialized.immutableordereddictionary-2.md) from the specified .

```csharp
public static ImmutableOrderedDictionary<K, V> ToImmutableOrderedDictionary<K, V>(IEnumerable<KeyValuePair<K, V>> entries, IEqualityComparer<K> keyComparer)
```

#### Type Parameters

`K`<br>

`V`<br>

#### Parameters

`entries` IEnumerable&lt;KeyValuePair&lt;K, V&gt;&gt;<br>
The entries to use.

`keyComparer` IEqualityComparer&lt;K&gt;<br>
The  to use for comparing keys.

#### Returns

ImmutableOrderedDictionary&lt;K, V&gt;<br>
