# Option

Namespace: Rustic



```csharp
public static class Option
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Option](./rustic.option.md)

## Methods

### **Some&lt;T&gt;(T&)**

Wraps the value in a [Option&lt;T&gt;](./rustic.option-1.md) some.

```csharp
public static Option<T> Some<T>(T& value)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T&<br>

#### Returns

Option&lt;T&gt;<br>

### **None&lt;T&gt;()**

Returns a new [Option&lt;T&gt;](./rustic.option-1.md) none.

```csharp
public static Option<T> None<T>()
```

#### Type Parameters

`T`<br>

#### Returns

Option&lt;T&gt;<br>

### **Flatten&lt;T&gt;(Option&lt;Option&lt;T&gt;&gt;)**

Flattens a nested [Option&lt;T&gt;](./rustic.option-1.md).

```csharp
public static Option<T> Flatten<T>(Option<Option<T>> self)
```

#### Type Parameters

`T`<br>

#### Parameters

`self` Option&lt;Option&lt;T&gt;&gt;<br>

#### Returns

Option&lt;T&gt;<br>

### **Unzip&lt;T, U&gt;(Option`1&)**

Unzips the option of a tuple to a tuple of options.

```csharp
public static ValueTuple<Option<T>, Option<U>> Unzip<T, U>(Option`1& self)
```

#### Type Parameters

`T`<br>

`U`<br>

#### Parameters

`self` Option`1&<br>

#### Returns

ValueTuple&lt;Option&lt;T&gt;, Option&lt;U&gt;&gt;<br>

### **TryInvoke(Action)**

Invokes the action, returns none if successful; otherwise, returns some [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception).

```csharp
public static Option<Exception> TryInvoke(Action action)
```

#### Parameters

`action` [Action](https://docs.microsoft.com/en-us/dotnet/api/system.action)<br>

#### Returns

[Option&lt;Exception&gt;](./rustic.option-1.md)<br>

### **TryInvoke(Func&lt;Task&gt;)**

Awaits invoking the action, returns none if successful; otherwise, returns some [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception).

```csharp
public static Task<Option<Exception>> TryInvoke(Func<Task> action)
```

#### Parameters

`action` [Func&lt;Task&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>

#### Returns

[Task&lt;Option&lt;Exception&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **TryInvoke&lt;E&gt;(Func&lt;Task&gt;)**

Awaits invoking the action, returns none if successful; otherwise, returns some exception .

```csharp
public static Task<Option<E>> TryInvoke<E>(Func<Task> action)
```

#### Type Parameters

`E`<br>

#### Parameters

`action` [Func&lt;Task&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>

#### Returns

Task&lt;Option&lt;E&gt;&gt;<br>

### **FilterMap&lt;T, U&gt;(IEnumerable&lt;T&gt;, Func&lt;T, Option&lt;U&gt;&gt;)**



```csharp
public static IEnumerable<U> FilterMap<T, U>(IEnumerable<T> sequence, Func<T, Option<U>> filterMap)
```

#### Type Parameters

`T`<br>

`U`<br>

#### Parameters

`sequence` IEnumerable&lt;T&gt;<br>

`filterMap` Func&lt;T, Option&lt;U&gt;&gt;<br>

#### Returns

IEnumerable&lt;U&gt;<br>
