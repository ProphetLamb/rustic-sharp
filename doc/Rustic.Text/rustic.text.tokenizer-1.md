# Tokenizer&lt;T&gt;

Namespace: Rustic.Text

Tokenizes a sequence, for use in lexer &amp; parser state machines.

```csharp
public struct Tokenizer<T>
```

#### Type Parameters

`T`<br>
The type of an element of the span.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Tokenizer&lt;T&gt;](./rustic.text.tokenizer-1.md)

## Properties

### **Raw**



```csharp
public ReadOnlySpan<T> Raw { get; }
```

#### Property Value

ReadOnlySpan&lt;T&gt;<br>

### **Head**

Defines the current position of the iterator.

```csharp
public int Head { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Length**



```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Token**

Represents the token currently being built.

```csharp
public ReadOnlySpan<T> Token { get; }
```

#### Property Value

ReadOnlySpan&lt;T&gt;<br>

### **Current**



```csharp
public T& Current { get; }
```

#### Property Value

T&<br>

### **TokenIndex**

Represents the zero-based start-index of the current token inside the span.

```csharp
public int TokenIndex { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TokenLength**

Represents the length of the current token.

```csharp
public int TokenLength { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**



```csharp
public T& Item { get; }
```

#### Property Value

T&<br>

## Constructors

### **Tokenizer(ReadOnlySpan&lt;T&gt;, IEqualityComparer&lt;T&gt;)**

Initializes a new instance of [Tokenizer&lt;T&gt;](./rustic.text.tokenizer-1.md) with the specified  and .

```csharp
Tokenizer(ReadOnlySpan<T> input, IEqualityComparer<T> comparer)
```

#### Parameters

`input` ReadOnlySpan&lt;T&gt;<br>
The input sequence.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

## Methods

### **Consume()**

Consumes one element.

```csharp
bool Consume()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element could be consumed; otherwise, .

### **Consume(Int32)**

Consumes a specified amount of elements.

```csharp
bool Consume(int amount)
```

#### Parameters

`amount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the elements could be consumed; otherwise, .

### **Dispose()**



```csharp
void Dispose()
```

### **FinalizeToken()**

Returns &amp; clears the Rustic.Text.Tokenizer`1.Token, and moves the iterator to the first element after the token.

```csharp
ReadOnlySpan<T> FinalizeToken()
```

#### Returns

ReadOnlySpan&lt;T&gt;<br>
The span representing the token.

### **Reset()**

Resets teh builder to the initial state.

```csharp
void Reset()
```

### **Advance(Int32)**

Advances the Rustic.Text.Tokenizer`1.Head to a specific , always consumes elements.

```csharp
void Advance(int position)
```

#### Parameters

`position` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The target position

### **TryAdvance(Int32)**

Advances the Rustic.Text.Tokenizer`1.Head to a specific , consumes elements only if successful.

```csharp
bool TryAdvance(int position)
```

#### Parameters

`position` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The target position

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the elements could be consumed; otherwise, .

### **ReadNextSequence(ReadOnlySpan`1&)**

Returns whether the following elements are the sequence, consumes elements.

```csharp
bool ReadNextSequence(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **TryReadNextSequence(ReadOnlySpan`1&)**

Returns whether the following elements are the sequence, consumes elements only if the successful.

```csharp
bool TryReadNextSequence(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **PeekNextSequence(ReadOnlySpan`1&, Int32&)**

Returns whether the following elements are the sequence.

```csharp
bool PeekNextSequence(ReadOnlySpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **ReadSequence(ReadOnlySpan`1&)**

Consumes elements until the specified sequence of elements has been encountered.

```csharp
bool ReadSequence(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The expected sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **TryReadSequence(ReadOnlySpan`1&)**

Returns whether the remaining span contains the sequence, consumes the elements only if it does.

```csharp
bool TryReadSequence(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **PeekSequence(ReadOnlySpan`1&, Int32&)**

Returns whether the remaining span contains the sequence.

```csharp
bool PeekSequence(ReadOnlySpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **ReadNext(TinySpan`1&)**

Consumes one element, and returns whether the element matches one of the expected elements.

```csharp
bool ReadNext(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **TryReadNext(TinySpan`1&)**

Returns whether the element matches one of the expected elements, and consumes the element only if it does.

```csharp
bool TryReadNext(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **PeekNext(TinySpan`1&)**

Returns whether the element matches one of the expected elements.

```csharp
bool PeekNext(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **ReadUntil(TinySpan`1&)**

Consumes elements until one of the expected elements occur.

```csharp
bool ReadUntil(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **TryReadUntil(TinySpan`1&)**

Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.

```csharp
bool TryReadUntil(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **PeekUntil(TinySpan`1&, Int32&)**

Returns whether the remaining span contains one of the expected elements.

```csharp
bool PeekUntil(TinySpan`1& expected, Int32& head)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position at which the expected element occured.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **ReadNext(T&)**

Consumes one element, and returns whether the element matches one of the expected elements.

```csharp
bool ReadNext(T& expected)
```

#### Parameters

`expected` T&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **TryReadNext(T&)**

Returns whether the element matches one of the expected elements, and consumes the element only if it does.

```csharp
bool TryReadNext(T& expected)
```

#### Parameters

`expected` T&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **PeekNext(T&)**

Returns whether the element matches one of the expected elements.

```csharp
bool PeekNext(T& expected)
```

#### Parameters

`expected` T&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **ReadUntil(T&)**

Consumes elements until one of the expected elements occur.

```csharp
bool ReadUntil(T& expected)
```

#### Parameters

`expected` T&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **TryReadUntil(T&)**

Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.

```csharp
bool TryReadUntil(T& expected)
```

#### Parameters

`expected` T&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **PeekUntil(T&, Int32&)**

Returns whether the remaining span contains one of the expected elements.

```csharp
bool PeekUntil(T& expected, Int32& head)
```

#### Parameters

`expected` T&<br>
The expected elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position at which the expected element occured.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **ReadNext(T&, T&)**

Consumes one element, and returns whether the element matches one of the expected elements.

```csharp
bool ReadNext(T& expected0, T& expected1)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **TryReadNext(T&, T&)**

Returns whether the element matches one of the expected elements, and consumes the element only if it does.

```csharp
bool TryReadNext(T& expected0, T& expected1)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **PeekNext(T&, T&)**

Returns whether the element matches one of the expected elements.

```csharp
bool PeekNext(T& expected0, T& expected1)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **ReadUntil(T&, T&)**

Consumes elements until one of the expected elements occur.

```csharp
bool ReadUntil(T& expected0, T& expected1)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **TryReadUntil(T&, T&)**

Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.

```csharp
bool TryReadUntil(T& expected0, T& expected1)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **PeekUntil(T&, T&, Int32&)**

Returns whether the remaining span contains one of the expected elements.

```csharp
bool PeekUntil(T& expected0, T& expected1, Int32& head)
```

#### Parameters

`expected0` T&<br>
The the expected element.

`expected1` T&<br>
The the other expected element.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position at which the expected element occured.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .
