# IReadOnlyVector&lt;T&gt;

Namespace: Rustic.Memory

Represents a strongly typed vector of object that can be accessed by ref. Provides a similar interface as [List&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1).

```csharp
public interface IReadOnlyVector<T> : , , , System.Collections.IEnumerable
```

#### Type Parameters

`T`<br>
The type of items of the vector.

Implements IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **IsEmpty**

Returns a value that indicates whether the vector is empty.

```csharp
public abstract bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsDefault**

Returns a value that indicates whether the vector is at its default value, no memory is allocated.

```csharp
public abstract bool IsDefault { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Count**

Returns the number of elements in the vector.

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**



```csharp
public abstract int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**



```csharp
public abstract T& Item { get; }
```

#### Property Value

T&<br>

## Methods

### **AsSpan(Int32, Int32)**

Creates a new span over a target vector.

```csharp
ReadOnlySpan<T> AsSpan(int start, int length)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

ReadOnlySpan&lt;T&gt;<br>
The span representation of the vector.

### **IndexOf&lt;E&gt;(Int32, Int32, T&, E&)**

Reports the zero-based index of the first occurrence of the specified  in the vector.

```csharp
int IndexOf<E>(int start, int count, T& item, E& comparer)
```

#### Type Parameters

`E`<br>
The type of the comparer.

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

`comparer` E&<br>
The comparer implementation to use when comparing elements.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IndexOf(Int32, Int32, T&)**

Reports the zero-based index of the first occurrence of the specified  in the vector.

```csharp
int IndexOf(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf&lt;E&gt;(Int32, Int32, T&, E&)**

Reports the zero-based index of the first occurrence of the specified  in the vector.

```csharp
int LastIndexOf<E>(int start, int count, T& item, E& comparer)
```

#### Type Parameters

`E`<br>
The type of the comparer.

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

`comparer` E&<br>
The comparer implementation to use when comparing elements.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf(Int32, Int32, T&)**

Reports the zero-based index of the first occurrence of the specified  in the vector.

```csharp
int LastIndexOf(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch&lt;C&gt;(Int32, Int32, T&, C&)**

Searches a range of elements in the sorted vector for an element using the System.Collections.Generic.Comparer`1.Default and returns the zero-based index of the element.

```csharp
int BinarySearch<C>(int start, int count, T& item, C& comparer)
```

#### Type Parameters

`C`<br>
The type of the comparer.

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

`comparer` C&<br>
The comparer implementation to use when comparing elements.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch(Int32, Int32, T&)**

Searches a range of elements in the sorted vector for an element using the System.Collections.Generic.Comparer`1.Default and returns the zero-based index of the element.

```csharp
int BinarySearch(int start, int count, T& item)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based starting index of the range to search.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the range to search.

`item` T&<br>
The element to locate.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TryCopyTo(Span&lt;T&gt;)**



```csharp
bool TryCopyTo(Span<T> destination)
```

#### Parameters

`destination` Span&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
