# UnreachableException

Namespace: System.Diagnostics

Exception thrown when the program executes an instruction that was thought to be unreachable.

```csharp
public sealed class UnreachableException : System.Exception, System.Runtime.Serialization.ISerializable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception) → [UnreachableException](./system.diagnostics.unreachableexception.md)<br>
Implements [ISerializable](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodbase)<br>

### **Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br>

### **InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **UnreachableException()**

Initializes a new instance of the [UnreachableException](./system.diagnostics.unreachableexception.md) class with the default error message.

```csharp
public UnreachableException()
```

### **UnreachableException(String)**

Initializes a new instance of the [UnreachableException](./system.diagnostics.unreachableexception.md)
 class with a specified error message.

```csharp
public UnreachableException(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The error message that explains the reason for the exception.

### **UnreachableException(String, Exception)**

Initializes a new instance of the [UnreachableException](./system.diagnostics.unreachableexception.md)
 class with a specified error message and a reference to the inner exception that is the cause of
 this exception.

```csharp
public UnreachableException(string message, Exception innerException)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The error message that explains the reason for the exception.

`innerException` [Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>
The exception that is the cause of the current exception.

## Methods

### **Throw()**

Throws a new instance of [UnreachableException](./system.diagnostics.unreachableexception.md).

```csharp
public static void Throw()
```

#### Exceptions

[UnreachableException](./system.diagnostics.unreachableexception.md)<br>

### **Throw&lt;T&gt;()**

```csharp
public static T Throw<T>()
```

#### Type Parameters

`T`<br>

#### Returns

T<br>
