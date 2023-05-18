# SeqSplitIterExtensions

Namespace: Rustic.Text

Collection of extensions and utility functionality related to [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md).

```csharp
public static class SeqSplitIterExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SeqSplitIterExtensions](./rustic.text.seqsplititerextensions.md)

## Methods

### **ToArray&lt;S&gt;(SeqSplitIter&lt;Char, S&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array.

```csharp
public static String[] ToArray<S>(SeqSplitIter<char, S> self)
```

#### Type Parameters

`S`<br>
The type of a sequence of chars.

#### Parameters

`self` SeqSplitIter&lt;Char, S&gt;<br>

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

**Remarks:**

Does consume from the iterator.

### **ToArray&lt;T, O, S&gt;(SeqSplitIter&lt;T, S&gt;, Func&lt;O, T, O&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array, aggregating the slice of elements.

```csharp
public static O[] ToArray<T, O, S>(SeqSplitIter<T, S> self, Func<O, T, O> aggregate)
```

#### Type Parameters

`T`<br>
The type of elements in the collection.

`O`<br>
The type of the aggregation of elements.

`S`<br>
The type of a sequence of elements.

#### Parameters

`self` SeqSplitIter&lt;T, S&gt;<br>
The iterator to aggregate.

`aggregate` Func&lt;O, T, O&gt;<br>
The function used to aggregate the items in each iterator element.

#### Returns

O[]<br>

### **ToArray&lt;T, O, S&gt;(SeqSplitIter&lt;T, S&gt;, Func&lt;O, T, O&gt;, Func&lt;O&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array, aggregating the slice of elements.

```csharp
public static O[] ToArray<T, O, S>(SeqSplitIter<T, S> self, Func<O, T, O> aggregate, Func<O> seed)
```

#### Type Parameters

`T`<br>
The type of elements in the collection.

`O`<br>
The type of the aggregation of elements.

`S`<br>
The type of a sequence of elements.

#### Parameters

`self` SeqSplitIter&lt;T, S&gt;<br>
The iterator to aggregate.

`aggregate` Func&lt;O, T, O&gt;<br>
The function used to aggregate the items in each iterator element.

`seed` Func&lt;O&gt;<br>
The initial value used for aggregating each element.

#### Returns

O[]<br>

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator` S&<br>
The separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` S&<br>
The fist separator by which to split the .

`separator1` S&<br>
The second separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, S& separator2, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` S&<br>
The fist separator by which to split the .

`separator1` S&<br>
The second separator by which to split the .

`separator2` S&<br>
The third separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, S&, S&, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, S& separator0, S& separator1, S& separator2, S& separator3, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` S&<br>
The fist separator by which to split the .

`separator1` S&<br>
The second separator by which to split the .

`separator2` S&<br>
The third separator by which to split the .

`separator3` S&<br>
The third separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(ReadOnlySpan&lt;T&gt;, TinyRoSpan&lt;S&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SeqSplitIter<T, S> Split<T, S>(ReadOnlySpan<T> span, TinyRoSpan<S> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separators` TinyRoSpan&lt;S&gt;<br>
The separators by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  with the specified parameters.

### **Split&lt;T, S&gt;(Span&lt;T&gt;, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, S& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separator` S&<br>
The separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(Span&lt;T&gt;, S&, S&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, S& separator0, S& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separator0` S&<br>
The fisS separator by which to split the .

`separator1` S&<br>
The second separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T, S&gt;(Span&lt;T&gt;, TinyRoSpan&lt;S&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SeqSplitIter<T, S> Split<T, S>(Span<T> span, TinyRoSpan<S> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

`S`<br>
The type of a sequence of elements.

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separators` TinyRoSpan&lt;S&gt;<br>
The separators by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SeqSplitIter&lt;T, S&gt;<br>
The iterator splitting the  span with the specified parameters.
