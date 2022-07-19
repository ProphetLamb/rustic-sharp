# CallerArgumentExpressionAttribute

Namespace: System.Runtime.CompilerServices

Indicates that a parameter captures the expression passed for another parameter as a string.

```csharp
public sealed class CallerArgumentExpressionAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [CallerArgumentExpressionAttribute](./system.runtime.compilerservices.callerargumentexpressionattribute.md)

## Properties

### **ParameterName**

Gets the name of the parameter whose expression should be captured as a string.

```csharp
public string ParameterName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

## Constructors

### **CallerArgumentExpressionAttribute(String)**

Initializes a new instance of the [CallerArgumentExpressionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute) class.

```csharp
public CallerArgumentExpressionAttribute(string parameterName)
```

#### Parameters

`parameterName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The name of the parameter whose expression should be captured as a string.
