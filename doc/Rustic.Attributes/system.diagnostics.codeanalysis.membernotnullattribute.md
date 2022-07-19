# MemberNotNullAttribute

Namespace: System.Diagnostics.CodeAnalysis

Specifies that the method or property will ensure that the listed field and property members have not-null values.

```csharp
public sealed class MemberNotNullAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) → [MemberNotNullAttribute](./system.diagnostics.codeanalysis.membernotnullattribute.md)

## Properties

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

### **MemberNotNullAttribute(String)**

Initializes the attribute with a field or property member.

```csharp
public MemberNotNullAttribute(string member)
```

#### Parameters

`member` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

            The field or property member that is promised to be not-null.

### **MemberNotNullAttribute(String[])**

Initializes the attribute with the list of field and property members.

```csharp
public MemberNotNullAttribute(String[] members)
```

#### Parameters

`members` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

            The list of field and property members that are promised to be not-null.
