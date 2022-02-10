# ReadOnlyVectorTraits

Namespace: Rustic.Memory

Collection of extensions and utility functions related to [IReadOnlyVector&lt;T&gt;](./rustic.memory.ireadonlyvector-1.md).

```csharp
public static class ReadOnlyVectorTraits
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyVectorTraits](./rustic.memory.readonlyvectortraits.md)

## Methods

### **AsSpan&lt;T&gt;(IReadOnlyVector&lt;T&gt;)**

Creates a new span over a target vector.

```csharp
public static ReadOnlySpan<T> AsSpan<T>(IReadOnlyVector<T> self)
```

#### Type Parameters

`T`<br>
The type of elements in the vector.

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

#### Returns

ReadOnlySpan&lt;T&gt;<br>
The span representation of the vector.

### **IndexOf&lt;T&gt;(IReadOnlyVector&lt;T&gt;, T&)**



```csharp
public static int IndexOf<T>(IReadOnlyVector<T> self, T& item)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IndexOf&lt;T, E&gt;(IReadOnlyVector&lt;T&gt;, T&, E&)**



```csharp
public static int IndexOf<T, E>(IReadOnlyVector<T> self, T& item, E& comparer)
```

#### Type Parameters

`T`<br>

`E`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

`comparer` E&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf&lt;T&gt;(IReadOnlyVector&lt;T&gt;, T&)**



```csharp
public static int LastIndexOf<T>(IReadOnlyVector<T> self, T& item)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LastIndexOf&lt;T, E&gt;(IReadOnlyVector&lt;T&gt;, T&, E)**



```csharp
public static int LastIndexOf<T, E>(IReadOnlyVector<T> self, T& item, E comparer)
```

#### Type Parameters

`T`<br>

`E`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

`comparer` E<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch&lt;T&gt;(IReadOnlyVector&lt;T&gt;, T&)**



```csharp
public static int BinarySearch<T>(IReadOnlyVector<T> self, T& item)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BinarySearch&lt;T, C&gt;(IReadOnlyVector&lt;T&gt;, T&, C)**



```csharp
public static int BinarySearch<T, C>(IReadOnlyVector<T> self, T& item, C comparer)
```

#### Type Parameters

`T`<br>

`C`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

`comparer` C<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **CopyTo&lt;T&gt;(IReadOnlyVector&lt;T&gt;, Span&lt;T&gt;)**



```csharp
public static void CopyTo<T>(IReadOnlyVector<T> self, Span<T> destination)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`destination` Span&lt;T&gt;<br>

### **Contains&lt;T&gt;(IReadOnlyVector&lt;T&gt;, T&)**



```csharp
public static bool Contains<T>(IReadOnlyVector<T> self, T& item)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` IReadOnlyVector&lt;T&gt;<br>

`item` T&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
