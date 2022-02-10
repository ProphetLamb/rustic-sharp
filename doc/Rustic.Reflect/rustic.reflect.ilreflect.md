# ILReflect

Namespace: Rustic.Reflect

A dynamic reflection extensions library that emits Gen to set/get fields/properties, call methods and invoke constructors
 Once the delegate is created, it can be stored and reused resulting in much faster access times than using regular reflection
 The results are cached. Once a delegate is generated, any subsequent call to generate the same delegate on the same field/property/method will return the previously generated delegate
 Note: Since this generates IL, it won't work on AOT platforms such as iOS an Android. But is useful and works very well in editor codes and standalone targets

```csharp
public static class ILReflect
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ILReflect](./rustic.reflect.ilreflect.md)

## Fields

### **CtorInvokerName**



```csharp
public static string CtorInvokerName;
```

### **MethodCallerName**



```csharp
public static string MethodCallerName;
```

### **FieldSetterName**



```csharp
public static string FieldSetterName;
```

### **FieldGetterName**



```csharp
public static string FieldGetterName;
```

### **PropertySetterName**



```csharp
public static string PropertySetterName;
```

### **PropertyGetterName**



```csharp
public static string PropertyGetterName;
```

## Properties

### **Emit**



```csharp
public static ILEmitter Emit { get; }
```

#### Property Value

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

## Methods

### **DelegateForCtor&lt;T&gt;(Type, Type[])**

Generates or gets a strongly-typed open-instance delegate to the specified type constructor that takes the specified type params

```csharp
public static CtorInvoker<T> DelegateForCtor<T>(Type type, Type[] paramTypes)
```

#### Type Parameters

`T`<br>
The target type.

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`paramTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

CtorInvoker&lt;T&gt;<br>

### **DelegateForCtor(Type, Type[])**

Generates or gets a weakly-typed open-instance delegate to the specified type constructor that takes the specified type params

```csharp
public static CtorInvoker<object> DelegateForCtor(Type type, Type[] ctorParamTypes)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`ctorParamTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[CtorInvoker&lt;Object&gt;](./rustic.reflect.ctorinvoker-1.md)<br>

### **DelegateForGet&lt;T, R&gt;(PropertyInfo)**

Generates or gets a strongly-typed open-instance delegate to get the value of the specified property from a given target

```csharp
public static MemberGetter<T, R> DelegateForGet<T, R>(PropertyInfo property)
```

#### Type Parameters

`T`<br>
The target type.

`R`<br>
The return type.

#### Parameters

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

#### Returns

MemberGetter&lt;T, R&gt;<br>

### **DelegateForGet(PropertyInfo)**

Generates or gets a weakly-typed open-instance delegate to get the value of the specified property from a given target

```csharp
public static MemberGetter<object, object> DelegateForGet(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

#### Returns

[MemberGetter&lt;Object, Object&gt;](./rustic.reflect.membergetter-2.md)<br>

### **DelegateForSet&lt;T, V&gt;(PropertyInfo)**

Generates or gets a strongly-typed open-instance delegate to set the value of the specified property on a given target

```csharp
public static MemberSetter<T, V> DelegateForSet<T, V>(PropertyInfo property)
```

#### Type Parameters

`T`<br>
The target type.

`V`<br>
The type of the value.

#### Parameters

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

#### Returns

MemberSetter&lt;T, V&gt;<br>

### **DelegateForSet(PropertyInfo)**

Generates or gets a weakly-typed open-instance delegate to set the value of the specified property on a given target

```csharp
public static MemberSetter<object, object> DelegateForSet(PropertyInfo property)
```

#### Parameters

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

#### Returns

[MemberSetter&lt;Object, Object&gt;](./rustic.reflect.membersetter-2.md)<br>

### **DelegateForGet&lt;T, R&gt;(FieldInfo)**

Generates an open-instance delegate to get the value of the property from a given target

```csharp
public static MemberGetter<T, R> DelegateForGet<T, R>(FieldInfo field)
```

#### Type Parameters

`T`<br>
The target type.

`R`<br>
The return type.

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

MemberGetter&lt;T, R&gt;<br>

### **DelegateForGet(FieldInfo)**

Generates a weakly-typed open-instance delegate to set the value of the field in a given target

```csharp
public static MemberGetter<object, object> DelegateForGet(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[MemberGetter&lt;Object, Object&gt;](./rustic.reflect.membergetter-2.md)<br>

### **DelegateForSet&lt;T, V&gt;(FieldInfo)**

Generates a strongly-typed open-instance delegate to set the value of the field in a given target

```csharp
public static MemberSetter<T, V> DelegateForSet<T, V>(FieldInfo field)
```

#### Type Parameters

`T`<br>
The target type.

`V`<br>
The type of the value.

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

MemberSetter&lt;T, V&gt;<br>

### **DelegateForSet(FieldInfo)**

Generates a weakly-typed open-instance delegate to set the value of the field in a given target

```csharp
public static MemberSetter<object, object> DelegateForSet(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[MemberSetter&lt;Object, Object&gt;](./rustic.reflect.membersetter-2.md)<br>

### **DelegateForCall&lt;T, R&gt;(MethodInfo)**

Generates a strongly-typed open-instance delegate to invoke the specified method

```csharp
public static MethodCaller<T, R> DelegateForCall<T, R>(MethodInfo method)
```

#### Type Parameters

`T`<br>
The target type.

`R`<br>
The return type.

#### Parameters

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

#### Returns

MethodCaller&lt;T, R&gt;<br>

### **DelegateForCall(MethodInfo)**

Generates a weakly-typed open-instance delegate to invoke the specified method

```csharp
public static MethodCaller<object, object> DelegateForCall(MethodInfo method)
```

#### Parameters

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

#### Returns

[MethodCaller&lt;Object, Object&gt;](./rustic.reflect.methodcaller-2.md)<br>

### **SafeInvoke&lt;T, V&gt;(MethodCaller&lt;T, V&gt;, T, Object[])**

Executes the delegate on the specified target and arguments but only if it's not null

```csharp
public static void SafeInvoke<T, V>(MethodCaller<T, V> caller, T target, Object[] args)
```

#### Type Parameters

`T`<br>
The target type.

`V`<br>
The type of the value.

#### Parameters

`caller` MethodCaller&lt;T, V&gt;<br>

`target` T<br>

`args` [Object[]](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **SafeInvoke&lt;T, V&gt;(MemberSetter&lt;T, V&gt;, T&, V)**

Executes the delegate on the specified target and value but only if it's not null

```csharp
public static void SafeInvoke<T, V>(MemberSetter<T, V> setter, T& target, V value)
```

#### Type Parameters

`T`<br>
The target type.

`V`<br>
The type of the value.

#### Parameters

`setter` MemberSetter&lt;T, V&gt;<br>

`target` T&<br>

`value` V<br>

### **SafeInvoke&lt;T, R&gt;(MemberGetter&lt;T, R&gt;, T)**

Executes the delegate on the specified target only if it's not null, returns default(TReturn) otherwise

```csharp
public static R SafeInvoke<T, R>(MemberGetter<T, R> getter, T target)
```

#### Type Parameters

`T`<br>
The target type.

`R`<br>
The return type.

#### Parameters

`getter` MemberGetter&lt;T, R&gt;<br>

`target` T<br>

#### Returns

R<br>

### **GenDebugAssembly(String, FieldInfo, PropertyInfo, MethodInfo, Type, Type[])**

Generates a assembly called 'name' that's useful for debugging purposes and inspecting the resulting C# code in ILSpy
 If 'field' is not null, it generates a setter and getter for that field
 If 'property' is not null, it generates a setter and getter for that property
 If 'method' is not null, it generates a call for that method
 if 'targetType' and 'ctorParamTypes' are not null, it generates a constructor for the target type that takes the specified arguments

```csharp
public static void GenDebugAssembly(string name, FieldInfo field, PropertyInfo property, MethodInfo method, Type targetType, Type[] ctorParamTypes)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

`targetType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`ctorParamTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **GenDebugAssembly&lt;T&gt;(String, FieldInfo, PropertyInfo, MethodInfo, Type, Type[])**

Generates a assembly called 'name' that's useful for debugging purposes and inspecting the resulting C# code in ILSpy
 If 'field' is not null, it generates a setter and getter for that field
 If 'property' is not null, it generates a setter and getter for that property
 If 'method' is not null, it generates a call for that method
 if 'targetType' and 'ctorParamTypes' are not null, it generates a constructor for the target type that takes the specified arguments

```csharp
public static ValueTuple<T, Type> GenDebugAssembly<T>(string name, FieldInfo field, PropertyInfo property, MethodInfo method, Type targetType, Type[] ctorParamTypes)
```

#### Type Parameters

`T`<br>
The target type.

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

`property` [PropertyInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br>

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

`targetType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`ctorParamTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

ValueTuple&lt;T, Type&gt;<br>

### **GenDelegateForMember&lt;D, M&gt;(M, Key&, String, IntPtr, Type, Type[])**



```csharp
public static D GenDelegateForMember<D, M>(M member, Key& key, string dynMethodName, IntPtr generator, Type returnType, Type[] paramTypes)
```

#### Type Parameters

`D`<br>

`M`<br>

#### Parameters

`member` M<br>

`key` [Key&](./rustic.reflect.ilreflect.key&.md)<br>

`dynMethodName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`generator` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`returnType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`paramTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

D<br>

### **GetKey&lt;T, R&gt;(String, MemberInfo)**



```csharp
internal static Key GetKey<T, R>(string callerName, MemberInfo member)
```

#### Type Parameters

`T`<br>

`R`<br>

#### Parameters

`callerName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`member` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

#### Returns

[Key](./rustic.reflect.ilreflect.key.md)<br>

### **GetKey(String, Type, Type, Type[])**



```csharp
internal static Key GetKey(string callerName, Type target, Type ret, Type[] paramTypes)
```

#### Parameters

`callerName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`target` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`ret` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`paramTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Key](./rustic.reflect.ilreflect.key.md)<br>

### **&lt;GenDebugAssembly&gt;g__Build|28_0&lt;T&gt;(String, Type, Type[], &lt;&gt;c__DisplayClass28_0`1&)**



```csharp
internal static ILGenerator <GenDebugAssembly>g__Build|28_0<T>(string methodName, Type returnType, Type[] parameterTypes, <>c__DisplayClass28_0`1& )
```

#### Type Parameters

`T`<br>

#### Parameters

`methodName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`returnType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`parameterTypes` [Type[]](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`` &lt;&gt;c__DisplayClass28_0`1&<br>

#### Returns

[ILGenerator](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator)<br>

### **&lt;GenFieldGetter&gt;g__B|32_0&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__B|32_0<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__I1|32_1&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__I1|32_1<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__U1|32_2&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__U1|32_2<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__I4|32_3&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__I4|32_3<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__U4|32_4&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__U4|32_4<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__I8|32_5&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__I8|32_5<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__U8|32_6&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__U8|32_6<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__F4|32_7&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__F4|32_7<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__F8|32_8&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__F8|32_8<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__Str|32_9&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__Str|32_9<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldGetter&gt;g__Fld|32_10&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldGetter>g__Fld|32_10<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenPropertyGetter&gt;g__EmitGetter|33_0&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenPropertyGetter>g__EmitGetter|33_0<T>(ILEmitter e, MemberInfo p)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`p` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenFieldSetter&gt;g__EmitSetter|35_0&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenFieldSetter>g__EmitSetter|35_0<T>(ILEmitter e, MemberInfo f)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`f` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

### **&lt;GenPropertySetter&gt;g__EmitSetter|36_0&lt;T&gt;(ILEmitter, MemberInfo)**



```csharp
internal static void <GenPropertySetter>g__EmitSetter|36_0<T>(ILEmitter e, MemberInfo p)
```

#### Type Parameters

`T`<br>

#### Parameters

`e` [ILEmitter](./rustic.reflect.ilemitter.md)<br>

`p` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>
