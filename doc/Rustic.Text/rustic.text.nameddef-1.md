# NamedDef&lt;T&gt;

Namespace: Rustic.Text

```csharp
public struct NamedDef<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [NamedDef&lt;T&gt;](./rustic.text.nameddef-1.md)<br>
Implements [IFmtDef](./rustic.text.ifmtdef.md)

## Properties

### **Prefix**

```csharp
public string Prefix { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Arguments**

```csharp
public IReadOnlyDictionary<string, T> Arguments { get; }
```

#### Property Value

IReadOnlyDictionary&lt;String, T&gt;<br>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Format**

```csharp
public IFormatProvider Format { get; }
```

#### Property Value

[IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

## Constructors

### **NamedDef(IReadOnlyDictionary&lt;String, T&gt;, IFormatProvider)**

```csharp
NamedDef(IReadOnlyDictionary<string, T> arguments, IFormatProvider format)
```

#### Parameters

`arguments` IReadOnlyDictionary&lt;String, T&gt;<br>

`format` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

### **NamedDef(String, IReadOnlyDictionary&lt;String, T&gt;, IFormatProvider)**

```csharp
NamedDef(string prefix, IReadOnlyDictionary<string, T> arguments, IFormatProvider format)
```

#### Parameters

`prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`arguments` IReadOnlyDictionary&lt;String, T&gt;<br>

`format` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

## Methods

### **NextTextEnd(Tokenizer`1&)**

```csharp
bool NextTextEnd(Tokenizer`1& tokenizer)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NextHoleBegin(Tokenizer`1&)**

```csharp
bool NextHoleBegin(Tokenizer`1& tokenizer)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NextHoleEnd(Tokenizer`1&)**

```csharp
bool NextHoleEnd(Tokenizer`1& tokenizer)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NextTextStart(Tokenizer`1&)**

```csharp
bool NextTextStart(Tokenizer`1& tokenizer)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TryGetValue(ReadOnlySpan`1&, ReadOnlySpan`1&)**

```csharp
bool TryGetValue(ReadOnlySpan`1& key, ReadOnlySpan`1& value)
```

#### Parameters

`key` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>

`value` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
