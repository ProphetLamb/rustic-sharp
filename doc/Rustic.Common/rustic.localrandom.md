# LocalRandom

Namespace: Rustic

Collection of extensions and utility functionality related to [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random) instances.

```csharp
public static class LocalRandom
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LocalRandom](./rustic.localrandom.md)

## Properties

### **Shared**

Gets the thread-local random pool.

```csharp
public static Random Shared { get; }
```

#### Property Value

[Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

### **PosixPortable**

Posix portable file name characters.

```csharp
public static ReadOnlySpan<char> PosixPortable { get; }
```

#### Property Value

[ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

## Methods

### **ChooseFrom&lt;T&gt;(Random, ReadOnlySpan&lt;T&gt;)**

Chooses a element from the collection using the random.

```csharp
public static T& ChooseFrom<T>(Random random, ReadOnlySpan<T> collection)
```

#### Type Parameters

`T`<br>
The type of the value.

#### Parameters

`random` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

`collection` ReadOnlySpan&lt;T&gt;<br>

#### Returns

T&<br>

### **ChooseFrom&lt;T&gt;(Random, IReadOnlyList&lt;T&gt;)**

Chooses a element from the collection using the random.

```csharp
public static T ChooseFrom<T>(Random random, IReadOnlyList<T> collection)
```

#### Type Parameters

`T`<br>
The type of the value.

#### Parameters

`random` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

`collection` IReadOnlyList&lt;T&gt;<br>

#### Returns

T<br>

### **GetString(Random, ReadOnlySpan&lt;Char&gt;, Int32)**

Returns a random string of a specific length, with characters exclusively from an alphabet.

```csharp
public static string GetString(Random random, ReadOnlySpan<char> alphabet, int length)
```

#### Parameters

`random` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

`alphabet` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ChooseMany&lt;T&gt;(Random, IReadOnlyList&lt;T&gt;, Int32)**

Chooses a number of elements from the collection.

```csharp
public static IEnumerable<T> ChooseMany<T>(Random random, IReadOnlyList<T> collection, int number)
```

#### Type Parameters

`T`<br>
The type of the value.

#### Parameters

`random` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

`collection` IReadOnlyList&lt;T&gt;<br>

`number` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

IEnumerable&lt;T&gt;<br>
