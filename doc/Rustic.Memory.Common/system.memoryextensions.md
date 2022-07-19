# MemoryExtensions

Namespace: System

Extensions for [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1), [ReadOnlySpan&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1).

```csharp
public static class MemoryExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MemoryExtensions](./system.memoryextensions.md)

## Methods

### **Union&lt;T&gt;(ReadOnlySpan`1&, ReadOnlySpan`1&, Span`1&)**

Computes the union of two sets.

```csharp
public static void Union<T>(ReadOnlySpan`1& set, ReadOnlySpan`1& other, Span`1& output)
```

#### Type Parameters

`T`<br>
The type of the elements of the sets.

#### Parameters

`set` ReadOnlySpan`1&<br>
The left set.

`other` ReadOnlySpan`1&<br>
The right set.

`output` Span`1&<br>
The output buffer.

### **Union&lt;T, C&gt;(ReadOnlySpan`1&, ReadOnlySpan`1&, Span`1&, C&)**

Computes the union of two sets.

```csharp
public static void Union<T, C>(ReadOnlySpan`1& set, ReadOnlySpan`1& other, Span`1& output, C& comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the sets.

`C`<br>
The type of the comparer.

#### Parameters

`set` ReadOnlySpan`1&<br>
The left set.

`other` ReadOnlySpan`1&<br>
The right set.

`output` Span`1&<br>
The output buffer.

`comparer` C&<br>
The comparer used to compare two elements.
