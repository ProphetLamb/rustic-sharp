# BitSet

Namespace: Rustic.Memory

Enables unaligned marking of bits in a fixed size memory area.

```csharp
public struct BitSet
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [BitSet](./rustic.memory.bitset.md)

## Properties

### **RawStorage**

Returns the raw storage of the [BitSet](./rustic.memory.bitset.md) readonly.

```csharp
public ReadOnlySpan<int> RawStorage { get; }
```

#### Property Value

[ReadOnlySpan&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

## Constructors

### **BitSet(Span&lt;Int32&gt;)**

Initializes a new instance of [BitSet](./rustic.memory.bitset.md).

```csharp
BitSet(Span<int> span)
```

#### Parameters

`span` [Span&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The span of 32bit integers to be used for marking bits.

## Methods

### **Mark(Int32)**

Sets the bit at the index to one.

```csharp
void Mark(int bitIndex)
```

#### Parameters

`bitIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Unmark(Int32)**

Sets the bit at the index to zero.

```csharp
void Unmark(int bitIndex)
```

#### Parameters

`bitIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Set(Int32, Int32)**

Sets the bit at the index.

```csharp
void Set(int bitIndex, int value)
```

#### Parameters

`bitIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`value` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsMarked(Int32)**

Gets the value indicating wether the bit at the index is marked.

```csharp
bool IsMarked(int bitIndex)
```

#### Parameters

`bitIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
