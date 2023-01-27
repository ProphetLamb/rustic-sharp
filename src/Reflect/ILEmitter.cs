#pragma warning disable IDE1006, IDE0058, CA1707, CS1591
#nullable disable
// ReSharper disable InconsistentNaming

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Rustic.Reflect;

/// <summary>Fluent builder simplifying the composition of IL code at runtime.</summary>
[CLSCompliant(false)]
public sealed class ILEmitter
{
    public ILGenerator Gen = null!;

    public ILEmitter ret() { Gen.Emit(OpCodes.Ret); return this; }
    public ILEmitter cast(Type type) { Gen.Emit(OpCodes.Castclass, type); return this; }
    public ILEmitter box(Type type) { Gen.Emit(OpCodes.Box, type); return this; }
    public ILEmitter unbox_any(Type type) { Gen.Emit(OpCodes.Unbox_Any, type); return this; }
    public ILEmitter unbox(Type type) { Gen.Emit(OpCodes.Unbox, type); return this; }
    public ILEmitter call(MethodInfo method) { Gen.Emit(OpCodes.Call, method); return this; }
    public ILEmitter callvirt(MethodInfo method) { Gen.Emit(OpCodes.Callvirt, method); return this; }
    public ILEmitter ldnull() { Gen.Emit(OpCodes.Ldnull); return this; }
    public ILEmitter bne_un(Label target) { Gen.Emit(OpCodes.Bne_Un, target); return this; }
    public ILEmitter beq(Label target) { Gen.Emit(OpCodes.Beq, target); return this; }
    public ILEmitter ldc_i4_0() { Gen.Emit(OpCodes.Ldc_I4_0); return this; }
    public ILEmitter ldc_i4_1() { Gen.Emit(OpCodes.Ldc_I4_1); return this; }
    public ILEmitter ldc_i4_s(sbyte c) { Gen.Emit(OpCodes.Ldc_I4_S, c); return this; }
    public ILEmitter ldc_i4_s(byte c) { Gen.Emit(OpCodes.Ldc_I4_S, c); return this; }
    public ILEmitter ldc_i4(int c) { Gen.Emit(OpCodes.Ldc_I4, c); return this; }
    public ILEmitter ldc_i4(uint c) { Gen.Emit(OpCodes.Ldc_I4, c); return this; }
    public ILEmitter ldc_i8(long c) { Gen.Emit(OpCodes.Ldc_I8, c); return this; }
    public ILEmitter ldc_i8(ulong c) { Gen.Emit(OpCodes.Ldc_I8, c); return this; }
    public ILEmitter ldc_r4(float c) { Gen.Emit(OpCodes.Ldc_R4, c); return this; }
    public ILEmitter ldc_r8(double c) { Gen.Emit(OpCodes.Ldc_R8, c); return this; }
    public ILEmitter ldarg0() { Gen.Emit(OpCodes.Ldarg_0); return this; }
    public ILEmitter ldarg1() { Gen.Emit(OpCodes.Ldarg_1); return this; }
    public ILEmitter ldarg2() { Gen.Emit(OpCodes.Ldarg_2); return this; }
    public ILEmitter ldarga(int idx) { Gen.Emit(OpCodes.Ldarga, idx); return this; }
    public ILEmitter ldarga_s(int idx) { Gen.Emit(OpCodes.Ldarga_S, idx); return this; }
    public ILEmitter ldarg(int idx) { Gen.Emit(OpCodes.Ldarg, idx); return this; }
    public ILEmitter ldarg_s(int idx) { Gen.Emit(OpCodes.Ldarg_S, idx); return this; }
    public ILEmitter ldstr(string str) { Gen.Emit(OpCodes.Ldstr, str); return this; }
    public ILEmitter ifclass_ldind_ref(Type type) { if (!type.IsValueType) { Gen.Emit(OpCodes.Ldind_Ref); } return this; }
    public ILEmitter ldloc0() { Gen.Emit(OpCodes.Ldloc_0); return this; }
    public ILEmitter ldloc1() { Gen.Emit(OpCodes.Ldloc_1); return this; }
    public ILEmitter ldloc2() { Gen.Emit(OpCodes.Ldloc_2); return this; }
    public ILEmitter ldloca_s(int idx) { Gen.Emit(OpCodes.Ldloca_S, idx); return this; }
    public ILEmitter ldloca_s(LocalBuilder local) { Gen.Emit(OpCodes.Ldloca_S, local); return this; }
    public ILEmitter ldloc_s(int idx) { Gen.Emit(OpCodes.Ldloc_S, idx); return this; }
    public ILEmitter ldloc_s(LocalBuilder local) { Gen.Emit(OpCodes.Ldloc_S, local); return this; }
    public ILEmitter ldloca(int idx) { Gen.Emit(OpCodes.Ldloca, idx); return this; }
    public ILEmitter ldloca(LocalBuilder local) { Gen.Emit(OpCodes.Ldloca, local); return this; }
    public ILEmitter ldloc(int idx) { Gen.Emit(OpCodes.Ldloc, idx); return this; }
    public ILEmitter ldloc(LocalBuilder local) { Gen.Emit(OpCodes.Ldloc, local); return this; }
    public ILEmitter initobj(Type type) { Gen.Emit(OpCodes.Initobj, type); return this; }
    public ILEmitter newobj(ConstructorInfo ctor) { Gen.Emit(OpCodes.Newobj, ctor); return this; }
    public ILEmitter Throw() { Gen.Emit(OpCodes.Throw); return this; }
    public ILEmitter throw_new(Type type) { var exp = type.GetConstructor(Type.EmptyTypes); newobj(exp).Throw(); return this; }
    public ILEmitter stelem_ref() { Gen.Emit(OpCodes.Stelem_Ref); return this; }
    public ILEmitter ldelem_ref() { Gen.Emit(OpCodes.Ldelem_Ref); return this; }
    public ILEmitter ldlen() { Gen.Emit(OpCodes.Ldlen); return this; }
    public ILEmitter stloc(int idx) { Gen.Emit(OpCodes.Stloc, idx); return this; }
    public ILEmitter stloc_s(int idx) { Gen.Emit(OpCodes.Stloc_S, idx); return this; }
    public ILEmitter stloc(LocalBuilder local) { Gen.Emit(OpCodes.Stloc, local); return this; }
    public ILEmitter stloc_s(LocalBuilder local) { Gen.Emit(OpCodes.Stloc_S, local); return this; }
    public ILEmitter stloc0() { Gen.Emit(OpCodes.Stloc_0); return this; }
    public ILEmitter stloc1() { Gen.Emit(OpCodes.Stloc_1); return this; }
    public ILEmitter mark(Label label) { Gen.MarkLabel(label); return this; }
    public ILEmitter ldfld(FieldInfo field) { Gen.Emit(OpCodes.Ldfld, field); return this; }
    public ILEmitter ldsfld(FieldInfo field) { Gen.Emit(OpCodes.Ldsfld, field); return this; }
    public ILEmitter lodfld(FieldInfo field) { if (field.IsStatic) { ldsfld(field); } else { ldfld(field); } return this; }
    public ILEmitter ifvaluetype_box(Type type) { if (!type.IsValueType) { return this; } Gen.Emit(OpCodes.Box, type); return this; }
    public ILEmitter stfld(FieldInfo field) { Gen.Emit(OpCodes.Stfld, field); return this; }
    public ILEmitter stsfld(FieldInfo field) { Gen.Emit(OpCodes.Stsfld, field); return this; }
    public ILEmitter setfld(FieldInfo field) { if (field.IsStatic) { stsfld(field); } else { stfld(field); } return this; }
    public ILEmitter unboxorcast(Type type) { if (type.IsValueType) { unbox(type); } else { cast(type); } return this; }
    public ILEmitter callorvirt(MethodInfo method) { if (method.IsVirtual) { Gen.Emit(OpCodes.Callvirt, method); } else { Gen.Emit(OpCodes.Call, method); } return this; }
    public ILEmitter stind_ref() { Gen.Emit(OpCodes.Stind_Ref); return this; }
    public ILEmitter ldind_ref() { Gen.Emit(OpCodes.Ldind_Ref); return this; }
    public LocalBuilder declocal(Type type) { return Gen.DeclareLocal(type); }
    public Label deflabel() { return Gen.DefineLabel(); }
    public ILEmitter ifclass_ldarg_else_ldarga(int idx, Type type) { if (type.IsValueType) { ldarga(idx); } else { ldarg(idx); } return this; }
    public ILEmitter ifclass_ldloc_else_ldloca(int idx, Type type) { if (type.IsValueType) { ldloca(idx); } else { ldloc(idx); } return this; }
    public unsafe ILEmitter perform(delegate*<ILEmitter, MemberInfo, void> action, MemberInfo member) { action(this, member); return this; }
    public ILEmitter ifbyref_ldloca_else_ldloc(LocalBuilder local, Type type) { if (type.IsByRef) { ldloca(local); } else { ldloc(local); } return this; }
#nullable restore
#pragma warning restore IDE1006, IDE0058, CA1707
}
