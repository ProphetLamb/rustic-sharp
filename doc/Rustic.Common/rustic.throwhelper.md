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

### **ThrowFormatException(String, Exception)**

```csharp
public static void ThrowFormatException(string message, Exception ex)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ThrowFormatException(Int32, Int32, String, Exception)**

```csharp
public static void ThrowFormatException(int start, int end, string message, Exception ex)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`end` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ArgumentIs&lt;T&gt;(T, Boolean, String, String)**

```csharp
public static void ArgumentIs<T>(T value, bool condition, string name, string message)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`condition` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ArgumentInRange&lt;T&gt;(T, Boolean, String, String)**

```csharp
public static void ArgumentInRange<T>(T value, bool condition, string name, string message)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`condition` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ArgumentNotNull(Object, String)**

```csharp
public static void ArgumentNotNull(object self, string name)
```

#### Parameters

`self` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ThrowKeyNotFoundException(String, Exception)**

```csharp
public static void ThrowKeyNotFoundException(string message, Exception ex)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ThrowUnreachableException(String, Exception)**

```csharp
public static void ThrowUnreachableException(string message, Exception ex)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ex` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **ObjectDisposedException&lt;T&gt;(String)**

```csharp
public static void ObjectDisposedException<T>(string message)
```

#### Type Parameters

`T`<br>

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ObjectDisposedException(String, String)**

```csharp
public static void ObjectDisposedException(string typeName, string message)
```

#### Parameters

`typeName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
