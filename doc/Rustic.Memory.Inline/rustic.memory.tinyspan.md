# TinySpan

Namespace: Rustic.Memory

Partially inlined immutable collection of function parameters.

```csharp
public static class TinySpan
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TinySpan](./rustic.memory.tinyspan.md)

## Methods

### **Empty&lt;T&gt;()**

Returns an empty [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md).

```csharp
public static TinySpan<T> Empty<T>()
```

#### Type Parameters

`T`<br>
The type of the span.

#### Returns

TinySpan&lt;T&gt;<br>
An empty .

### **From&lt;T&gt;(T&)**

Initializes a new parameter span with one value.

```csharp
public static TinySpan<T> From<T>(T& arg0)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinySpan<T> From<T>(T& arg0, T& arg1)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

`arg1` T&<br>
The second value.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinySpan<T> From<T>(T& arg0, T& arg1, T& arg2)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

`arg1` T&<br>
The second value.

`arg2` T&<br>
The third value.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&, T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinySpan<T> From<T>(T& arg0, T& arg1, T& arg2, T& arg3)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

`arg1` T&<br>
The second value.

`arg2` T&<br>
The third value.

`arg3` T&<br>
The fourth value.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(ReadOnlySpan`1&)**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinySpan<T> From<T>(ReadOnlySpan`1& values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` ReadOnlySpan`1&<br>
The values collection.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(Span`1&)**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinySpan<T> From<T>(Span`1& values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` Span`1&<br>
The values collection.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T[], Int32)**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(T[] values, int start)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` T[]<br>
The values collection.

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first value.

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T[], Int32, Int32)**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(T[] values, int start, int length)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` T[]<br>
The values collection.

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first value.

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of values form the .

#### Returns

TinySpan&lt;T&gt;<br>

### **From&lt;T&gt;(T[])**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(T[] values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` T[]<br>
The sequence of values.

#### Returns

TinySpan&lt;T&gt;<br>

**Remarks:**

If  if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
 then [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md) uses the allocated memory of .
 <br>
 If a deep copy is desired use .

### **From&lt;T&gt;(ReadOnlyMemory&lt;T&gt;)**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(ReadOnlyMemory<T> values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` ReadOnlyMemory&lt;T&gt;<br>
The sequence of values.

#### Returns

TinySpan&lt;T&gt;<br>

**Remarks:**

If  if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
 then [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md) uses the allocated memory of .
 <br>
 If a deep copy is desired use .

### **From&lt;T&gt;(Memory&lt;T&gt;)**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(Memory<T> values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` Memory&lt;T&gt;<br>
The sequence of values.

#### Returns

TinySpan&lt;T&gt;<br>

**Remarks:**

If  if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
 then [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md) uses the allocated memory of .
 <br>
 If a deep copy is desired use .

### **From&lt;T&gt;(ArraySegment&lt;T&gt;)**

Initializes a new parameter span with a sequence of values. Does not allocate or copy.

```csharp
public static TinySpan<T> From<T>(ArraySegment<T> values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` ArraySegment&lt;T&gt;<br>
The sequence of values.

#### Returns

TinySpan&lt;T&gt;<br>

**Remarks:**

If  if of the type [I] Array, [II] (ReadOnly-)Memory, or [III] ArraySegment,
 then [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md) uses the allocated memory of .
 <br>
 If a deep copy is desired use .

### **Copy&lt;T, E&gt;(E)**

Initializes a new parameter span with a sequence of values. Performs a shallow-copy of the sequence of values.

```csharp
public static TinySpan<T> Copy<T, E>(E values)
```

#### Type Parameters

`T`<br>

`E`<br>

#### Parameters

`values` E<br>
The sequence of values.

#### Returns

TinySpan&lt;T&gt;<br>

### **SequenceEquals&lt;T&gt;(TinySpan&lt;T&gt;, TinySpan`1&)**

Determines whether two sequences are equal by comparing the elements.

```csharp
public static bool SequenceEquals<T>(TinySpan<T> span, TinySpan`1& other)
```

#### Type Parameters

`T`<br>

#### Parameters

`span` TinySpan&lt;T&gt;<br>

`other` TinySpan`1&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
