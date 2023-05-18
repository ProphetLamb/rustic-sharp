# EnumerableExtensions

Namespace: Rustic.Memory

Extensions for types implementing [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1).

```csharp
public static class EnumerableExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [EnumerableExtensions](./rustic.memory.enumerableextensions.md)

## Methods

### **WithIndex&lt;T&gt;(IEnumerable&lt;T&gt;)**

Zips the sequence of elements with a incrementing count from zero, indicating the index of the item in a array-list.

```csharp
public static IEnumerable<ValueTuple<int, T>> WithIndex<T>(IEnumerable<T> source)
```

#### Type Parameters

`T`<br>
The type of elements in the sequence.

#### Parameters

`source` IEnumerable&lt;T&gt;<br>
The sequence of elements.

#### Returns

IEnumerable&lt;ValueTuple&lt;Int32, T&gt;&gt;<br>
