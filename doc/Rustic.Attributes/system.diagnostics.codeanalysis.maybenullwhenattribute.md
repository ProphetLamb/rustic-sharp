# MaybeNullWhenAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that when a method returns [MaybeNullWhenAttribute.ReturnValue](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.maybenullwhenattribute.returnvalue), the parameter may be null even if the corresponding type disallows it.

```csharp
public sealed class MaybeNullWhenAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [MaybeNullWhenAttribute](./system.diagnostics.codeanalysis.maybenullwhenattribute.md)

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

### **MaybeNullWhenAttribute(Boolean)**

Initializes the attribute with the specified return value condition.

```csharp
public MaybeNullWhenAttribute(bool returnValue)
```

#### Parameters

`returnValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

            The return value condition. If the method returns this value, the associated parameter may be null.
