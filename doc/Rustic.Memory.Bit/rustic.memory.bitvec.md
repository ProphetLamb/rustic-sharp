# BitVec

Namespace: Rustic.Memory

Represents a collection of bits with a list-like interface. Resorts to [ArrayPool&lt;T&gt;.Shared](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1.shared) when growing.

```csharp
public struct BitVec
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [BitVec](./rustic.memory.bitvec.md)

## Properties

### **Count**

The number of bits in the collection.

```csharp
public int Count { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**

The upper limit to the amount of bits the collection can hold without reallocating.

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**

```csharp
public bool Item { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **BitVec(Span&lt;Int32&gt;)**

Initializes a new [BitVec](./rustic.memory.bitvec.md) with the specified .

```csharp
BitVec(Span<int> initialStorage)
```

#### Parameters

`initialStorage` [Span&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **BitVec(Int32)**

Initializes a new [BitVec](./rustic.memory.bitvec.md) able to contain the specified number of bits.

```csharp
BitVec(int capacity)
```

#### Parameters

`capacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The lower limit to the number of bits the collection can contain.

## Methods

### **Add(Boolean)**

Add the value to the end of the collection.

```csharp
void Add(bool value)
```

#### Parameters

`value` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Reserve(Int32)**

Ensures that the collection can hold at least  bits.

```csharp
void Reserve(int additionalCapacityBeyondPos)
```

#### Parameters

`additionalCapacityBeyondPos` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Dispose()**

```csharp
void Dispose()
```
