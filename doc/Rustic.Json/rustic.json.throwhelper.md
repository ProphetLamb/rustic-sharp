# ThrowHelper

Namespace: Rustic.Json

Centralized functionality related to validation and throwing exceptions.

```csharp
public static class ThrowHelper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ThrowHelper](./rustic.json.throwhelper.md)

## Methods

### **ThrowJsonException(String, String, Exception)**

```csharp
public static void ThrowJsonException(string message, string path, Exception inner)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inner` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ThrowJsonUnexpectedTokenException(JsonTokenType, JsonTokenType, String, Exception)**

```csharp
public static void ThrowJsonUnexpectedTokenException(JsonTokenType expected, JsonTokenType actual, string path, Exception inner)
```

#### Parameters

`expected` JsonTokenType<br>

`actual` JsonTokenType<br>

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inner` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>
