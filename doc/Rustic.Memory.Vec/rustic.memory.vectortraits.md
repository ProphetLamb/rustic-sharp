# VectorTraits

Namespace: Rustic.Memory

Collection of extensions and utility functions related to [IVector&lt;T&gt;](./rustic.memory.ivector-1.md).

```csharp
public static class VectorTraits
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [VectorTraits](./rustic.memory.vectortraits.md)

## Methods

### **AddRange&lt;T&gt;(IVector&lt;T&gt;, ReadOnlySpan&lt;T&gt;)**

```csharp
public static void AddRange<T>(IVector<T> self, ReadOnlySpan<T> values)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

`values` ReadOnlySpan&lt;T&gt;<br>

### **Remove&lt;T&gt;(IVector&lt;T&gt;, T&)**

```csharp
public static bool Remove<T>(IVector<T> self, T& item)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

`item` T&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Remove&lt;T, E&gt;(IVector&lt;T&gt;, T&, E)**

```csharp
public static bool Remove<T, E>(IVector<T> self, T& item, E comparer)
```

#### Type Parameters

`T`<br>

`E`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

`item` T&<br>

`comparer` E<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SwapRemove&lt;T&gt;(IVector&lt;T&gt;, Int32)**

Removes the element at the specified  from the vector by over-writing it with the last element.

```csharp
public static void SwapRemove<T>(IVector<T> self, int index)
```

#### Type Parameters

`T`<br>
The type of elements in the vector.

#### Parameters

`self` IVector&lt;T&gt;<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

**Remarks:**

No block of elements in moved. The order of the vector is disturbed.

### **Sort&lt;T&gt;(IVector&lt;T&gt;)**

```csharp
public static void Sort<T>(IVector<T> self)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

### **Sort&lt;T, C&gt;(IVector&lt;T&gt;, C)**

```csharp
public static void Sort<T, C>(IVector<T> self, C comparer)
```

#### Type Parameters

`T`<br>

`C`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

`comparer` C<br>

### **Sort&lt;T&gt;(IVector&lt;T&gt;, Comparison&lt;T&gt;)**

```csharp
public static void Sort<T>(IVector<T> self, Comparison<T> comparison)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

`comparison` Comparison&lt;T&gt;<br>

### **Reverse&lt;T&gt;(IVector&lt;T&gt;)**

```csharp
public static void Reverse<T>(IVector<T> self)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IVector&lt;T&gt;<br>

### **TryPop&lt;T&gt;(IVector&lt;T&gt;, T&)**

Attempts to remove the topmost element from the stack.

```csharp
public static bool TryPop<T>(IVector<T> self, T& value)
```

#### Type Parameters

`T`<br>
The type of elements in the stack

#### Parameters

`self` IVector&lt;T&gt;<br>
The stack

`value` T&<br>
The value removed from the stack, or default

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if a value was removed; otherwise false
