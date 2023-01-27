# MemoryCopyHelper

Namespace: Rustic.Memory

Utility methods assisting with handling sections of memory

```csharp
public static class MemoryCopyHelper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MemoryCopyHelper](./rustic.memory.memorycopyhelper.md)

## Methods

### **CopyToReversed&lt;T&gt;(T&, T&, UIntPtr)**

Copies the source to the destination buffer, starting at the last element in source to the first element in destination.

```csharp
public static void CopyToReversed<T>(T& src, T& dst, UIntPtr len)
```

#### Type Parameters

`T`<br>
The type of the element to copy.

#### Parameters

`src` T&<br>
The source buffer

`dst` T&<br>
The destination buffer

`len` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The number of elements to copy. Must be less or equal then the minimum of both buffer lengths.

### **CopyToReversed&lt;T&gt;(ReadOnlySpan&lt;T&gt;, Span&lt;T&gt;)**

Copies the source to the destination buffer, starting at the last element in source to the first element in destination.

```csharp
public static void CopyToReversed<T>(ReadOnlySpan<T> src, Span<T> dst)
```

#### Type Parameters

`T`<br>
The type of the element to copy.

#### Parameters

`src` ReadOnlySpan&lt;T&gt;<br>
The source buffer. Must be smaller or equal in size to the destination.

`dst` Span&lt;T&gt;<br>
The destination buffer. Must be greater or equal in size ot the source.

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
If the source buffer is greater is size then the destination.

### **TryCopyToReversed&lt;T&gt;(ReadOnlySpan&lt;T&gt;, Span&lt;T&gt;)**

Copies the source to the destination buffer, starting at the last element in source to the first element in destination.

```csharp
public static bool TryCopyToReversed<T>(ReadOnlySpan<T> src, Span<T> dst)
```

#### Type Parameters

`T`<br>
The type of the element to copy.

#### Parameters

`src` ReadOnlySpan&lt;T&gt;<br>
The source buffer. Must be smaller or equal in size to the destination.

`dst` Span&lt;T&gt;<br>
The destination buffer. Must be greater or equal in size ot the source.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
If the source buffer is greater is size then the destination.
