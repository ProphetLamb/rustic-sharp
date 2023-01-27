# IFmtDef

Namespace: Rustic.Text

Interface used to identify a format defintion.

```csharp
public interface IFmtDef
```

## Properties

### **Format**

The format provider used for formatting.

```csharp
public abstract IFormatProvider Format { get; }
```

#### Property Value

[IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)<br>

### **Count**

The number of formatting arguments.

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **NextTextEnd(Tokenizer`1&, StrBuilder&)**

Moves the tokenizer to the char at which this text portion ended. Consumes the hole indicating characters. Adds the relevant text to the builder.

```csharp
bool NextTextEnd(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>
The tokenizer used.

`builder` StrBuilder&<br>
The builder used to collect text.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if successful; otherwise false.

### **NextTextStart(Tokenizer`1&, StrBuilder&)**

Moves the tokenizer to the char at which the next text portion begins. Consumes the hole terminating characters. Adds the relevant hole definition to the builder.

```csharp
bool NextTextStart(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>
The tokenizer used.

`builder` StrBuilder&<br>
The builder used to collect the hole definition

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if successful; otherwise false.

### **FinalTextEnd(Tokenizer`1&, StrBuilder&)**

Moves the tokenizer to the end of the format string. Ensures that the tokenizer cursor is at the end and empty.

```csharp
void FinalTextEnd(Tokenizer`1& tokenizer, StrBuilder& builder)
```

#### Parameters

`tokenizer` [Tokenizer`1&](./rustic.text.tokenizer-1&.md)<br>
The tokenizer used.

`builder` StrBuilder&<br>
The builder used to collect text.

#### Exceptions

[FormatException](https://docs.microsoft.com/en-us/dotnet/api/system.formatexception)<br>
The format string is invalid or not fully processed.

### **TryGetValue(ReadOnlySpan`1&, ReadOnlySpan`1&)**

Attempts to resolve the  using the formatting arguments.

```csharp
bool TryGetValue(ReadOnlySpan`1& key, ReadOnlySpan`1& value)
```

#### Parameters

`key` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>
The key.

`value` [ReadOnlySpan`1&](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1&)<br>
The value representing the key.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the  is found; otherwise, false.
