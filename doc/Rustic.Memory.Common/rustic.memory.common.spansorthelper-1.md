# SpanSortHelper&lt;T&gt;

Namespace: Rustic.Memory.Common

Provides the function [SpanSortHelper&lt;T&gt;.Sort(Span&lt;T&gt;, Comparison&lt;T&gt;)](./rustic.memory.common.spansorthelper-1.md#sortspant-comparisont) with which a [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1) can be sorted.

```csharp
public static class SpanSortHelper<T>
```

#### Type Parameters

`T`<br>
The type of elements in the collection.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SpanSortHelper&lt;T&gt;](./rustic.memory.common.spansorthelper-1.md)

## Fields

### **IntrosortSizeThreshold**

This is the threshold where Introspective sort switches to Insertion sort.
 Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
 Large value types may benefit from a smaller number.

```csharp
public static int IntrosortSizeThreshold;
```

## Methods

### **Sort(Span&lt;T&gt;, Comparison&lt;T&gt;)**

Sorts the elements of the span using the comparer.

```csharp
public static void Sort(Span<T> keys, Comparison<T> comparer)
```

#### Parameters

`keys` Span&lt;T&gt;<br>
The span to sort.

`comparer` Comparison&lt;T&gt;<br>
The comparer used to sort the span.

### **InternalBinarySearch(T[], Int32, Int32, T, Comparison&lt;T&gt;)**

```csharp
internal static int InternalBinarySearch(T[] array, int index, int length, T value, Comparison<T> comparer)
```

#### Parameters

`array` T[]<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`value` T<br>

`comparer` Comparison&lt;T&gt;<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IntrospectiveSort(Span&lt;T&gt;, Comparison&lt;T&gt;)**

```csharp
internal static void IntrospectiveSort(Span<T> keys, Comparison<T> comparer)
```

#### Parameters

`keys` Span&lt;T&gt;<br>

`comparer` Comparison&lt;T&gt;<br>
