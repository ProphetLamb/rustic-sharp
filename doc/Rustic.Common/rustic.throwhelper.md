# ThrowHelper

Namespace: Rustic

Centralized functionality related to validation and throwing exceptions.

```csharp
public static class ThrowHelper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ThrowHelper](./rustic.throwhelper.md)

## Methods

### **ThrowArgumentException(String, String, Exception)**

```csharp
public static void ThrowArgumentException(string message, string name, Exception inner)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inner` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ThrowArgumentNullException(String, String)**

```csharp
public static void ThrowArgumentNullException(string name, string message)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ThrowArgumentOutOfRangeException(String, Object, String)**

```csharp
public static void ThrowArgumentOutOfRangeException(string name, object actual, string message)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`actual` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ThrowInvalidOperationException(String, Exception)**

```csharp
public static void ThrowInvalidOperationException(string message, Exception ex)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ThrowNotSupportedException(String, Exception)**

```csharp
public static void ThrowNotSupportedException(string message, Exception ex)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ValidateArg&lt;T&gt;(T, Boolean, String, String)**

```csharp
public static void ValidateArg<T>(T value, bool condition, string name, string message)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`condition` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ValidateArgRange&lt;T&gt;(T, Boolean, String, String)**

```csharp
public static void ValidateArgRange<T>(T value, bool condition, string name, string message)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`condition` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ValidateArgNotNull(Object, String)**

```csharp
public static void ValidateArgNotNull(object self, string name)
```

#### Parameters

`self` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
