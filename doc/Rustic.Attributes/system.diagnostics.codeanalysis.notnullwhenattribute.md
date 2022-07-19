# NotNullWhenAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that when a method returns [NotNullWhenAttribute.ReturnValue](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.notnullwhenattribute.returnvalue), the parameter will not be null even if the corresponding type allows it.

```csharp
public sealed class NotNullWhenAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [NotNullWhenAttribute](./system.diagnostics.codeanalysis.notnullwhenattribute.md)

## Properties

### **ReturnValue**

Gets the return value condition.

```csharp
public bool ReturnValue { get; }
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

### **NotNullWhenAttribute(Boolean)**

Initializes the attribute with the specified return value condition.

```csharp
public NotNullWhenAttribute(bool returnValue)
```

#### Parameters

`returnValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

            The return value condition. If the method returns this value, the associated parameter will not be null.
