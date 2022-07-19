# VecIterExtensions

Namespace: Rustic.Memory

Extension methods for [ArraySegment&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.arraysegment-1) and [Array](https://docs.microsoft.com/en-us/dotnet/api/system.array).

```csharp
public static class VecIterExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [VecIterExtensions](./rustic.memory.veciterextensions.md)

## Methods

### **GetIterator&lt;T&gt;(ArraySegment&lt;T&gt;)**

Initializes a new [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md) for the segment.

```csharp
public static VecIter<T> GetIterator<T>(ArraySegment<T> segment)
```

#### Type Parameters

`T`<br>
The type of elements in the array.

#### Parameters

`segment` ArraySegment&lt;T&gt;<br>
The segment.

#### Returns

VecIter&lt;T&gt;<br>

### **GetIterator&lt;T&gt;(T[])**

Initializes a new [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md) for the array.

```csharp
public static VecIter<T> GetIterator<T>(T[] array)
```

#### Type Parameters

`T`<br>
The type of elements in the array.

#### Parameters

`array` T[]<br>
The array.

#### Returns

VecIter&lt;T&gt;<br>

### **GetIterator&lt;T&gt;(T[], Int32)**

Initializes a new [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md) for the array.

```csharp
public static VecIter<T> GetIterator<T>(T[] array, int offset)
```

#### Type Parameters

`T`<br>
The type of elements in the array.

#### Parameters

`array` T[]<br>
The array.

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first element in the array.

#### Returns

VecIter&lt;T&gt;<br>

### **GetIterator&lt;T&gt;(T[], Int32, Int32)**

Initializes a new [VecIter&lt;T&gt;](./rustic.memory.veciter-1.md) for the array.

```csharp
public static VecIter<T> GetIterator<T>(T[] array, int offset, int count)
```

#### Type Parameters

`T`<br>
The type of elements in the array.

#### Parameters

`array` T[]<br>
The array.

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first element in the array.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of elements form the .

#### Returns

VecIter&lt;T&gt;<br>
