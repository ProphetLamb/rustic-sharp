# VecIter&lt;T&gt;

Namespace: Rustic.Memory

Enumerates the elements of a segment of an array.

```csharp
public struct VecIter<T>
```

#### Type Parameters

`T`<br>
The type of items of the array.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md)<br>
Implements IEnumerator&lt;T&gt;, [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable), [IEnumerator](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerator), IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Array**



```csharp
public T[] Array { get; }
```

#### Property Value

T[]<br>

### **Current**



```csharp
public T& Current { get; }
```

#### Property Value

T&<br>

### **Length**



```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Index**

The current position of the enumerator.

```csharp
public int Index { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsEmpty**

Returns a value that indicates whether the current segment is empty.

```csharp
public bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Offset**



```csharp
public int Offset { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**



```csharp
public T& Item { get; }
```

#### Property Value

T&<br>

## Constructors

### **VecIter(T[], Int32, Int32)**

Initializes a new instance of the iterator.

```csharp
VecIter(T[] array, int offset, int count)
```

#### Parameters

`array` T[]<br>
The array to iterate.

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index of the first element to iterate.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of elements to iterate.

## Methods

### **AsSpan()**

Returns the segment represented by the [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md) as a span.

```csharp
ReadOnlySpan<T> AsSpan()
```

#### Returns

ReadOnlySpan&lt;T&gt;<br>
The span representing the segment.

### **GetEnumerator()**



```csharp
VecIter<T> GetEnumerator()
```

#### Returns

[VecIter&lt;T&gt;](./rustic.memory.veciter-1.md)<br>

### **Dispose()**



```csharp
void Dispose()
```

### **MoveNext()**



```csharp
bool MoveNext()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Reset()**



```csharp
void Reset()
```
