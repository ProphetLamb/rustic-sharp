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

**Remarks:**

Provides utility to read the next element, to read until an element occurs, read a sequence of elements, and to read until a sequence of elements occurs,
 in the modes TryRead, Peek, and Read:
 <br>
 TryRead only consumes elements only if successful.
 <br>
 Peek is [PureAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.contracts.pureattribute) and never consumes elements and informs about the position at which the condition was fulfilled.
 <br>
 Read always consumes elements, if not successful elements will still be consumed.

## Properties

### **Raw**

The reference to the source buffer.

```csharp
public ReadOnlySpan<T> Raw { get; }
```

#### Property Value

ReadOnlySpan&lt;T&gt;<br>

### **Comparer**

The comparer used to determine whether two elements are equal.

```csharp
public IEqualityComparer<T> Comparer { get; }
```

#### Property Value

IEqualityComparer&lt;T&gt;<br>

### **CursorPosition**

Defines the current cursor position of the iterator.

```csharp
public int CursorPosition { get; set; }
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

### **IsCursorEnd**

Indicates whether the end of the source sequence is reached.

```csharp
public bool IsCursorEnd { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsCursorStart**

Indicates whether the cursor is at the beginning of the source sequence.

```csharp
public bool IsCursorStart { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Length**

```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Token**

Represents the token currently being built from [Tokenizer&lt;T&gt;.Position](./rustic.text.tokenizer-1.md#position) to see [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition).

```csharp
public ReversibleIndexedSpan<T> Token { get; }
```

#### Property Value

ReversibleIndexedSpan&lt;T&gt;<br>

### **Current**

The reference to the current element at [Tokenizer&lt;T&gt;.Position](./rustic.text.tokenizer-1.md#position).

```csharp
public T& Current { get; }
```

#### Property Value

T&<br>

### **Cursor**

The reference to the next element after the cursor at index [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition).

```csharp
public T& Cursor { get; }
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

### **Tokenizer(ReversibleIndexedSpan&lt;T&gt;, IEqualityComparer&lt;T&gt;)**

Initializes a new instance of [Tokenizer&lt;T&gt;](./rustic.text.tokenizer-1.md) with the specified  and .

```csharp
Tokenizer(ReversibleIndexedSpan<T> input, IEqualityComparer<T> comparer)
```

#### Parameters

`input` ReversibleIndexedSpan&lt;T&gt;<br>
The input sequence.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

## Methods

### **GetAtCursor(Int32)**

Accesses the element at a specific offset from the [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition).

```csharp
T& GetAtCursor(int offset)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

T&<br>

### **TryGetAtCursor(Int32, T&)**

Attempts to obtain the element at a specific offset from the [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition).

```csharp
bool TryGetAtCursor(int offset, T& value)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The offset from the cursor

`value` T&<br>
The element at the offset, if any.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the element exists; otherwise false.

### **GetAtPosition(Int32)**

Accesses the element at a specific offset from the [Tokenizer&lt;T&gt;.Position](./rustic.text.tokenizer-1.md#position).

```csharp
T& GetAtPosition(int offset)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

T&<br>

### **TryGetAtPosition(Int32, T&)**

Attempts to obtain the element at a specific offset from the [Tokenizer&lt;T&gt;.Position](./rustic.text.tokenizer-1.md#position).

```csharp
bool TryGetAtPosition(int offset, T& value)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The offset from the position

`value` T&<br>
The element at the offset, if any.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the element exists; otherwise false.

### **Consume()**

Consumes one element.

```csharp
bool Consume()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element could be consumed; otherwise, .

### **Consume(Int32)**

Consumes a specified amount of elements. Moves the cursor by amount

```csharp
bool Consume(int amount)
```

#### Parameters

`amount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the elements could be consumed; otherwise, .

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
If `amount < -Token.Length`: Cannot move the cursor to before the start of the current token. The token length cannot be negative.

### **Dispose()**

```csharp
void Dispose()
```

### **FinalizeToken()**

Returns &amp; clears the [Tokenizer&lt;T&gt;.Token](./rustic.text.tokenizer-1.md#token), and moves the iterator to the first element after the token.

```csharp
ReversibleIndexedSpan<T> FinalizeToken()
```

#### Returns

ReversibleIndexedSpan&lt;T&gt;<br>
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

Advances the [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition) to a specific , always consumes elements.

```csharp
void Advance(int position)
```

#### Parameters

`position` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The target position

### **TryAdvance(Int32)**

Advances the [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition) to a specific , consumes elements only if successful.

```csharp
bool TryAdvance(int position)
```

#### Parameters

`position` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The target position

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the elements could be consumed; otherwise, .

**Remarks:**

If the target  is behind the [Tokenizer&lt;T&gt;.CursorPosition](./rustic.text.tokenizer-1.md#cursorposition) the state won't change.

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

### **Read(TinyRoSpan`1&)**

Consumes elements until the specified sequence of elements has been encountered.

```csharp
bool Read(TinyRoSpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The expected sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **TryRead(TinyRoSpan`1&)**

Returns whether the remaining span contains the sequence, consumes the elements only if it does.

```csharp
bool TryRead(TinyRoSpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **Peek(TinyRoSpan`1&, Int32&)**

Returns whether the remaining span contains the sequence.

```csharp
bool Peek(TinyRoSpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the remaining elements contain the sequence of elements; otherwise, .

### **ReadNext(TinyRoSpan`1&)**

Returns whether the following elements are the sequence, consumes elements.

```csharp
bool ReadNext(TinyRoSpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **TryReadNext(TinyRoSpan`1&)**

Returns whether the following elements are the sequence, consumes elements only if the successful.

```csharp
bool TryReadNext(TinyRoSpan`1& expectedSequence)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The sequence of elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **PeekNext(TinyRoSpan`1&, Int32&)**

Returns whether the following elements are the sequence.

```csharp
bool PeekNext(TinyRoSpan`1& expectedSequence, Int32& head)
```

#### Parameters

`expectedSequence` TinyRoSpan`1&<br>
The sequence of elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position of the element after the sequence.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the following elements are equal to the sequence of elements; otherwise, .

### **ReadAny(TinyRoSpan`1&)**

Consumes one element, and returns whether the element matches one of the expected elements.

```csharp
bool ReadAny(TinyRoSpan`1& expected)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **TryReadAny(TinyRoSpan`1&)**

Returns whether the element matches one of the expected elements, and consumes the element only if it does.

```csharp
bool TryReadAny(TinyRoSpan`1& expected)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **PeekAny(TinyRoSpan`1&)**

Returns whether the element matches one of the expected elements.

```csharp
bool PeekAny(TinyRoSpan`1& expected)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if the element is as expected; otherwise, .

### **ReadUntilAny(TinyRoSpan`1&)**

Consumes elements until one of the expected elements occur.

```csharp
bool ReadUntilAny(TinyRoSpan`1& expected)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **TryReadUntilAny(TinyRoSpan`1&)**

Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.

```csharp
bool TryReadUntilAny(TinyRoSpan`1& expected)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .

### **PeekUntilAny(TinyRoSpan`1&, Int32&)**

Returns whether the remaining span contains one of the expected elements.

```csharp
bool PeekUntilAny(TinyRoSpan`1& expected, Int32& head)
```

#### Parameters

`expected` TinyRoSpan`1&<br>
The expected elements.

`head` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The position at which the expected element occured.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
 if one or more of the expected elements occur in the remaining elements; otherwise, .
