# SplitIterExtensions

Namespace: Rustic.Text

Collection of extensions and utility functionality related to [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md).

```csharp
public static class SplitIterExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SplitIterExtensions](./rustic.text.splititerextensions.md)

## Methods

### **ToArray(SplitIter&lt;Char&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array.

```csharp
public static String[] ToArray(SplitIter<char> self)
```

#### Parameters

`self` [SplitIter&lt;Char&gt;](./rustic.text.splititer-1.md)<br>

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

**Remarks:**

Does consume from the iterator.

### **ToArray&lt;T, O&gt;(SplitIter&lt;T&gt;, Func&lt;O, T, O&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array, aggregating the slice of elements.

```csharp
public static O[] ToArray<T, O>(SplitIter<T> self, Func<O, T, O> aggregate)
```

#### Type Parameters

`T`<br>
The type of elements in the collection.

`O`<br>
The type of the aggregation of elements.

#### Parameters

`self` SplitIter&lt;T&gt;<br>
The iterator to aggregate.

`aggregate` Func&lt;O, T, O&gt;<br>
The function used to aggregate the items in each iterator element.

#### Returns

O[]<br>

### **ToArray&lt;T, O&gt;(SplitIter&lt;T&gt;, Func&lt;O, T, O&gt;, Func&lt;O&gt;)**

Enumerates the remaining elements in the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) into a array, aggregating the slice of elements.

```csharp
public static O[] ToArray<T, O>(SplitIter<T> self, Func<O, T, O> aggregate, Func<O> seed)
```

#### Type Parameters

`T`<br>
The type of elements in the collection.

`O`<br>
The type of the aggregation of elements.

#### Parameters

`self` SplitIter&lt;T&gt;<br>
The iterator to aggregate.

`aggregate` Func&lt;O, T, O&gt;<br>
The function used to aggregate the items in each iterator element.

`seed` Func&lt;O&gt;<br>
The initial value used for aggregating each element.

#### Returns

O[]<br>

### **Split&lt;T&gt;(ReadOnlySpan&lt;T&gt;, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SplitIter<T> Split<T>(ReadOnlySpan<T> span, T& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator` T&<br>
The separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(ReadOnlySpan&lt;T&gt;, T&, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SplitIter<T> Split<T>(ReadOnlySpan<T> span, T& separator0, T& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` T&<br>
The fist separator by which to split the .

`separator1` T&<br>
The second separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(ReadOnlySpan&lt;T&gt;, T&, T&, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SplitIter<T> Split<T>(ReadOnlySpan<T> span, T& separator0, T& separator1, T& separator2, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` T&<br>
The fist separator by which to split the .

`separator1` T&<br>
The second separator by which to split the .

`separator2` T&<br>
The third separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(ReadOnlySpan&lt;T&gt;, T&, T&, T&, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SplitIter<T> Split<T>(ReadOnlySpan<T> span, T& separator0, T& separator1, T& separator2, T& separator3, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separator0` T&<br>
The fist separator by which to split the .

`separator1` T&<br>
The second separator by which to split the .

`separator2` T&<br>
The third separator by which to split the .

`separator3` T&<br>
The third separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(ReadOnlySpan&lt;T&gt;, TinyRoSpan&lt;T&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SplitIter<T> Split<T>(ReadOnlySpan<T> span, TinyRoSpan<T> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` ReadOnlySpan&lt;T&gt;<br>
The span.

`separators` TinyRoSpan&lt;T&gt;<br>
The separators by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  with the specified parameters.

### **Split&lt;T&gt;(Span&lt;T&gt;, T&)**

Splits the  span at the positions defined by the .

```csharp
public static SplitIter<T> Split<T>(Span<T> span, T& separator)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separator` T&<br>
The separator by which to split the .

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(Span&lt;T&gt;, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SplitIter<T> Split<T>(Span<T> span, T& separator, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separator` T&<br>
The separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(Span&lt;T&gt;, T&, T&, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the separators.

```csharp
public static SplitIter<T> Split<T>(Span<T> span, T& separator0, T& separator1, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separator0` T&<br>
The fist separator by which to split the .

`separator1` T&<br>
The second separator by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.

### **Split&lt;T&gt;(Span&lt;T&gt;, TinyRoSpan&lt;T&gt;, SplitOptions, IEqualityComparer&lt;T&gt;)**

Splits the  span at the positions defined by the .

```csharp
public static SplitIter<T> Split<T>(Span<T> span, TinyRoSpan<T> separators, SplitOptions options, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the elements of the .

#### Parameters

`span` Span&lt;T&gt;<br>
The span.

`separators` TinyRoSpan&lt;T&gt;<br>
The separators by which to split the .

`options` [SplitOptions](./rustic.text.splitoptions.md)<br>
The options defining how to return the segments.

`comparer` IEqualityComparer&lt;T&gt;<br>
The comparer used to determine whether two objects are equal.

#### Returns

SplitIter&lt;T&gt;<br>
The iterator splitting the  span with the specified parameters.
