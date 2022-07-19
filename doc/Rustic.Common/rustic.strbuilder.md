# StrBuilder

Namespace: Rustic

This class represents a mutable string. Initially allocated in the stack, resorts to the [ArrayPool&lt;T&gt;.Shared](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1.shared) when growing.

```csharp
public struct StrBuilder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [StrBuilder](./rustic.strbuilder.md)

## Properties

### **Length**

The length of the string.

```csharp
public int Length { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**

The current capacity of the builder.

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**

```csharp
public Char& Item { get; }
```

#### Property Value

[Char&](https://docs.microsoft.com/en-us/dotnet/api/system.char&)<br>

### **RawChars**

Returns the underlying storage of the builder.

```csharp
public Span<char> RawChars { get; }
```

#### Property Value

[Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

## Constructors

### **StrBuilder(Span&lt;Char&gt;)**

Initializes a new [StrBuilder](./rustic.strbuilder.md) with the specified buffer.

```csharp
StrBuilder(Span<char> initialBuffer)
```

#### Parameters

`initialBuffer` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The stack-buffer used to build the string.

### **StrBuilder(Int32)**

Initializes a new [StrBuilder](./rustic.strbuilder.md) with a array from the [ArrayPool&lt;T&gt;.Shared](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1.shared) with the specific size.

```csharp
StrBuilder(int initialCapacity)
```

#### Parameters

`initialCapacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The minimum capacity of the pool-array.

## Methods

### **EnsureCapacity(Int32)**

Ensures that the builder has at least the given capacity.

```csharp
void EnsureCapacity(int capacity)
```

#### Parameters

`capacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The minimum capacity of the pool-array.

### **GetPinnableReference()**

Get a pinnable reference to the builder.
 Does not ensure there is a null char after [StrBuilder.Length](./rustic.strbuilder.md#length)
 This overload is pattern matched in the C# 7.3+ compiler so you can omit
 the explicit method call, and write eg "fixed (char* c = builder)"

```csharp
Char& GetPinnableReference()
```

#### Returns

[Char&](https://docs.microsoft.com/en-us/dotnet/api/system.char&)<br>

### **GetPinnableReference(Boolean)**

Get a pinnable reference to the builder.

```csharp
Char& GetPinnableReference(bool terminate)
```

#### Parameters

`terminate` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Ensures that the builder has a null char after

#### Returns

[Char&](https://docs.microsoft.com/en-us/dotnet/api/system.char&)<br>

### **ToString()**

Creates the string from the builder and disposes the instance.

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The string represented by the builder.

### **AsSpan(Boolean)**

Returns a span around the contents of the builder.

```csharp
ReadOnlySpan<char> AsSpan(bool terminate)
```

#### Parameters

`terminate` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Ensures that the builder has a null char after

#### Returns

[ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **AsSpan()**

Returns the span representing the current string.

```csharp
ReadOnlySpan<char> AsSpan()
```

#### Returns

[ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **AsSpan(Int32)**

Returns the span representing a portion of the current string.

```csharp
ReadOnlySpan<char> AsSpan(int start)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first char.

#### Returns

[ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **AsSpan(Int32, Int32)**

Returns the span representing a portion of the current string.

```csharp
ReadOnlySpan<char> AsSpan(int start, int length)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the first char.

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of characters after the .

#### Returns

[ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **TryCopyTo(Span&lt;Char&gt;, Int32&)**

```csharp
bool TryCopyTo(Span<char> destination, Int32& charsWritten)
```

#### Parameters

`destination` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`charsWritten` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Insert(Int32, Char, Int32)**

Inserts a character a specific number of times at the .

```csharp
void Insert(int index, char value, int count)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index at which to insert the characters.

`value` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
The value of the characters to insert.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of characters to insert.

### **Insert(Int32, Char)**

Inserts a character at the .

```csharp
void Insert(int index, char value)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index at which to insert the character.

`value` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
The value of the character to insert.

### **Insert(Int32, String)**

Inserts a string at the .

```csharp
void Insert(int index, string value)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index at which to insert the character.

`value` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The string to insert.

### **Append(Char)**

Appends the character to the end of the builder.

```csharp
void Append(char value)
```

#### Parameters

`value` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
The character.

### **Append(String)**

Appends the string to the end of the builder.

```csharp
void Append(string value)
```

#### Parameters

`value` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The string to append.

### **Append(Char, Int32)**

Appends a character a specific number of times at the end of the builder.

```csharp
void Append(char value, int count)
```

#### Parameters

`value` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>
The value of the characters to insert.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of characters to insert.

### **Append(Char*, Int32)**

Appends a unmanaged char-array to the builder

```csharp
void Append(Char* value, int length)
```

#### Parameters

`value` [Char*](https://docs.microsoft.com/en-us/dotnet/api/system.char*)<br>
The pointer to the first character to append.

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of characters after the  pointer.

### **Append(ReadOnlySpan&lt;Char&gt;)**

Appends a span to the builder.

```csharp
void Append(ReadOnlySpan<char> value)
```

#### Parameters

`value` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The span to append.

### **AppendSpan(Int32)**

Appends a mutable span of a specific length to the builder.

```csharp
Span<char> AppendSpan(int length)
```

#### Parameters

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the span to append.

#### Returns

[Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The span at the end of the builder.

### **Dispose()**

```csharp
void Dispose()
```
