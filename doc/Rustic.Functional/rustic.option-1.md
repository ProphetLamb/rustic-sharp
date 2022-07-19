# Option&lt;T&gt;

Namespace: Rustic

Represents an optional value. Every [Option&lt;T&gt;](./rustic.option-1.md) is either  or .

```csharp
public struct Option<T>
```

#### Type Parameters

`T`<br>
The type of the value.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Option&lt;T&gt;](./rustic.option-1.md)<br>
Implements IEquatable&lt;Option&lt;T&gt;&gt;

## Properties

### **IsNone**

Returns `true` if the option contains a value.

```csharp
public bool IsNone { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsSome**

Returns `true` if the option does not contain a value.

```csharp
public bool IsSome { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **TrySome(T&)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome), unwraps the value.

```csharp
bool TrySome(T& value)
```

#### Parameters

`value` T&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SomeUnchecked()**

Returns the value without checking if the option contains a value.

```csharp
T SomeUnchecked()
```

#### Returns

T<br>

**Remarks:**

Undefined behaviour if there is no value.

### **SomeOrDefault()**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) returns the value; otherwise, the `default` of the type.

```csharp
T SomeOrDefault()
```

#### Returns

T<br>

### **Expect(String)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) returns the value; otherwise, throws a [InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception) with the  specified.

```csharp
T Expect(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

T<br>

### **SomeOr(T&)**

Coalesces the option with the fallback value specified.

```csharp
T SomeOr(T& other)
```

#### Parameters

`other` T&<br>

#### Returns

T<br>

### **SomeOr(Func&lt;T&gt;)**

Coalesces the option with the fallback value specified.

```csharp
T SomeOr(Func<T> other)
```

#### Parameters

`other` Func&lt;T&gt;<br>

#### Returns

T<br>

### **SomeOr(IntPtr)**

```csharp
T SomeOr(IntPtr other)
```

#### Parameters

`other` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

T<br>

### **Map&lt;U&gt;(Func&lt;T, U&gt;)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) maps the value.

```csharp
Option<U> Map<U>(Func<T, U> map)
```

#### Type Parameters

`U`<br>
The type to map to.

#### Parameters

`map` Func&lt;T, U&gt;<br>

#### Returns

Option&lt;U&gt;<br>

### **Map&lt;U&gt;(IntPtr)**

```csharp
Option<U> Map<U>(IntPtr map)
```

#### Type Parameters

`U`<br>

#### Parameters

`map` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

Option&lt;U&gt;<br>

### **MapOr&lt;U&gt;(Func&lt;T, U&gt;, U)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) maps the value; coalesces with the ault value.

```csharp
U MapOr<U>(Func<T, U> map, U def)
```

#### Type Parameters

`U`<br>
The type to map to.

#### Parameters

`map` Func&lt;T, U&gt;<br>

`def` U<br>

#### Returns

U<br>

### **MapOr&lt;U&gt;(IntPtr, U)**

```csharp
U MapOr<U>(IntPtr map, U def)
```

#### Type Parameters

`U`<br>

#### Parameters

`map` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`def` U<br>

#### Returns

U<br>

### **MapOr&lt;U&gt;(Func&lt;T, U&gt;, Func&lt;U&gt;)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) maps the value; coalesces with the ault value.

```csharp
U MapOr<U>(Func<T, U> map, Func<U> def)
```

#### Type Parameters

`U`<br>
The type to map to.

#### Parameters

`map` Func&lt;T, U&gt;<br>

`def` Func&lt;U&gt;<br>

#### Returns

U<br>

### **MapOr&lt;U&gt;(IntPtr, IntPtr)**

```csharp
U MapOr<U>(IntPtr map, IntPtr def)
```

#### Type Parameters

`U`<br>

#### Parameters

`map` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`def` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

U<br>

### **And&lt;U&gt;(Option&lt;U&gt;)**

If [Option&lt;T&gt;.IsNone](./rustic.option-1.md#isnone) returns None; otherwise, returns the  option.

```csharp
Option<U> And<U>(Option<U> other)
```

#### Type Parameters

`U`<br>
The of the other value.

#### Parameters

`other` Option&lt;U&gt;<br>

#### Returns

Option&lt;U&gt;<br>

### **And&lt;U&gt;(Func&lt;Option&lt;U&gt;&gt;)**

If [Option&lt;T&gt;.IsNone](./rustic.option-1.md#isnone) returns None; otherwise, returns the  option.

```csharp
Option<U> And<U>(Func<Option<U>> other)
```

#### Type Parameters

`U`<br>
The of the other value.

#### Parameters

`other` Func&lt;Option&lt;U&gt;&gt;<br>

#### Returns

Option&lt;U&gt;<br>

### **And&lt;U&gt;(IntPtr)**

```csharp
Option<U> And<U>(IntPtr other)
```

#### Type Parameters

`U`<br>

#### Parameters

`other` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

Option&lt;U&gt;<br>

### **Or(Option&lt;T&gt;)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) returns this; otherwise, returns the  option.

```csharp
Option<T> Or(Option<T> other)
```

#### Parameters

`other` [Option&lt;T&gt;](./rustic.option-1.md)<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Or(Func&lt;Option&lt;T&gt;&gt;)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) returns this; otherwise, returns the  option.

```csharp
Option<T> Or(Func<Option<T>> other)
```

#### Parameters

`other` Func&lt;Option&lt;T&gt;&gt;<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Or(IntPtr)**

```csharp
Option<T> Or(IntPtr other)
```

#### Parameters

`other` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Xor(Option&lt;T&gt;)**

Returns Some if exactly one of this, and  is Some, otherwise returns None.

```csharp
Option<T> Xor(Option<T> other)
```

#### Parameters

`other` [Option&lt;T&gt;](./rustic.option-1.md)<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Where(Predicate&lt;T&gt;)**

If [Option&lt;T&gt;.IsSome](./rustic.option-1.md#issome) and the filter applies to the value returns Some; otherwise returns None.

```csharp
Option<T> Where(Predicate<T> filter)
```

#### Parameters

`filter` Predicate&lt;T&gt;<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Where(IntPtr)**

```csharp
Option<T> Where(IntPtr filter)
```

#### Parameters

`filter` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[Option&lt;T&gt;](./rustic.option-1.md)<br>

### **Zip&lt;U&gt;(Option&lt;U&gt;)**

Zips the option with another; otherwise, returns None.

```csharp
Option<ValueTuple<T, U>> Zip<U>(Option<U> other)
```

#### Type Parameters

`U`<br>

#### Parameters

`other` Option&lt;U&gt;<br>

#### Returns

Option&lt;ValueTuple&lt;T, U&gt;&gt;<br>

### **Zip&lt;U, R&gt;(Option&lt;U&gt;, Func&lt;T, U, R&gt;)**

Zips the option with another then maps the value; otherwise, returns None.

```csharp
Option<R> Zip<U, R>(Option<U> other, Func<T, U, R> map)
```

#### Type Parameters

`U`<br>

`R`<br>

#### Parameters

`other` Option&lt;U&gt;<br>

`map` Func&lt;T, U, R&gt;<br>

#### Returns

Option&lt;R&gt;<br>

### **Zip&lt;U, R&gt;(Option&lt;U&gt;, IntPtr)**

```csharp
Option<R> Zip<U, R>(Option<U> other, IntPtr map)
```

#### Type Parameters

`U`<br>

`R`<br>

#### Parameters

`other` Option&lt;U&gt;<br>

`map` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

Option&lt;R&gt;<br>

### **Equals(T&)**

Returns `true` if Some value is equal to the ; otherwise, `false`.

```csharp
bool Equals(T& value)
```

#### Parameters

`value` T&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(T&, IEqualityComparer&lt;T&gt;)**

Returns `true` if Some value is equal to the ; otherwise, `false`. Used the  to compare the values.

```csharp
bool Equals(T& value, IEqualityComparer<T> comparer)
```

#### Parameters

`value` T&<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Option&lt;T&gt;)**

```csharp
bool Equals(Option<T> other)
```

#### Parameters

`other` [Option&lt;T&gt;](./rustic.option-1.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Option&lt;T&gt;, IEqualityComparer&lt;T&gt;)**

```csharp
bool Equals(Option<T> other, IEqualityComparer<T> comparer)
```

#### Parameters

`other` [Option&lt;T&gt;](./rustic.option-1.md)<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
