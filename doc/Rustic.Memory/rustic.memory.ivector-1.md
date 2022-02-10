# IVector&lt;T&gt;

Namespace: Rustic.Memory

Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as [List&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1).

```csharp
public interface IVector<T> : IReadOnlyVector`1, , , , System.Collections.IEnumerable, , , System.Collections.ICollection
```

#### Type Parameters

`T`<br>
The type of items of the vector.

Implements IReadOnlyVector&lt;T&gt;, IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), IList&lt;T&gt;, ICollection&lt;T&gt;, [ICollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection)

## Properties

### **Item**



```csharp
public abstract T& Item { get; }
```

#### Property Value

T&<br>

### **Count**

Returns the number of elements in the vector.

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **Reserve(Int32)**

Ensures that the collection can contain at least  more elements.

```csharp
int Reserve(int additionalCapacity)
```

#### Parameters

`additionalCapacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of additional elements the collection must be able to contain.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The new capacity of the collection.

### **InsertRange(Int32, ReadOnlySpan&lt;T&gt;)**

Inserts a range of  into the vector at the specified index.

```csharp
void InsertRange(int index, ReadOnlySpan<T> values)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index at which to insert the first element.

`values` ReadOnlySpan&lt;T&gt;<br>
The collection of values to insert.

### **RemoveRange(Int32, Int32)**

Removes a specified range of values for the vector.

```csharp
void RemoveRange(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to remove.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to remove.

### **Sort&lt;C&gt;(Int32, Int32, C&)**

Sorts a range of elements in the vector using the specified .

```csharp
void Sort<C>(int start, int count, C& comparer)
```

#### Type Parameters

`C`<br>
The type of the comparer.

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to sort.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to sort.

`comparer` C&<br>
The comparer implementation to use when comparing elements.

### **Sort(Int32, Int32)**

Sorts a range of elements in the vector using the System.Collections.Generic.Comparer`1.Default.

```csharp
void Sort(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to sort.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to sort.

### **Reverse(Int32, Int32)**

Reveses the order of a range of elements in the vector.

```csharp
void Reverse(int start, int count)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to reverse.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to reverse.
