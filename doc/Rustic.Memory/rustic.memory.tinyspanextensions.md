# TinySpanExtensions

Namespace: Rustic.Memory

Collection of extensions and utility functionality for [TinySpan&lt;T&gt;](./rustic.memory.tinyspan-1.md).

```csharp
public static class TinySpanExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TinySpanExtensions](./rustic.memory.tinyspanextensions.md)

## Methods

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
