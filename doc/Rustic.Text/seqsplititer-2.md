# SeqSplitIter&lt;T, S&gt;

Namespace:

Iterates a span in segments determined by separator sequences.

```csharp
public struct SeqSplitIter<T, S>
```

#### Type Parameters

`T`<br>
The type of an element of the span.

`S`<br>
The type of a sequence of elements.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [SeqSplitIter&lt;T, S&gt;](./seqsplititer-2.md)

## Properties

### **Current**

The segment of the current state of the enumerator.

```csharp
public ReadOnlySpan<T> Current { get; }
```

#### Property Value

ReadOnlySpan&lt;T&gt;<br>

### **Position**

Represents the zero-based start-index of the current segment inside the source span.

```csharp
public int Position { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Width**

Represents the length of the current segment.

```csharp
public int Width { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IncludesSeparator**

Indicates whether the SeqSplitIter`2.Current item is terminated by the separator.

```csharp
public bool IncludesSeparator { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **GetEnumerator()**



```csharp
SeqSplitIter<T, S> GetEnumerator()
```

#### Returns

[SeqSplitIter&lt;T, S&gt;](./seqsplititer-2.md)<br>

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

### **Dispose()**



```csharp
void Dispose()
```
