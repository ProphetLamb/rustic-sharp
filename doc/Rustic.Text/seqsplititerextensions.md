# SeqSplitIterExtensions

Namespace:

Collection of extensions and utility functionality related to [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md).

```csharp
public static class SeqSplitIterExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SeqSplitIterExtensions](./seqsplititerextensions.md)

## Methods

### **ToArray&lt;S&gt;(SeqSplitIter&lt;Char, S&gt;)**



```csharp
public static String[] ToArray<S>(SeqSplitIter<char, S> self)
```

#### Type Parameters

`S`<br>

#### Parameters

`self` SeqSplitIter&lt;Char, S&gt;<br>

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ToArray&lt;T, O, S&gt;(SeqSplitIter&lt;T, S&gt;, Func&lt;O, T, O&gt;)**



```csharp
public static O[] ToArray<T, O, S>(SeqSplitIter<T, S> self, Func<O, T, O> aggregate)
```

#### Type Parameters

`T`<br>

`O`<br>

`S`<br>

#### Parameters

`self` SeqSplitIter&lt;T, S&gt;<br>

`aggregate` Func&lt;O, T, O&gt;<br>

#### Returns

O[]<br>

### **ToArray&lt;T, O, S&gt;(SeqSplitIter&lt;T, S&gt;, Func&lt;O, T, O&gt;, Func&lt;O&gt;)**



```csharp
public static O[] ToArray<T, O, S>(SeqSplitIter<T, S> self, Func<O, T, O> aggregate, Func<O> seed)
```

#### Type Parameters

`T`<br>

`O`<br>

`S`<br>

#### Parameters

`self` SeqSplitIter&lt;T, S&gt;<br>

`aggregate` Func&lt;O, T, O&gt;<br>

`seed` Func&lt;O&gt;<br>

#### Returns

O[]<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>

`separator` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>

`separator0` S&<br>

`separator1` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, S& separator2, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>

`separator0` S&<br>

`separator1` S&<br>

`separator2` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, S& separator2, S& separator3, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>

`separator0` S&<br>

`separator1` S&<br>

`separator2` S&<br>

`separator3` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, TinySpan&lt;S&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, TinySpan<S> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>

`separators` TinySpan&lt;S&gt;<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(Span&lt;T&gt;, S&)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, S& separator)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

`separator` S&<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(Span&lt;T&gt;, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, S& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

`separator` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(Span&lt;T&gt;, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, S& separator0, S& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

`separator0` S&<br>

`separator1` S&<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>

### **Split&lt;T, S&gt;(Span&lt;T&gt;, TinySpan&lt;S&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**



```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, TinySpan<S> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

`S`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

`separators` TinySpan&lt;S&gt;<br>

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
