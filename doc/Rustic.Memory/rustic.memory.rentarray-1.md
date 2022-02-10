# RentArray&lt;T&gt;

Namespace: Rustic.Memory

Allows renting a array without necessarily allocating.

```csharp
public struct RentArray<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [RentArray&lt;T&gt;](./rustic.memory.rentarray-1.md)<br>
Implements IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Array**



```csharp
public T[] Array { get; }
```

#### Property Value

T[]<br>

### **Item**



```csharp
public T& Item { get; }
```

#### Property Value

T&<br>

### **Count**



```csharp
public int Count { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**



```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **RentArray(Int32)**



```csharp
RentArray(int length)
```

#### Parameters

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **Add(T&)**



```csharp
void Add(T& item)
```

#### Parameters

`item` T&<br>

### **Dispose()**



```csharp
void Dispose()
```

### **GetEnumerator()**



```csharp
IEnumerator<T> GetEnumerator()
```

#### Returns

IEnumerator&lt;T&gt;<br>

### **Rent(Int32)**



```csharp
T[] Rent(int length)
```

#### Parameters

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

T[]<br>

### **Return(T[])**



```csharp
void Return(T[] rented)
```

#### Parameters

`rented` T[]<br>
