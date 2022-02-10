# Fmt

Namespace: Rustic.Text



```csharp
public class Fmt
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Fmt](./rustic.text.fmt.md)

## Properties

### **Definition**

The global instance containing definitions.

```csharp
public static Fmt Definition { get; }
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



```csharp
public static string Format<D>(ReadOnlySpan<char> format, D& definition, IEqualityComparer<char> comparer)
```

#### Type Parameters

`D`<br>

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`definition` D&<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Index(ReadOnlySpan&lt;Char&gt;, TinyVec&lt;Object&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**



```csharp
public string Index(ReadOnlySpan<char> format, TinyVec<object> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`arguments` TinyVec&lt;Object&gt;<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Named(ReadOnlySpan&lt;Char&gt;, IReadOnlyDictionary&lt;String, Object&gt;, IEqualityComparer&lt;Char&gt;, IFormatProvider)**



```csharp
public string Named(ReadOnlySpan<char> format, IReadOnlyDictionary<string, object> arguments, IEqualityComparer<char> comparer, IFormatProvider provider)
```

#### Parameters

`format` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`arguments` [IReadOnlyDictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

`provider` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
