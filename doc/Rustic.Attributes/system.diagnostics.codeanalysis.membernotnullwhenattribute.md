# MemberNotNullWhenAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that the method or property will ensure that the listed field and property members have not-null values when returning with the specified return value condition.

```csharp
public sealed class MemberNotNullWhenAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [MemberNotNullWhenAttribute](./system.diagnostics.codeanalysis.membernotnullwhenattribute.md)

## Properties

### **ReturnValue**

Gets the return value condition.

```csharp
public bool ReturnValue { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Members**

Gets field or property member names.

```csharp
public String[] Members { get; }
```

#### Property Value

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

## Constructors

### **MemberNotNullWhenAttribute(Boolean, String)**

Initializes the attribute with the specified return value condition and a field or property member.

```csharp
public MemberNotNullWhenAttribute(bool returnValue, string member)
```

#### Parameters

`returnValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

            The return value condition. If the method returns this value, the associated parameter will not be null.

`member` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

            The field or property member that is promised to be not-null.

### **MemberNotNullWhenAttribute(Boolean, String[])**

Initializes the attribute with the specified return value condition and list of field and property members.

```csharp
public MemberNotNullWhenAttribute(bool returnValue, String[] members)
```

#### Parameters

`returnValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

            The return value condition. If the method returns this value, the associated parameter will not be null.

`members` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

            The list of field and property members that are promised to be not-null.
