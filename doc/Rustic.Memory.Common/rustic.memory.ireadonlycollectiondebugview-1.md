# IReadOnlyCollectionDebugView&lt;T&gt;

Namespace: Rustic.Memory

Proxy class used for displaying a [IReadOnlyCollection&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1) in the debugger.

```csharp
public sealed class IReadOnlyCollectionDebugView<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [IReadOnlyCollectionDebugView&lt;T&gt;](./rustic.memory.ireadonlycollectiondebugview-1.md)

## Properties

### **Items**

A shallow-copy of the items of the collection.

```csharp
public T[] Items { get; }
```

#### Property Value

T[]<br>

## Constructors

### **IReadOnlyCollectionDebugView(IReadOnlyCollection&lt;T&gt;)**

Intializes a new instance of [IReadOnlyCollectionDebugView&lt;T&gt;](./rustic.memory.ireadonlycollectiondebugview-1.md).

```csharp
public IReadOnlyCollectionDebugView(IReadOnlyCollection<T> collection)
```

#### Parameters

`collection` IReadOnlyCollection&lt;T&gt;<br>
The collection to display.
