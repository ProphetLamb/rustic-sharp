# BufWriter&lt;T&gt;

Namespace: Rustic.Memory

Reusable System.Buffers.IBufferWriter`1 intended for use as a thread-static singleton.

```csharp
public class BufWriter<T> : , IVector`1, IReadOnlyVector`1, , , , System.Collections.IEnumerable, , , System.Collections.ICollection, System.IDisposable
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BufWriter&lt;T&gt;](./rustic.memory.bufwriter-1.md)<br>
Implements IBufferWriter&lt;T&gt;, IVector&lt;T&gt;, IReadOnlyVector&lt;T&gt;, IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), IList&lt;T&gt;, ICollection&lt;T&gt;, [ICollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Length**



```csharp
public int Length { get; internal set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**



```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsEmpty**



```csharp
public bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsDefault**



```csharp
public bool IsDefault { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Count**



```csharp
public int Count { get; }
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

### **BufWriter()**

Initializes a new instance of [BufWriter&lt;T&gt;](./rustic.memory.bufwriter-1.md).

```csharp
public BufWriter()
```

### **BufWriter(Int32)**

Initializes a new instance of [BufWriter&lt;T&gt;](./rustic.memory.bufwriter-1.md).

```csharp
public BufWriter(int initialCapacity)
```

#### Parameters

`initialCapacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The minimum capacity of the writer.

## Methods

### **Advance(Int32)**



```csharp
public void Advance(int count)
```

#### Parameters

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **GetMemory(Int32)**



```csharp
public Memory<T> GetMemory(int sizeHint)
```

#### Parameters

`sizeHint` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

Memory&lt;T&gt;<br>

### **GetSpan(Int32)**



```csharp
public Span<T> GetSpan(int sizeHint)
```

#### Parameters

`sizeHint` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

Span&lt;T&gt;<br>

### **Add(T)**



```csharp
public void Add(T item)
```

#### Parameters

`item` T<br>

### **Clear()**



```csharp
public void Clear()
```

### **Insert(Int32, T&)**



```csharp
public void Insert(int index, T& item)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

### **RemoveAt(Int32)**



```csharp
public void RemoveAt(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **CopyTo(T[], Int32)**



```csharp
public void CopyTo(T[] array, int arrayIndex)
```

#### Parameters

`array` T[]<br>

`arrayIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ToSpan(T[]&)**

Returns the [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1) representing the written / requested portion of the buffer.
 Rustic.Memory.BufWriter`1.Resets the buffer.

```csharp
public Span<T> ToSpan(T[]& array)
```

#### Parameters

`array` T[]&<br>
The internal array

#### Returns

Span&lt;T&gt;<br>

### **ToMemory(T[]&)**

Returns the [Memory&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1) representing the written / requested portion of the buffer.
 Rustic.Memory.BufWriter`1.Resets the buffer.

```csharp
public Memory<T> ToMemory(T[]& array)
```

#### Parameters

`array` T[]&<br>
The internal array

#### Returns

Memory&lt;T&gt;<br>

### **ToSegment()**

Returns the [ArraySegment&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.arraysegment-1) representing the written / requested portion of the buffer.
 Rustic.Memory.BufWriter`1.Resets the buffer.

```csharp
public ArraySegment<T> ToSegment()
```

#### Returns

ArraySegment&lt;T&gt;<br>

### **ToArray()**

Returns a array containing a shallow-copy of the written / requested portion of the buffer.

```csharp
public T[] ToArray()
```

#### Returns

T[]<br>
A array containing a shallow-copy of the written / requested portion of the buffer.

### **ToArray(Boolean)**

Returns a array containing a shallow-copy of the written / requested portion of the buffer.

```csharp
public T[] ToArray(bool dispose)
```

#### Parameters

`dispose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to dispose the object, or reset.

#### Returns

T[]<br>
A array containing a shallow-copy of the written / requested portion of the buffer.

### **Reset()**

Resets the writer to the initial state and returns the buffer to the array-pool.

```csharp
public void Reset()
```

### **Dispose()**



```csharp
public void Dispose()
```

### **Dispose(Boolean)**



```csharp
protected void Dispose(bool disposing)
```

#### Parameters

`disposing` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetEnumerator()**



```csharp
public VecIter<T> GetEnumerator()
```

#### Returns

VecIter&lt;T&gt;<br>

### **Grow(Int32)**

Grows the buffer so that it can contain at least .

```csharp
protected void Grow(int additionalCapacityBeyondPos)
```

#### Parameters

`additionalCapacityBeyondPos` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Reserve(Int32)**



```csharp
public int Reserve(int additionalCapacity)
```

#### Parameters

`additionalCapacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **InsertRange(Int32, ReadOnlySpan&lt;T&gt;)**



```csharp
public void InsertRange(int index, ReadOnlySpan<T> values)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`values` ReadOnlySpan&lt;T&gt;<br>

### **RemoveRange(Int32, Int32)**



```csharp
public void RemoveRange(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Sort&lt;C&gt;(Int32, Int32, C&)**



```csharp
public void Sort<C>(int start, int count, C& comparer)
```

#### Type Parameters

`C`<br>

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`comparer` C&<br>

### **Reverse(Int32, Int32)**



```csharp
public void Reverse(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AsSpan()**



```csharp
public ReadOnlySpan<T> AsSpan()
```

#### Returns

ReadOnlySpan&lt;T&gt;<br>

### **AsSpan(Int32)**



```csharp
public ReadOnlySpan<T> AsSpan(int start)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

ReadOnlySpan&lt;T&gt;<br>

### **AsSpan(Int32, Int32)**



```csharp
public ReadOnlySpan<T> AsSpan(int start, int length)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

ReadOnlySpan&lt;T&gt;<br>

### **IndexOf&lt;E&gt;(Int32, Int32, T&, E&)**



```csharp
public int IndexOf<E>(int start, int count, T& item, E& comparer)
```

#### Type Parameters

`E`<br>

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

`comparer` E&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf&lt;E&gt;(Int32, Int32, T&, E&)**



```csharp
public int LastIndexOf<E>(int start, int count, T& item, E& comparer)
```

#### Type Parameters

`E`<br>

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

`comparer` E&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch&lt;C&gt;(Int32, Int32, T&, C&)**



```csharp
public int BinarySearch<C>(int start, int count, T& item, C& comparer)
```

#### Type Parameters

`C`<br>

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

`comparer` C&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TryCopyTo(Span&lt;T&gt;)**



```csharp
public bool TryCopyTo(Span<T> destination)
```

#### Parameters

`destination` Span&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Sort(Int32, Int32)**



```csharp
public void Sort(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IndexOf(Int32, Int32, T&)**



```csharp
public int IndexOf(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf(Int32, Int32, T&)**



```csharp
public int LastIndexOf(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch(Int32, Int32, T&)**



```csharp
public int BinarySearch(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
