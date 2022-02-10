# SplitOptions

Namespace: Rustic.Text

Defines how the results of the [SplitIter&lt;T&gt;](./rustic.text.splititer-1.md) are transformed.

```csharp
public enum SplitOptions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [SplitOptions](./rustic.text.splitoptions.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | Default behavior. No transformation. |
| RemoveEmptyEntries | 1 | Do not return zero-length segments. Instead return the next result, if any. |
| IncludeSeparator | 2 | Include the separator at the end of the resulting segment, if not at the end. |
| All | 255 | All options. |
