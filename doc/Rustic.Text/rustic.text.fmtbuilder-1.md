# FmtBuilder&lt;D&gt;

Namespace: Rustic.Text

```csharp
public struct FmtBuilder<D>
```

#### Type Parameters

`D`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [FmtBuilder&lt;D&gt;](./rustic.text.fmtbuilder-1.md)

## Constructors

### **FmtBuilder(StrBuilder, ReadOnlySpan&lt;Char&gt;, D&, IEqualityComparer&lt;Char&gt;)**

```csharp
FmtBuilder(StrBuilder builder, ReadOnlySpan<char> input, D& definition, IEqualityComparer<char> comparer)
```

#### Parameters

`builder` StrBuilder<br>

`input` [ReadOnlySpan&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`definition` D&<br>

`comparer` [IEqualityComparer&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br>

## Methods

### **Next()**

```csharp
bool Next()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Dispose()**

```csharp
void Dispose()
```
