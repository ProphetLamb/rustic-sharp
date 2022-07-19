# NotNullIfNotNullAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that the output will be non-null if the named parameter is non-null.

```csharp
public sealed class NotNullIfNotNullAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [NotNullIfNotNullAttribute](./system.diagnostics.codeanalysis.notnullifnotnullattribute.md)

## Properties

### **ParameterName**

Gets the associated parameter name.

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

### **NotNullIfNotNullAttribute(String)**

Initializes the attribute with the associated parameter name.

```csharp
public NotNullIfNotNullAttribute(string parameterName)
```

#### Parameters

`parameterName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

            The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
