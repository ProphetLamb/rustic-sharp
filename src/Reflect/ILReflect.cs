using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using Rustic.Memory;

namespace Rustic.Reflect;

public delegate void MemberSetter<T, in V>(ref T target, V value);

public delegate R MemberGetter<in T, out R>(T target);

public delegate R MethodCaller<in T, out R>(T target, object[] args);

public delegate T CtorInvoker<out T>(object[] parameters);

/// <summary>
/// A dynamic reflection extensions library that emits Gen to set/get fields/properties, call methods and invoke constructors
/// Once the delegate is created, it can be stored and reused resulting in much faster access times than using regular reflection
/// The results are cached. Once a delegate is generated, any subsequent call to generate the same delegate on the same field/property/method will return the previously generated delegate
/// Note: Since this generates IL, it won't work on AOT platforms such as iOS an Android. But is useful and works very well in editor codes and standalone targets
/// </summary>
public static class ILReflect
{
    [ThreadStatic] private static ILEmitter? EmitInst;
    [CLSCompliant(false)]
    public static ILEmitter Emit => EmitInst ??= new ILEmitter();

    private static readonly Lazy<ConcurrentDictionary<Key, Delegate>> CacheInst = new(() => new ConcurrentDictionary<Key, Delegate>());
    internal static ConcurrentDictionary<Key, Delegate> Cache => CacheInst.Value;

    public const string CtorInvokerName = "CI<>";
    public const string MethodCallerName = "MC<>";
    public const string FieldSetterName = "FS<>";
    public const string FieldGetterName = "FG<>";
    public const string PropertySetterName = "PS<>";
    public const string PropertyGetterName = "PG<>";

    /// <summary>
    /// Generates or gets a strongly-typed open-instance delegate to the specified type constructor that takes the specified type params
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    public static CtorInvoker<T> DelegateForCtor<T>(this Type type, params Type[] paramTypes)
    {
        Key key = GetKey(CtorInvokerName, type, type, paramTypes);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (CtorInvoker<T>)result;
        }

        DynamicMethod dynMethod;
        using (RentArray<Type> paramT = new(1) { typeof(object[]) })
        {
            dynMethod = new DynamicMethod(CtorInvokerName, typeof(T), paramT);
        }

        Emit.Gen = dynMethod.GetILGenerator();
        GenCtor<T>(type, paramTypes);

        result = dynMethod.CreateDelegate(typeof(CtorInvoker<T>));
        Cache[key] = result;
        return (CtorInvoker<T>)result;
    }

    /// <summary>
    /// Generates or gets a weakly-typed open-instance delegate to the specified type constructor that takes the specified type params
    /// </summary>
    public static CtorInvoker<object> DelegateForCtor(this Type type, params Type[] ctorParamTypes)
    {
        return DelegateForCtor<object>(type, ctorParamTypes);
    }

    /// <summary>
    /// Generates or gets a strongly-typed open-instance delegate to get the value of the specified property from a given target
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    public static MemberGetter<T, R> DelegateForGet<T, R>(this PropertyInfo property)
    {
        if (!property.CanRead)
        {
            ThrowHelper.ThrowInvalidOperationException($"Property {property.Name} is not readable.");
        }

        Key key = GetKey<T, R>(PropertyGetterName, property);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (MemberGetter<T, R>)result;
        }

        unsafe
        {
            using RentArray<Type> paramTypes = new(1) { typeof(T) };
            return GenDelegateForMember<MemberGetter<T, R>, PropertyInfo>(property, key, PropertyGetterName, &GenPropertyGetter<T>, typeof(R), paramTypes);
        }
    }

    /// <summary>
    /// Generates or gets a weakly-typed open-instance delegate to get the value of the specified property from a given target
    /// </summary>
    public static MemberGetter<object, object> DelegateForGet(this PropertyInfo property)
    {
        return DelegateForGet<object, object>(property);
    }

    /// <summary>
    /// Generates or gets a strongly-typed open-instance delegate to set the value of the specified property on a given target
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public static MemberSetter<T, V> DelegateForSet<T, V>(this PropertyInfo property)
    {
        if (!property.CanWrite)
        {
            ThrowHelper.ThrowInvalidOperationException($"Property {property.Name} is not writable.");
        }

        Key key = GetKey<T, V>(PropertySetterName, property);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (MemberSetter<T, V>)result;
        }

        unsafe
        {
            using RentArray<Type> paramTypes = new(2) { typeof(T).MakeByRefType(), typeof(V) };
            return GenDelegateForMember<MemberSetter<T, V>, PropertyInfo>(property, key, PropertySetterName, &GenPropertySetter<T>, typeof(void), paramTypes);
        }
    }

    /// <summary>
    /// Generates or gets a weakly-typed open-instance delegate to set the value of the specified property on a given target
    /// </summary>
    public static MemberSetter<object, object> DelegateForSet(this PropertyInfo property)
    {
        return DelegateForSet<object, object>(property);
    }

    /// <summary>
    /// Generates an open-instance delegate to get the value of the property from a given target
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    public static MemberGetter<T, R> DelegateForGet<T, R>(this FieldInfo field)
    {
        Key key = GetKey<T, R>(FieldGetterName, field);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (MemberGetter<T, R>)result;
        }

        unsafe
        {
            using RentArray<Type> paramTypes = new(1) { typeof(T) };
            return GenDelegateForMember<MemberGetter<T, R>, FieldInfo>(field, key, FieldGetterName, &GenFieldGetter<T>, typeof(R), paramTypes);
        }
    }

    /// <summary>
    /// Generates a weakly-typed open-instance delegate to set the value of the field in a given target
    /// </summary>
    public static MemberGetter<object, object> DelegateForGet(this FieldInfo field)
    {
        return DelegateForGet<object, object>(field);
    }

    /// <summary>
    /// Generates a strongly-typed open-instance delegate to set the value of the field in a given target
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public static MemberSetter<T, V> DelegateForSet<T, V>(this FieldInfo field)
    {
        Key key = GetKey<T, V>(FieldSetterName, field);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (MemberSetter<T, V>)result;
        }

        unsafe
        {
            using RentArray<Type> paramTypes = new(2) { typeof(T).MakeByRefType(), typeof(V) };
            return GenDelegateForMember<MemberSetter<T, V>, FieldInfo>(field, key, FieldSetterName, &GenFieldSetter<T>, typeof(void), paramTypes);
        }
    }

    /// <summary>
    /// Generates a weakly-typed open-instance delegate to set the value of the field in a given target
    /// </summary>
    public static MemberSetter<object, object> DelegateForSet(this FieldInfo field)
    {
        return DelegateForSet<object, object>(field);
    }

    /// <summary>
    /// Generates a strongly-typed open-instance delegate to invoke the specified method
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    public static MethodCaller<T, R> DelegateForCall<T, R>(this MethodInfo method)
    {
        Key key = GetKey<T, R>(MethodCallerName, method);
        if (Cache.TryGetValue(key, out Delegate? result))
        {
            return (MethodCaller<T, R>)result;
        }

        unsafe
        {
            using RentArray<Type> paramTypes = new(2) { typeof(T), typeof(object[]) };
            return GenDelegateForMember<MethodCaller<T, R>, MethodInfo>(method, key, MethodCallerName, &GenMethodInvocation<T>, typeof(R), paramTypes);
        }
    }

    /// <summary>
    /// Generates a weakly-typed open-instance delegate to invoke the specified method
    /// </summary>
    public static MethodCaller<object, object> DelegateForCall(this MethodInfo method)
    {
        return DelegateForCall<object, object>(method);
    }

    /// <summary>
    /// Executes the delegate on the specified target and arguments but only if it's not null
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public static void SafeInvoke<T, V>(this MethodCaller<T, V>? caller, T target, params object[] args)
    {
        caller?.Invoke(target, args);
    }

    /// <summary>
    /// Executes the delegate on the specified target and value but only if it's not null
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public static void SafeInvoke<T, V>(this MemberSetter<T, V>? setter, ref T target, V value)
    {
        setter?.Invoke(ref target, value);
    }

    /// <summary>
    /// Executes the delegate on the specified target only if it's not null, returns default(TReturn) otherwise
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    public static R SafeInvoke<T, R>(this MemberGetter<T, R>? getter, T target)
    {
        if (getter is not null)
        {
            return getter(target);
        }
        return default!;
    }

    /// <summary>
    /// Generates a assembly called 'name' that's useful for debugging purposes and inspecting the resulting C# code in ILSpy
    /// If 'field' is not null, it generates a setter and getter for that field
    /// If 'property' is not null, it generates a setter and getter for that property
    /// If 'method' is not null, it generates a call for that method
    /// if 'targetType' and 'ctorParamTypes' are not null, it generates a constructor for the target type that takes the specified arguments
    /// </summary>
    public static void GenDebugAssembly(string name, FieldInfo field, PropertyInfo property, MethodInfo method, Type targetType, Type[] ctorParamTypes)
    {
        GenDebugAssembly<object>(name, field, property, method, targetType, ctorParamTypes);
    }

    /// <summary>
    /// Generates a assembly called 'name' that's useful for debugging purposes and inspecting the resulting C# code in ILSpy
    /// If 'field' is not null, it generates a setter and getter for that field
    /// If 'property' is not null, it generates a setter and getter for that property
    /// If 'method' is not null, it generates a call for that method
    /// if 'targetType' and 'ctorParamTypes' are not null, it generates a constructor for the target type that takes the specified arguments
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    public static (T, Type) GenDebugAssembly<T>(string name, FieldInfo? field, PropertyInfo? property, MethodInfo? method, Type? targetType, Type[] ctorParamTypes)
    {
        AssemblyName? asmName = new AssemblyName("Asm");
        AssemblyBuilder? asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder? modBuilder = asmBuilder.DefineDynamicModule(name);
        TypeBuilder? typeBuilder = modBuilder.DefineType("Test", TypeAttributes.Public);

        bool weakTyping = typeof(T) == typeof(object);

        ILGenerator Build(string methodName, Type returnType, Type[] parameterTypes)
        {
            MethodBuilder? methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes);
            return methodBuilder.GetILGenerator();
        }

        if (field != null)
        {
            Type? fieldType = weakTyping ? typeof(object) : field.FieldType;
            using (RentArray<Type> paramTypes = new(2) { typeof(T).MakeByRefType(), fieldType })
            {
                Emit.Gen = Build("FieldSetter", typeof(void), paramTypes);
                GenFieldSetter<T>(field);
            }
            using (RentArray<Type> paramTypes = new(1) { typeof(T) })
            {
                Emit.Gen = Build("FieldGetter", fieldType, paramTypes);
                GenFieldGetter<T>(field);
            }
        }

        if (property != null)
        {
            Type? propType = weakTyping ? typeof(object) : property.PropertyType;
            using (RentArray<Type> paramTypes = new(2) { typeof(T).MakeByRefType(), propType })
            {
                Emit.Gen = Build("PropertySetter", typeof(void), paramTypes);
                GenPropertySetter<T>(property);
            }
            using (RentArray<Type> paramTypes = new(1) { typeof(T) })
            {
                Emit.Gen = Build("PropertyGetter", propType, paramTypes);
                GenPropertyGetter<T>(property);
            }
        }

        if (method != null)
        {
            Type? returnType = weakTyping || method.ReturnType == typeof(void) ? typeof(object) : method.ReturnType;
            using RentArray<Type> paramTypes = new(2) { typeof(T), typeof(object[]) };
            Emit.Gen = Build("MethodCaller", returnType, paramTypes);
            GenMethodInvocation<T>(method);
        }

        if (targetType != null)
        {
            using RentArray<Type> paramTypes = new(1) { typeof(object[]) };
            Emit.Gen = Build("Ctor", typeof(T), paramTypes);
            GenCtor<T>(targetType, ctorParamTypes);
        }

        Type? type = typeBuilder.CreateType()!;
        T? target = (T)asmBuilder.CreateInstance(name)!;
        return (target, type);
    }

    /// <summary>Generates the delegate for the member.</summary>
    /// <param name="member">The member.</param>
    /// <param name="key">The key of the delegate.</param>
    /// <param name="dynMethodName">The name of the delegate.</param>
    /// <param name="generator">The member generator.</param>
    /// <param name="returnType">The return type of the delegate.</param>
    /// <param name="paramTypes">The type parameters of the delegate.</param>
    /// <typeparam name="D">The type of the delegate.</typeparam>
    /// <typeparam name="M">The type of the MemberInfo</typeparam>
    /// <returns>The compiled delegate.</returns>
    [CLSCompliant(false)]
    public static unsafe D GenDelegateForMember<D, M>(M member, in Key key, string dynMethodName, delegate*<M, void> generator, Type returnType, Type[] paramTypes)
        where D : class
        where M : MemberInfo
    {
        DynamicMethod? dynMethod = new DynamicMethod(dynMethodName, returnType, paramTypes, true);

        Emit.Gen = dynMethod.GetILGenerator();
        generator(member);

        Delegate? result = dynMethod.CreateDelegate(typeof(D));
        Cache[key] = result;
        return (D)(object)result;
    }

    private static void GenCtor<T>(Type type, Type[] paramTypes)
    {
        // arg0: object[] arguments
        // goal: return new T(arguments)
        Type? targetType = typeof(T) == typeof(object) ? type : typeof(T);

        if (targetType.IsValueType && paramTypes.Length == 0)
        {
            LocalBuilder? tmp = Emit.declocal(targetType);
            Emit.ldloca(tmp)
                .initobj(targetType)
                .ldloc(0);
        }
        else
        {
            ConstructorInfo? ctor = targetType.GetConstructor(paramTypes);
            if (ctor is null)
            {
                ThrowInvalidCtorParams(targetType, paramTypes);
            }

            // push parameters in order to then call ctor
            for (int i = 0, imax = paramTypes.Length; i < imax; i++)
            {
                Emit.ldarg0()					// push args array
                    .ldc_i4(i)					// push index
                    .ldelem_ref()				// push array[index]
                    .unbox_any(paramTypes[i]);	// cast
            }

            Emit.newobj(ctor);
        }

        if (typeof(T) == typeof(object) && targetType.IsValueType)
        {
            Emit.box(targetType);
        }

        Emit.ret();
    }

    private static void GenMethodInvocation<T>(MethodInfo method)
    {
        bool weaklyTyped = typeof(T) == typeof(object);

        // push target if not static (instance-method. in that case first arg is always 'this')
        if (!method.IsStatic)
        {
            Type? targetType = weaklyTyped ? method.DeclaringType : typeof(T);
            Debug.Assert(targetType is not null);

            Emit.declocal(targetType);
            Emit.ldarg0();
            if (weaklyTyped)
            {
                Emit.unbox_any(targetType);
            }

            Emit.stloc0()
                .ifclass_ldloc_else_ldloca(0, targetType);
        }

        // push arguments in order to call method
        var prams = method.GetParameters();
        for (int i = 0, imax = prams.Length; i < imax; i++)
        {
            Emit.ldarg1()		// push array
                .ldc_i4(i)		// push index
                .ldelem_ref();	// pop array, index and push array[index]

            ParameterInfo? param = prams[i];
            Type? dataType = param.ParameterType;

            if (dataType.IsByRef)
            {
                dataType = dataType.GetElementType();
            }

            Debug.Assert(dataType is not null);

            LocalBuilder? tmp = Emit.declocal(dataType);
            Emit.unbox_any(dataType)
                .stloc(tmp)
                .ifbyref_ldloca_else_ldloc(tmp, param.ParameterType);
        }

        // perform the correct call (pushes the result)
        Emit.callorvirt(method);

        // if method wasn't static that means we declared a temp local to load the target
        // that means our local variables index for the arguments start from 1
        int localVarStart = method.IsStatic ? 0 : 1;
        for (var i = 0; i < prams.Length; i++)
        {
            Type? paramType = prams[i].ParameterType;
            if (paramType.IsByRef)
            {
                Type? byRefType = paramType.GetElementType();
                Debug.Assert(byRefType is not null);

                Emit.ldarg1()
                    .ldc_i4(i)
                    .ldloc(i + localVarStart);
                if (byRefType!.IsValueType)
                {
                    Emit.box(byRefType);
                }

                Emit.stelem_ref();
            }
        }

        if (method.ReturnType == typeof(void))
        {
            Emit.ldnull();
        }
        else if (weaklyTyped)
        {
            Emit.ifvaluetype_box(method.ReturnType);
        }

        Emit.ret();
    }

    private static void GenFieldGetter<T>(FieldInfo field)
    {
        unsafe
        {
            static void B(ILEmitter e, MemberInfo f) => e.ldc_i4_1();
            static void I1(ILEmitter e, MemberInfo f) => e.ldc_i4_s((sbyte)((FieldInfo)f).GetRawConstantValue()!);
            static void U1(ILEmitter e, MemberInfo f) => e.ldc_i4_s((byte)((FieldInfo)f).GetRawConstantValue()!);
            static void I4(ILEmitter e, MemberInfo f) => e.ldc_i4((int)((FieldInfo)f).GetRawConstantValue()!);
            static void U4(ILEmitter e, MemberInfo f) => e.ldc_i4((uint)((FieldInfo)f).GetRawConstantValue()!);
            static void I8(ILEmitter e, MemberInfo f) => e.ldc_i8((long)((FieldInfo)f).GetRawConstantValue()!);
            static void U8(ILEmitter e, MemberInfo f) => e.ldc_i8((ulong)((FieldInfo)f).GetRawConstantValue()!);
            static void F4(ILEmitter e, MemberInfo f) => e.ldc_r4((float)((FieldInfo)f).GetRawConstantValue()!);
            static void F8(ILEmitter e, MemberInfo f) => e.ldc_r8((double)((FieldInfo)f).GetRawConstantValue()!);
            static void Str(ILEmitter e, MemberInfo f) => e.ldstr((string)((FieldInfo)f).GetRawConstantValue()!);
            static void Fld(ILEmitter e, MemberInfo f) => e.lodfld((FieldInfo)f);

            delegate*<ILEmitter, MemberInfo, void> emitGetter;

            if (field.IsLiteral)
            {
                if (field.FieldType == typeof(bool)) { emitGetter = &B; }
                else if (field.FieldType == typeof(sbyte)) { emitGetter = &I1; }
                else if (field.FieldType == typeof(byte)) { emitGetter = &U1; }
                else if (field.FieldType == typeof(short) || field.FieldType == typeof(int)) { emitGetter = &I4; }
                else if (field.FieldType == typeof(ushort) || field.FieldType == typeof(uint)) { emitGetter = &U4; }
                else if (field.FieldType == typeof(long)) { emitGetter = &I8; }
                else if (field.FieldType == typeof(ulong)) { emitGetter = &U8; }
                else if (field.FieldType == typeof(float)) { emitGetter = &F4; }
                else if (field.FieldType == typeof(double)) { emitGetter = &F8; }
                else if (field.FieldType == typeof(string)) { emitGetter = &Str; }
                else { ThrowHelper.ThrowNotSupportedException($"Creating a FieldGetter for type: {field.FieldType.Name} is unsupported."); return; }
            }
            else { emitGetter = &Fld; }

            GenMemberGetter<T>(field, field.FieldType, field.IsStatic, emitGetter);
        }
    }

    private static void GenPropertyGetter<T>(PropertyInfo property)
    {
        static void EmitGetter(ILEmitter e, MemberInfo p) => e.callorvirt(((PropertyInfo)p).GetGetMethod(true));

        unsafe
        {
            GenMemberGetter<T>(property, property.PropertyType, property.GetGetMethod(true)!.IsStatic, &EmitGetter);
        }
    }

    private static unsafe void GenMemberGetter<T>(MemberInfo member, Type memberType, bool isStatic, delegate*<ILEmitter, MemberInfo, void> get)
    {
        if (typeof(T) == typeof(object)) // weakly-typed?
        {
            // if we're static immediately load member and return value
            // otherwise load and cast target, get the member value and box it if neccessary:
            // return ((DeclaringType)target).member;
            if (!isStatic)
            {
                Debug.Assert(member.DeclaringType is not null);

                Emit.ldarg0()
                    .unboxorcast(member.DeclaringType);
            }

            Emit.perform(get, member)
                .ifvaluetype_box(memberType);
        }
        else // we're strongly-typed, don't need any casting or boxing
        {
            // if we're static return member value immediately
            // otherwise load target and get member value immeidately
            // return target.member;
            if (!isStatic)
            {
                Emit.ifclass_ldarg_else_ldarga(0, typeof(T));
            }

            Emit.perform(get, member);
        }

        Emit.ret();
    }

    private static void GenFieldSetter<T>(FieldInfo field)
    {
        static void EmitSetter(ILEmitter e, MemberInfo f) => e.setfld((FieldInfo)f);

        unsafe
        {
            GenMemberSetter<T>(field, field.FieldType, field.IsStatic, &EmitSetter);
        }
    }

    private static void GenPropertySetter<T>(PropertyInfo property)
    {
        static void EmitSetter(ILEmitter e, MemberInfo p) => e.callorvirt(((PropertyInfo)p).GetSetMethod(true));

        unsafe
        {
            GenMemberSetter<T>(property, property.PropertyType, property.GetSetMethod(true)!.IsStatic, &EmitSetter);
        }
    }

    private static unsafe void GenMemberSetter<T>(MemberInfo member, Type memberType, bool isStatic, delegate*<ILEmitter, MemberInfo, void> set)
    {
        Type? targetType = typeof(T);
        bool stronglyTyped = targetType != typeof(object);

        // if we're static set member immediately
        if (isStatic)
        {
            Emit.ldarg1();
            if (!stronglyTyped)
            {
                Emit.unbox_any(memberType);
            }

            Emit.perform(set, member)
                .ret();
            return;
        }

        if (stronglyTyped)
        {
            // push target and value argument, set member immediately
            // target.member = value;
            Emit.ldarg0()
                .ifclass_ldind_ref(targetType)
                .ldarg1()
                .perform(set, member)
                .ret();
            return;
        }

        // we're weakly-typed
        targetType = member.DeclaringType;
        if (!targetType!.IsValueType) // are we a reference-type?
        {
            // load and cast target, load and cast value and set
            // ((TargetType)target).member = (MemberType)value;
            Emit.ldarg0()
                .ldind_ref()
                .cast(targetType)
                .ldarg1()
                .unbox_any(memberType)
                .perform(set, member)
                .ret();
            return;
        }

        // we're a value-type
        // handle boxing/unboxing for the user so he doesn't have to do it himself
        // here's what we're basically generating (remember, we're weakly typed, so
        // the target argument is of type object here):
        // TargetType tmp = (TargetType)target; // unbox
        // tmp.member = (MemberField)value;		// set member value
        // target = tmp;						// box back

        Emit.declocal(targetType);
        Emit.ldarg0()
            .ldind_ref()
            .unbox_any(targetType)
            .stloc0()
            .ldloca(0)
            .ldarg1()
            .unbox_any(memberType)
            .perform(set, member)
            .ldarg0()
            .ldloc0()
            .box(targetType)
            .stind_ref()
            .ret();
    }

    [DoesNotReturn]
    private static void ThrowInvalidCtorParams(Type? targetType, Type?[]? paramTypes)
    {
        using StrBuilder msg = new(stackalloc char[2046]);
        msg.Append("Generating constructor for type: ");
        msg.Append(targetType?.ToString());
        if (paramTypes is null || paramTypes.Length == 0)
        {
            msg.Append("No empty constructor found!");
        }
        else
        {
            msg.Append("No constructor found that matches the following parameter types: ");
            msg.Append(paramTypes[0]?.Name);
            for (var i = 1; i < paramTypes.Length; i++)
            {
                msg.Append(',');
                msg.Append(paramTypes[i]?.Name);
            }
        }
        ThrowHelper.ThrowInvalidOperationException(msg.ToString());
    }

    /// <summary>16 byte integer Guid</summary>
    public readonly struct Key : IEquatable<Key>
    {
        public readonly int Caller;
        public readonly int Member;
        public readonly int Target;
        public readonly int Return;

        public Key(string callerName, int member, Type target, Type ret)
        {
            Caller = callerName.GetHashCode();
            Member = member;
            Target = target.GetHashCode();
            Return = ret.GetHashCode();
        }

        public bool Equals(Key other)
        {
            return Caller == other.Caller && Member == other.Member && Target == other.Target && Return == other.Return;
        }

        public override bool Equals(object? obj)
        {
            return obj is Key other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(Caller, Member, Target, Return);
        }

        /// <summary>
        /// Returns true if the two keys are equal; otherwise, false.
        /// </summary>
        public static bool operator ==(Key left, Key right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two keys are not equal; otherwise, false.
        /// </summary>
        public static bool operator !=(Key left, Key right)
        {
            return !(left == right);
        }
    }

    internal static Key GetKey<T, R>(string callerName, MemberInfo member)
    {
        return new Key(callerName, member.GetHashCode(), typeof(T), typeof(R));
    }

    internal static Key GetKey(string callerName, Type target, Type ret, Type[] paramTypes)
    {
        HashCode paramHash = new();
        foreach (Type? t in paramTypes)
        {
            paramHash.Add(t);
        }
        return new Key(callerName, paramHash.ToHashCode(), target, ret);
    }
}
