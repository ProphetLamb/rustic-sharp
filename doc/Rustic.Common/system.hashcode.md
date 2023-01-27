# HashCode

Namespace: System

Combines the hash code for multiple values into a single hash code.

```csharp
public struct HashCode
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [HashCode](./system.hashcode.md)

## Methods

### **Combine&lt;T1&gt;(T1)**

Diffuses the hash code returned by the specified value.

```csharp
int Combine<T1>(T1 value1)
```

#### Type Parameters

`T1`<br>
The type of the value to add the hash code.

#### Parameters

`value1` T1<br>
The value to add to the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the single value.

### **Combine&lt;T1, T2&gt;(T1, T2)**

Combines two values into a hash code.

```csharp
int Combine<T1, T2>(T1 value1, T2 value2)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the two values.

### **Combine&lt;T1, T2, T3&gt;(T1, T2, T3)**

Combines three values into a hash code.

```csharp
int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the three values.

### **Combine&lt;T1, T2, T3, T4&gt;(T1, T2, T3, T4)**

Combines four values into a hash code.

```csharp
int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

`T4`<br>
The type of the fourth value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

`value4` T4<br>
The fourth value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the four values.

### **Combine&lt;T1, T2, T3, T4, T5&gt;(T1, T2, T3, T4, T5)**

Combines five values into a hash code.

```csharp
int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

`T4`<br>
The type of the fourth value to combine into the hash code.

`T5`<br>
The type of the fifth value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

`value4` T4<br>
The fourth value to combine into the hash code.

`value5` T5<br>
The fifth value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the five values.

### **Combine&lt;T1, T2, T3, T4, T5, T6&gt;(T1, T2, T3, T4, T5, T6)**

Combines six values into a hash code.

```csharp
int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

`T4`<br>
The type of the fourth value to combine into the hash code.

`T5`<br>
The type of the fifth value to combine into the hash code.

`T6`<br>
The type of the sixth value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

`value4` T4<br>
The fourth value to combine into the hash code.

`value5` T5<br>
The fifth value to combine into the hash code.

`value6` T6<br>
The sixth value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the six values.

### **Combine&lt;T1, T2, T3, T4, T5, T6, T7&gt;(T1, T2, T3, T4, T5, T6, T7)**

Combines seven values into a hash code.

```csharp
int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

`T4`<br>
The type of the fourth value to combine into the hash code.

`T5`<br>
The type of the fifth value to combine into the hash code.

`T6`<br>
The type of the sixth value to combine into the hash code.

`T7`<br>
The type of the seventh value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

`value4` T4<br>
The fourth value to combine into the hash code.

`value5` T5<br>
The fifth value to combine into the hash code.

`value6` T6<br>
The sixth value to combine into the hash code.

`value7` T7<br>
The seventh value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the seven values.

### **Combine&lt;T1, T2, T3, T4, T5, T6, T7, T8&gt;(T1, T2, T3, T4, T5, T6, T7, T8)**

Combines eight values into a hash code.

```csharp
int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
```

#### Type Parameters

`T1`<br>
The type of the first value to combine into the hash code.

`T2`<br>
The type of the second value to combine into the hash code.

`T3`<br>
The type of the third value to combine into the hash code.

`T4`<br>
The type of the fourth value to combine into the hash code.

`T5`<br>
The type of the fifth value to combine into the hash code.

`T6`<br>
The type of the sixth value to combine into the hash code.

`T7`<br>
The type of the seventh value to combine into the hash code.

`T8`<br>
The type of the eighth value to combine into the hash code.

#### Parameters

`value1` T1<br>
The first value to combine into the hash code.

`value2` T2<br>
The second value to combine into the hash code.

`value3` T3<br>
The third value to combine into the hash code.

`value4` T4<br>
The fourth value to combine into the hash code.

`value5` T5<br>
The fifth value to combine into the hash code.

`value6` T6<br>
The sixth value to combine into the hash code.

`value7` T7<br>
The seventh value to combine into the hash code.

`value8` T8<br>
The eighth value to combine into the hash code.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The hash code that represents the eight values.

### **Add&lt;T&gt;(T)**

Adds a single value to the hash code.

```csharp
void Add<T>(T value)
```

#### Type Parameters

`T`<br>
The type of the value to add to the hash code.

#### Parameters

`value` T<br>
The value to add to the hash code.

### **Add&lt;T&gt;(T, IEqualityComparer&lt;T&gt;)**

Adds a single value to the hash code, specifying the type that provides the hash code function.

```csharp
void Add<T>(T value, IEqualityComparer<T> comparer)
```

#### Type Parameters

`T`<br>
The type of the value to add to the hash code.

#### Parameters

`value` T<br>
The value to add to the hash code.

`comparer` IEqualityComparer&lt;T&gt;<br>
The  to use to calculate the hash code.
            This value can be a null reference (Nothing in Visual Basic), which will use the default equality comparer for .

### **AddBytes(ReadOnlySpan&lt;Byte&gt;)**

Adds a span of bytes to the hash code.

```csharp
void AddBytes(ReadOnlySpan<byte> value)
```

#### Parameters

`value` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The span to add.

### **ToHashCode()**

Calculates the final hash code after consecutive [HashCode.Add&lt;T&gt;(T)](https://docs.microsoft.com/en-us/dotnet/api/system.hashcode.add) invocations.

```csharp
int ToHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The calculated hash code.

### **GetHashCode()**

This method is not supported and should not be called.

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
This method will always throw a .

#### Exceptions

[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception)<br>
Always thrown when this method is called.

### **Equals(Object)**

This method is not supported and should not be called.

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
Ignored.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
This method will always throw a .

#### Exceptions

[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception)<br>
Always thrown when this method is called.
