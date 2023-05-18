# ReversibleIndexedSpan&lt;T&gt;

Namespace: Rustic.Memory

Wrapper around [ReadOnlySpan&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1) that allows indexed access in reversed order.

```csharp
public struct ReversibleIndexedSpan<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md)

## Fields

### **Span**

The underlying data span, in the original order.

```csharp
public ReadOnlySpan<T> Span;
```

## Properties

### **IsReverse**

Indicates whether the [ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md) is reversed or not.

```csharp
public bool IsReverse { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Item**

```csharp
public T& Item { get; }
```

#### Property Value

T&<br>

### **IsEmpty**

```csharp
public bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Length**

```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **ReversibleIndexedSpan(ReadOnlySpan&lt;T&gt;, Boolean)**

Initializes a new [ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md).

```csharp
ReversibleIndexedSpan(ReadOnlySpan<T> span, bool reverse)
```

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span

`reverse` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If the indexer access is reversed

## Methods

### **Slice(Int32)**

```csharp
ReversibleIndexedSpan<T> Slice(int start)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md)<br>

### **Slice(Int32, Int32)**

```csharp
ReversibleIndexedSpan<T> Slice(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md)<br>

### **SliceEnd(Int32)**

Slices the span from the end

```csharp
ReversibleIndexedSpan<T> SliceEnd(int end)
```

#### Parameters

`end` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of elements to slice from the end.

#### Returns

[ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md)<br>
The sliced span.

### **CopyTo(Span&lt;T&gt;)**

```csharp
void CopyTo(Span<T> span)
```

#### Parameters

`span` Span&lt;T&gt;<br>

### **TryCopyTo(Span&lt;T&gt;)**

```csharp
bool TryCopyTo(Span<T> span)
```

#### Parameters

`span` Span&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToSpan()**

Returns a span in correct order.

```csharp
ReadOnlySpan<T> ToSpan()
```

#### Returns

ReadOnlySpan&lt;T&gt;<br>

**Remarks:**

If reversed allocates a array and returns a span over the array.

### **ToSpan(T[]&)**

Returns a span in correct order.

```csharp
ReadOnlySpan<T> ToSpan(T[]& sharedPoolArray)
```

#### Parameters

`sharedPoolArray` T[]&<br>
The array from the shared pool, if any.

#### Returns

ReadOnlySpan&lt;T&gt;<br>

**Remarks:**

If reversed retrieves an array from the [ArrayPool&lt;T&gt;.Shared](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1.shared) and returns a span a portion over the array.

### **Reverse()**

Returns a new [ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md) over the data in opposite direction.

```csharp
ReversibleIndexedSpan<T> Reverse()
```

#### Returns

[ReversibleIndexedSpan&lt;T&gt;](./rustic.memory.reversibleindexedspan-1.md)<br>

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
