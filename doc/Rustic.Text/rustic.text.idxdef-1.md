# IdxDef&lt;T&gt;

Namespace: Rustic.Text

Format definition for numeric index based formatting

```csharp
public struct IdxDef<T>
```

#### Type Parameters

`T`<br>
The type of the formatting arguments.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [IdxDef&lt;T&gt;](./rustic.text.idxdef-1.md)<br>
Implements [IFmtDef](./rustic.text.ifmtdef.md)

## Properties

### **Prefix**

The prefix required before curly bracket open to identify a hole.

```csharp
public string Prefix { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Arguments**

The formatting arguments used to fill holes in the format.

```csharp
public TinyRoVec<T> Arguments { get; }
```

#### Property Value

TinyRoVec&lt;T&gt;<br>

### **Count**

The number of formatting arguments.

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Format**

The formatter providing localization.

```csharp
public IFormatProvider Format { get; }
```

#### Property Value

[IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

## Constructors

### **IdxDef(TinyRoVec&lt;T&gt;, IFormatProvider)**

Initializes a new instance of [IdxDef&lt;T&gt;](./rustic.text.idxdef-1.md).

```csharp
IdxDef(TinyRoVec<T> arguments, IFormatProvider format)
```

#### Parameters

`arguments` TinyRoVec&lt;T&gt;<br>
The formatting arguments used to fill holes in the format.

`format` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>
The formatter providing localization.

### **IdxDef(String, TinyRoVec&lt;T&gt;, IFormatProvider)**

Initializes a new instance of [IdxDef&lt;T&gt;](./rustic.text.idxdef-1.md).

```csharp
IdxDef(string prefix, TinyRoVec<T> arguments, IFormatProvider format)
```

#### Parameters

`prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The prefix required before curly bracket open to identify a hole.

`arguments` TinyRoVec&lt;T&gt;<br>
The formatting arguments used to fill holes in the format.

`format` [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>
The formatter providing localization.

## Methods

### **NextTextEnd(Tokenizer`1&, StrBuilder&)**

```csharp
bool NextTextEnd(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

`builder` StrBuilder&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NextTextStart(Tokenizer`1&, StrBuilder&)**

```csharp
bool NextTextStart(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

`builder` StrBuilder&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **FinalTextEnd(Tokenizer`1&, StrBuilder&)**

```csharp
void FinalTextEnd(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>

`builder` StrBuilder&<br>

### **TryGetValue(ReadOnlySpan`1&, ReadOnlySpan`1&)**

```csharp
bool TryGetValue(ReadOnlySpan`1& key, ReadOnlySpan`1& value)
```

#### Parameters

`key` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>

`value` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;NextTextEnd&gt;g__determineHoleEnd|15_0(Char)**

```csharp
bool <NextTextEnd>g__determineHoleEnd|15_0(char c)
```

#### Parameters

`c` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;NextTextStart&gt;g__determineHoleEnd|16_0(Char)**

```csharp
bool <NextTextStart>g__determineHoleEnd|16_0(char c)
```

#### Parameters

`c` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;FinalTextEnd&gt;g__noop|17_0(Char)**

```csharp
bool <FinalTextEnd>g__noop|17_0(char _)
```

#### Parameters

`_` [Char](https://docs.microsoft.com/en-us/dotnet/api/system.char)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
