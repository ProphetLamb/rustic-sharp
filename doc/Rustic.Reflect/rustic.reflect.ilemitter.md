# ILEmitter

Namespace: Rustic.Reflect



```csharp
public class ILEmitter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ILEmitter](./rustic.reflect.ilemitter.md)

## Fields

### **Gen**



```csharp
public ILGenerator Gen;
```

## Constructors

### **ILEmitter()**



```csharp
public ILEmitter()
```

## Methods

### **ret()**



```csharp
public ILEmitter ret()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **cast(Type)**



```csharp
public ILEmitter cast(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **box(Type)**



```csharp
public ILEmitter box(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **unbox_any(Type)**



```csharp
public ILEmitter unbox_any(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **unbox(Type)**



```csharp
public ILEmitter unbox(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **call(MethodInfo)**



```csharp
public ILEmitter call(MethodInfo method)
```

#### Parameters

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **callvirt(MethodInfo)**



```csharp
public ILEmitter callvirt(MethodInfo method)
```

#### Parameters

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldnull()**



```csharp
public ILEmitter ldnull()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **bne_un(Label)**



```csharp
public ILEmitter bne_un(Label target)
```

#### Parameters

`target` [Label](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.label)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **beq(Label)**



```csharp
public ILEmitter beq(Label target)
```

#### Parameters

`target` [Label](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.label)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4_0()**



```csharp
public ILEmitter ldc_i4_0()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4_1()**



```csharp
public ILEmitter ldc_i4_1()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4_s(SByte)**



```csharp
public ILEmitter ldc_i4_s(sbyte c)
```

#### Parameters

`c` [SByte](https://docs.microsoft.com/en-us/dotnet/api/system.sbyte)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4_s(Byte)**



```csharp
public ILEmitter ldc_i4_s(byte c)
```

#### Parameters

`c` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4(Int32)**



```csharp
public ILEmitter ldc_i4(int c)
```

#### Parameters

`c` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i4(UInt32)**



```csharp
public ILEmitter ldc_i4(uint c)
```

#### Parameters

`c` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i8(Int64)**



```csharp
public ILEmitter ldc_i8(long c)
```

#### Parameters

`c` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_i8(UInt64)**



```csharp
public ILEmitter ldc_i8(ulong c)
```

#### Parameters

`c` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_r4(Single)**



```csharp
public ILEmitter ldc_r4(float c)
```

#### Parameters

`c` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldc_r8(Double)**



```csharp
public ILEmitter ldc_r8(double c)
```

#### Parameters

`c` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarg0()**



```csharp
public ILEmitter ldarg0()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarg1()**



```csharp
public ILEmitter ldarg1()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarg2()**



```csharp
public ILEmitter ldarg2()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarga(Int32)**



```csharp
public ILEmitter ldarga(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarga_s(Int32)**



```csharp
public ILEmitter ldarga_s(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarg(Int32)**



```csharp
public ILEmitter ldarg(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldarg_s(Int32)**



```csharp
public ILEmitter ldarg_s(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldstr(String)**



```csharp
public ILEmitter ldstr(string str)
```

#### Parameters

`str` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ifclass_ldind_ref(Type)**



```csharp
public ILEmitter ifclass_ldind_ref(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc0()**



```csharp
public ILEmitter ldloc0()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc1()**



```csharp
public ILEmitter ldloc1()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc2()**



```csharp
public ILEmitter ldloc2()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloca_s(Int32)**



```csharp
public ILEmitter ldloca_s(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloca_s(LocalBuilder)**



```csharp
public ILEmitter ldloca_s(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc_s(Int32)**



```csharp
public ILEmitter ldloc_s(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc_s(LocalBuilder)**



```csharp
public ILEmitter ldloc_s(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloca(Int32)**



```csharp
public ILEmitter ldloca(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloca(LocalBuilder)**



```csharp
public ILEmitter ldloca(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc(Int32)**



```csharp
public ILEmitter ldloc(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldloc(LocalBuilder)**



```csharp
public ILEmitter ldloc(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **initobj(Type)**



```csharp
public ILEmitter initobj(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **newobj(ConstructorInfo)**



```csharp
public ILEmitter newobj(ConstructorInfo ctor)
```

#### Parameters

`ctor` [ConstructorInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.constructorinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **Throw()**



```csharp
public ILEmitter Throw()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **throw_new(Type)**



```csharp
public ILEmitter throw_new(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stelem_ref()**



```csharp
public ILEmitter stelem_ref()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldelem_ref()**



```csharp
public ILEmitter ldelem_ref()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldlen()**



```csharp
public ILEmitter ldlen()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc(Int32)**



```csharp
public ILEmitter stloc(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc_s(Int32)**



```csharp
public ILEmitter stloc_s(int idx)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc(LocalBuilder)**



```csharp
public ILEmitter stloc(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc_s(LocalBuilder)**



```csharp
public ILEmitter stloc_s(LocalBuilder local)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc0()**



```csharp
public ILEmitter stloc0()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stloc1()**



```csharp
public ILEmitter stloc1()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **mark(Label)**



```csharp
public ILEmitter mark(Label label)
```

#### Parameters

`label` [Label](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.label)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldfld(FieldInfo)**



```csharp
public ILEmitter ldfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldsfld(FieldInfo)**



```csharp
public ILEmitter ldsfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **lodfld(FieldInfo)**



```csharp
public ILEmitter lodfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ifvaluetype_box(Type)**



```csharp
public ILEmitter ifvaluetype_box(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stfld(FieldInfo)**



```csharp
public ILEmitter stfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stsfld(FieldInfo)**



```csharp
public ILEmitter stsfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **setfld(FieldInfo)**



```csharp
public ILEmitter setfld(FieldInfo field)
```

#### Parameters

`field` [FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **unboxorcast(Type)**



```csharp
public ILEmitter unboxorcast(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **callorvirt(MethodInfo)**



```csharp
public ILEmitter callorvirt(MethodInfo method)
```

#### Parameters

`method` [MethodInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **stind_ref()**



```csharp
public ILEmitter stind_ref()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ldind_ref()**



```csharp
public ILEmitter ldind_ref()
```

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **declocal(Type)**



```csharp
public LocalBuilder declocal(Type type)
```

#### Parameters

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

### **deflabel()**



```csharp
public Label deflabel()
```

#### Returns

[Label](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.label)<br>

### **ifclass_ldarg_else_ldarga(Int32, Type)**



```csharp
public ILEmitter ifclass_ldarg_else_ldarga(int idx, Type type)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ifclass_ldloc_else_ldloca(Int32, Type)**



```csharp
public ILEmitter ifclass_ldloc_else_ldloca(int idx, Type type)
```

#### Parameters

`idx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **perform(IntPtr, MemberInfo)**



```csharp
public ILEmitter perform(IntPtr action, MemberInfo member)
```

#### Parameters

`action` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`member` [MemberInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>

### **ifbyref_ldloca_else_ldloc(LocalBuilder, Type)**



```csharp
public ILEmitter ifbyref_ldloca_else_ldloc(LocalBuilder local, Type type)
```

#### Parameters

`local` [LocalBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.localbuilder)<br>

`type` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[ILEmitter](./rustic.reflect.ilemitter.md)<br>
