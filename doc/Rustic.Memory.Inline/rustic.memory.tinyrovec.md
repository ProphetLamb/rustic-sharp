# TinyRoVec

Namespace: Rustic.Memory

Partially inlined immutable collection of function parameters.

```csharp
public static class TinyRoVec
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TinyRoVec](./rustic.memory.tinyrovec.md)

## Methods

### **Empty&lt;T&gt;()**

Returns an empty [TinyRoVec&lt;T&gt;](./rustic.memory.tinyrovec-1.md).

```csharp
public static TinyRoVec<T> Empty<T>()
```

#### Type Parameters

`T`<br>
The type of the span.

#### Returns

TinyRoVec&lt;T&gt;<br>
An empty .

### **From&lt;T&gt;(T&)**

Initializes a new parameter span with one value.

```csharp
public static TinyRoVec<T> From<T>(T& arg0)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

#### Returns

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinyRoVec<T> From<T>(T& arg0, T& arg1)
```

#### Type Parameters

`T`<br>

#### Parameters

`arg0` T&<br>
The first value.

`arg1` T&<br>
The second value.

#### Returns

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinyRoVec<T> From<T>(T& arg0, T& arg1, T& arg2)
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

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T&, T&, T&, T&)**

Initializes a new parameter span with one value.

```csharp
public static TinyRoVec<T> From<T>(T& arg0, T& arg1, T& arg2, T& arg3)
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

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(ArraySegment`1&)**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinyRoVec<T> From<T>(ArraySegment`1& values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` ArraySegment`1&<br>
The values array.

#### Returns

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T[])**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinyRoVec<T> From<T>(T[] values)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` T[]<br>
The values array.

#### Returns

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T[], Int32)**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinyRoVec<T> From<T>(T[] values, int start)
```

#### Type Parameters

`T`<br>

#### Parameters

`values` T[]<br>
The values collection.

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first value.

#### Returns

TinyRoVec&lt;T&gt;<br>

### **From&lt;T&gt;(T[], Int32, Int32)**

Initializes a new parameter span with a sequence of values.

```csharp
public static TinyRoVec<T> From<T>(T[] values, int start, int length)
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

TinyRoVec&lt;T&gt;<br>

### **Copy&lt;T, E&gt;(E)**

Initializes a new parameter span with a sequence of values. Performs a shallow copy.

```csharp
public static TinyRoVec<T> Copy<T, E>(E values)
```

#### Type Parameters

`T`<br>

`E`<br>

#### Parameters

`values` E<br>
The sequence of values.

#### Returns

TinyRoVec&lt;T&gt;<br>
