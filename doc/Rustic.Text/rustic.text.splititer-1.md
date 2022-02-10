# SplitIter&lt;T&gt;

Namespace: Rustic.Text

Iterates a span in segments determined by separators.

```csharp
public struct SplitIter<T>
```

#### Type Parameters

`T`<br>
The type of an element of the span.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md)

## Properties

### **Current**

The segment of the current state of the enumerator.

```csharp
public ReadOnlySpan<T> Current { get; }
```

#### Property Value

ReadOnlySpan&lt;T&gt;<br>

### **SegmentIndex**

Represents the zero-based start-index of the current segment inside the source span.

```csharp
public int SegmentIndex { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SegmentLength**

Represents the length of the current segment.

```csharp
public int SegmentLength { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IncludesSeparator**

Indicates wether the Rustic.Text.SplitIter`1.Current item is terminated by the separator.

```csharp
public bool IncludesSeparator { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **GetEnumerator()**

Returns a new [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) enumerator with the same input in the initial state.

```csharp
SplitIter<T> GetEnumerator()
```

#### Returns

[SplitIter&lt;T&gt;](./rustic.text.splititer-1.md)<br>

### **MoveNext()**

Attempts to move the enumerator to the next segment.

```csharp
bool MoveNext()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the enumerator successfully moved to the next segment; otherwise, .

### **Reset()**

Resets the enumerator to the initial state.

```csharp
void Reset()
```

### **Dispose()**

Disposes the enumerator.

```csharp
void Dispose()
```
