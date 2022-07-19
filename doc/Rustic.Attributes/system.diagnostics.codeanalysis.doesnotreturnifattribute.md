# DoesNotReturnIfAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that the method will not return if the associated Boolean parameter is passed the specified value.

```csharp
public sealed class DoesNotReturnIfAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [DoesNotReturnIfAttribute](./system.diagnostics.codeanalysis.doesnotreturnifattribute.md)

## Properties

### **ParameterValue**

Gets the condition parameter value.

```csharp
public bool ParameterValue { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

## Constructors

### **DoesNotReturnIfAttribute(Boolean)**

Initializes the attribute with the specified parameter value.

```csharp
public DoesNotReturnIfAttribute(bool parameterValue)
```

#### Parameters

`parameterValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

            The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
            the associated parameter matches this value.
