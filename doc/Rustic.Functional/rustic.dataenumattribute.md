# DataEnumAttribute

Namespace: Rustic

Allows a enum member to ship with additional data.

```csharp
public sealed class DataEnumAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [DataEnumAttribute](./rustic.dataenumattribute.md)

## Properties

### **Data**

```csharp
public Type Data { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

## Constructors

### **DataEnumAttribute(Type)**

```csharp
public DataEnumAttribute(Type data)
```

#### Parameters

`data` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>
