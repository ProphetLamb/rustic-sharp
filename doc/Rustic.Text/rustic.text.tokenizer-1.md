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

### **Comparer**



```csharp
public IEqualityComparer<T> Comparer { get; }
```

#### Property Value

IEqualityComparer&lt;T&gt;<br>

### **Head**

Defines the current position of the iterator.

```csharp
public int Head { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Position**

Represents the zero-based start-index of the current token inside the span.

```csharp
public int Position { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Width**

Represents the length of the current token.

```csharp
public int Width { get; set; }
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

### **Offset(Int32)**

Represents the token at an offset relative to the Rustic.Text.Tokenizer`1.Head.

```csharp
T& Offset(int offset)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

T&<br>

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

### **Discard()**

Discards the current token &amp; resets the iterator to the start of the token.

```csharp
void Discard()
```

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

### **Read&lt;S&gt;(S&, Int32&)**

Consumes elements until the specified sequence of elements has been encountered.

```csharp
bool Read<S>(S& expectedSequence, Int32& len)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The expected sequence.

`len` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The length of the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **TryRead&lt;S&gt;(S&, Int32&)**

Returns whether the remaining span contains the sequence, consumes the elements only if it does.

```csharp
bool TryRead<S>(S& expectedSequence, Int32& len)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The sequence of elements.

`len` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The length of the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **Peek&lt;S&gt;(S&, Int32&, Int32&)**

Returns whether the remaining span contains the sequence.

```csharp
bool Peek<S>(S& expectedSequence, Int32& head, Int32& len)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

`len` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The length of the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **ReadNext&lt;S&gt;(S&)**

Returns whether the following elements are the sequence, consumes elements.

```csharp
bool ReadNext<S>(S& expectedSequence)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **TryReadNext&lt;S&gt;(S&)**

Returns whether the following elements are the sequence, consumes elements only if the successful.

```csharp
bool TryReadNext<S>(S& expectedSequence)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **PeekNext&lt;S&gt;(S&, Int32&)**

Returns whether the following elements are the sequence.

```csharp
bool PeekNext<S>(S& expectedSequence, Int32& head)
```

#### Type Parameters

`S`<br>
The type of the sequence.

#### Parameters

`expectedSequence` S&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **Read(ReadOnlySpan`1&)**

Consumes elements until the specified sequence of elements has been encountered.

```csharp
bool Read(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The expected sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **TryRead(ReadOnlySpan`1&)**

Returns whether the remaining span contains the sequence, consumes the elements only if it does.

```csharp
bool TryRead(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **Peek(ReadOnlySpan`1&, Int32&)**

Returns whether the remaining span contains the sequence.

```csharp
bool Peek(ReadOnlySpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **ReadNext(ReadOnlySpan`1&)**

Returns whether the following elements are the sequence, consumes elements.

```csharp
bool ReadNext(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **TryReadNext(ReadOnlySpan`1&)**

Returns whether the following elements are the sequence, consumes elements only if the successful.

```csharp
bool TryReadNext(ReadOnlySpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **PeekNext(ReadOnlySpan`1&, Int32&)**

Returns whether the following elements are the sequence.

```csharp
bool PeekNext(ReadOnlySpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` ReadOnlySpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **ReadAny(TinySpan`1&)**

Consumes one element, and returns whether the element matches one of the expected elements.

```csharp
bool ReadAny(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **TryReadAny(TinySpan`1&)**

Returns whether the element matches one of the expected elements, and consumes the element only if it does.

```csharp
bool TryReadAny(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **PeekAny(TinySpan`1&)**

Returns whether the element matches one of the expected elements.

```csharp
bool PeekAny(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **ReadUntilAny(TinySpan`1&)**

Consumes elements until one of the expected elements occur.

```csharp
bool ReadUntilAny(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **TryReadUntilAny(TinySpan`1&)**

Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.

```csharp
bool TryReadUntilAny(TinySpan`1& expected)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **PeekUntilAny(TinySpan`1&, Int32&)**

Returns whether the remaining span contains one of the expected elements.

```csharp
bool PeekUntilAny(TinySpan`1& expected, Int32& head)
```

#### Parameters

`expected` TinySpan`1&<br>
The expected elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position at which the expected element occured.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .
