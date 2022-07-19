# HashCode

Namespace: System

```csharp
public struct HashCode
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [HashCode](./system.hashcode.md)

## Methods

### **Combine&lt;T1&gt;(T1)**

```csharp
int Combine<T1>(T1 value1)
```

#### Type Parameters

`T1`<br>

#### Parameters

`value1` T1<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2&gt;(T1, T2)**

```csharp
int Combine<T1, T2>(T1 value1, T2 value2)
```

#### Type Parameters

`T1`<br>

`T2`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3&gt;(T1, T2, T3)**

```csharp
int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3, T4&gt;(T1, T2, T3, T4)**

```csharp
int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

`value4` T4<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3, T4, T5&gt;(T1, T2, T3, T4, T5)**

```csharp
int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

`value4` T4<br>

`value5` T5<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3, T4, T5, T6&gt;(T1, T2, T3, T4, T5, T6)**

```csharp
int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

`T6`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

`value4` T4<br>

`value5` T5<br>

`value6` T6<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3, T4, T5, T6, T7&gt;(T1, T2, T3, T4, T5, T6, T7)**

```csharp
int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

`T6`<br>

`T7`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

`value4` T4<br>

`value5` T5<br>

`value6` T6<br>

`value7` T7<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Combine&lt;T1, T2, T3, T4, T5, T6, T7, T8&gt;(T1, T2, T3, T4, T5, T6, T7, T8)**

```csharp
int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

`T6`<br>

`T7`<br>

`T8`<br>

#### Parameters

`value1` T1<br>

`value2` T2<br>

`value3` T3<br>

`value4` T4<br>

`value5` T5<br>

`value6` T6<br>

`value7` T7<br>

`value8` T8<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Add&lt;T&gt;(T)**

```csharp
void Add<T>(T value)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

### **Add&lt;T&gt;(T, IEqualityComparer&lt;T&gt;)**

```csharp
void Add<T>(T value, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`comparer` IEqualityComparer&lt;T&gt;<br>

### **AddBytes(ReadOnlySpan&lt;Byte&gt;)**

Adds a span of bytes to the hash code.

```csharp
void AddBytes(ReadOnlySpan<byte> value)
```

#### Parameters

`value` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The span.

**Remarks:**

This method does not guarantee that the result of adding a span of bytes will match
 the result of adding the same bytes individually.

### **ToHashCode()**

```csharp
int ToHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
