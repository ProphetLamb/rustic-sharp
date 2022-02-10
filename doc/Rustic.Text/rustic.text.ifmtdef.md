# IFmtDef

Namespace: Rustic.Text



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
