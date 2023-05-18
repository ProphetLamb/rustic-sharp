# Fmt

Namespace: Rustic.Text

Allows dynamic string formatting according the templates.

```csharp
public sealed class Fmt
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Fmt](./rustic.text.fmt.md)

## Properties

### **Def**

The global instance containing definitions.

```csharp
public static Fmt Def { get; }
```

#### Property Value

[Fmt](./rustic.text.fmt.md)<br>

## Constructors

### **Fmt()**

```csharp
public Fmt()
```

## Methods

### **Format&lt;D&gt;(ReadOnlySpan&lt;Char&gt;, D&, IEqualityComparer&lt;Char&gt;)**

Formats a string using the specified definition.

```csharp
public static string Format<D>(ReadOnlySpan<char> format, D& definition, IEqualityComparer<char> comparer)
```

#### Type Parameters

`D`<br>
The type of the definition.

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The format string.

`definition` D&<br>
The format definition.

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>
The comparer determining whether two chars are equal.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The formatted string.

### **Index(ReadOnlySpan&lt;Char&gt;, TinyRoVec&lt;Object&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**

Formats a string using index based definitions.

```csharp
public string Index(ReadOnlySpan<char> format, TinyRoVec<object> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The format string

`arguments` TinyRoVec&lt;Object&gt;<br>
The format arguments.

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>
The comparer determining whether two chars are equal.

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>
The localizing formatter used.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The formatted string.

### **Index&lt;T&gt;(ReadOnlySpan&lt;Char&gt;, TinyRoVec&lt;T&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**

```csharp
public string Index<T>(ReadOnlySpan<char> format, TinyRoVec<T> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Type Parameters

`T`<br>

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`arguments` TinyRoVec&lt;T&gt;<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Named(ReadOnlySpan&lt;Char&gt;, IReadOnlyDictionary&lt;String, Object&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**

Formats a string using name based definitions.

```csharp
public string Named(ReadOnlySpan<char> format, IReadOnlyDictionary<string, object> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The format string

`arguments` [IReadOnlyDictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>
The format arguments.

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>
The comparer determining whether two chars are equal.

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>
The localizing formatter used.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The formatted string.

### **Named&lt;T&gt;(ReadOnlySpan&lt;Char&gt;, IReadOnlyDictionary&lt;String, T&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**

```csharp
public string Named<T>(ReadOnlySpan<char> format, IReadOnlyDictionary<string, T> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Type Parameters

`T`<br>

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`arguments` IReadOnlyDictionary&lt;String, T&gt;<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
