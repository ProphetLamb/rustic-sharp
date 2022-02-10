# RefVecExtensions

Namespace: Rustic.Memory

Collection of extensions and utility functions related to [RefVec&lt;T&gt;](./rustic.memory.refvec-1.md).

```csharp
public static class RefVecExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RefVecExtensions](./rustic.memory.refvecextensions.md)

## Methods

### **SequenceEqual&lt;T&gt;(RefVec&lt;T&gt;, RefVec&lt;T&gt;)**

Determines whether two lists are equal by comparing the elements using IEquatable{T}.Equals(T).

```csharp
public static bool SequenceEqual<T>(RefVec<T> list, RefVec<T> other)
```

#### Type Parameters

`T`<br>

#### Parameters

`list` RefVec&lt;T&gt;<br>

`other` RefVec&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SequenceCompareTo&lt;T&gt;(RefVec&lt;T&gt;, RefVec&lt;T&gt;)**

Determines the relative order of the lists being compared by comparing the elements using IComparable{T}.CompareTo(T).

```csharp
public static int SequenceCompareTo<T>(RefVec<T> list, RefVec<T> other)
```

#### Type Parameters

`T`<br>

#### Parameters

`list` RefVec&lt;T&gt;<br>

`other` RefVec&lt;T&gt;<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SequenceCompareHelper&lt;T&gt;(T&, Int32, T&, Int32, IComparer&lt;T&gt;)**



```csharp
internal static int SequenceCompareHelper<T>(T& first, int firstLength, T& second, int secondLength, IComparer<T> comparer)
```

#### Type Parameters

`T`<br>

#### Parameters

`first` T&<br>

`firstLength` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`second` T&<br>

`secondLength` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`comparer` IComparer&lt;T&gt;<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
