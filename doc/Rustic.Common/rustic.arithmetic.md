# Arithmetic

Namespace: Rustic

Collection of extension methods and utility functions related to integer arithmetic.

```csharp
public static class Arithmetic
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Arithmetic](./rustic.arithmetic.md)

## Fields

### **IntShift**

The number of shifts required to obtain the number of b in a 32-bit signed integer and vice versa.

```csharp
public static int IntShift;
```

### **PtrWidth**

The size of any given native signed integer.

```csharp
public static int PtrWidth;
```

### **IntWidth**

The size of any given 32bit signed integer.

```csharp
public static int IntWidth;
```

## Methods

### **IntsToContainBits(Int32&)**

The number of integers required to represent a minimum of  b.

```csharp
public static int IntsToContainBits(Int32& n)
```

#### Parameters

`n` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BitsContainedInInts(Int32&)**

Returns the number of b represented by the number of  b.

```csharp
public static int BitsContainedInInts(Int32& n)
```

#### Parameters

`n` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Negate(Int32&)**

Negates the .

```csharp
public static int Negate(Int32& value)
```

#### Parameters

`value` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Negate(Int64&)**

Negates the .

```csharp
public static long Negate(Int64& value)
```

#### Parameters

`value` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Negate(IntPtr)**

Negates the .

```csharp
public static long Negate(IntPtr value)
```

#### Parameters

`value` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **NegateIf(Int32&, Int32&)**

Negates the value , if the condition  is 1.

```csharp
public static int NegateIf(Int32& negate, Int32& value)
```

#### Parameters

`negate` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`value` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **NegateIf(Int64&, Int64&)**

Negates the value , if the condition  is 1.

```csharp
public static long NegateIf(Int64& negate, Int64& value)
```

#### Parameters

`negate` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

`value` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **GetFastModMultiplier(UInt32&)**

Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).

```csharp
public static ulong GetFastModMultiplier(UInt32& divisor)
```

#### Parameters

`divisor` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **FastMod(UInt32&, UInt32&, UInt64&)**

Performs a mod operation using the multiplier pre-computed with Rustic.Arithmetic.GetFastModMultiplier(System.UInt32@).

```csharp
public static uint FastMod(UInt32& value, UInt32& divisor, UInt64& multiplier)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

`divisor` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

`multiplier` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **FastMod2(Int32&, Int32&)**

Performs a mod operation on a 32bit signed integer where the divisor is a multiple of 2.

```csharp
public static int FastMod2(Int32& value, Int32& divisor)
```

#### Parameters

`value` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`divisor` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **FastMod2(UInt32&, UInt32&)**

Performs a mod operation on a 32bit unsigned integer where the divisor is a multiple of 2.

```csharp
public static uint FastMod2(UInt32& value, UInt32& divisor)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

`divisor` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **FastMod2(Int64&, Int64&)**

Performs a mod operation on a 64bit signed integer where the divisor is a multiple of 2.

```csharp
public static long FastMod2(Int64& value, Int64& divisor)
```

#### Parameters

`value` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

`divisor` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **FastMod2(UInt64&, UInt64&)**

Performs a mod operation on a 64bit unsigned integer where the divisor is a multiple of 2.

```csharp
public static ulong FastMod2(UInt64& value, UInt64& divisor)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

`divisor` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **HasZeroByte(UInt32&)**

Determines whether the  contains one or more zeroed bytes.

```csharp
public static uint HasZeroByte(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **HasZeroByte(UInt64&)**

Determines whether the  contains one or more zeroed bytes.

```csharp
public static ulong HasZeroByte(UInt64& value)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Mix(UInt64)**

MurrMurrHash3 bit mixer.

```csharp
public static ulong Mix(ulong key)
```

#### Parameters

`key` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Mix2(UInt64)**

Trained low entropy bit mixer.

```csharp
public static ulong Mix2(ulong key)
```

#### Parameters

`key` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Abs(Int32&)**

Computes the unchecked absolute of a value.

```csharp
public static int Abs(Int32& v)
```

#### Parameters

`v` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Abs(Int64&)**

Computes the unchecked absolute of a value.

```csharp
public static long Abs(Int64& v)
```

#### Parameters

`v` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Abs(IntPtr)**

Computes the unchecked absolute of a value.

```csharp
public static IntPtr Abs(IntPtr v)
```

#### Parameters

`v` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **Max(Int32&, Int32&)**

Computes the maximum of two given positive integers.

```csharp
public static int Max(Int32& x, Int32& y)
```

#### Parameters

`x` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`y` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Min(Int32&, Int32&)**

Computes the minimum of two given positive integers.

```csharp
public static int Min(Int32& x, Int32& y)
```

#### Parameters

`x` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`y` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Max(Int64&, Int64&)**

Computes the maximum of two given positive integers.

```csharp
public static long Max(Int64& x, Int64& y)
```

#### Parameters

`x` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

`y` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Min(Int64&, Int64&)**

Computes the minimum of two given positive integers.

```csharp
public static long Min(Int64& x, Int64& y)
```

#### Parameters

`x` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

`y` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Max(IntPtr, IntPtr)**

Computes the maximum of two given positive integers.

```csharp
public static long Max(IntPtr x, IntPtr y)
```

#### Parameters

`x` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`y` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Min(IntPtr, IntPtr)**

Computes the minimum of two given positive integers.

```csharp
public static long Min(IntPtr x, IntPtr y)
```

#### Parameters

`x` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`y` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **IsPow2(Int32&)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(Int32& value)
```

#### Parameters

`value` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsPow2(UInt32&)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsPow2(Int64&)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(Int64& value)
```

#### Parameters

`value` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/system.int64&)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsPow2(UInt64&)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(UInt64& value)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsPow2(IntPtr)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(IntPtr value)
```

#### Parameters

`value` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsPow2(UIntPtr)**

Evaluate whether a given integral value is a power of 2.

```csharp
public static bool IsPow2(UIntPtr value)
```

#### Parameters

`value` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The value.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DigitCount(UInt32&)**

Computes the number of decimal digits required to represent the integer value.

```csharp
public static int DigitCount(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Log10(UInt32&)**

Computes the base 10 logarithm of on a 64bit unsigned integer value.

```csharp
public static int Log10(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Log10(UInt64&)**

Computes the base 10 logarithm of on a 64bit unsigned integer value.

```csharp
public static int Log10(UInt64& value)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Log2(UInt32&)**

Performs a base 2 logarithm operation on an integer using a LUT.

```csharp
public static int Log2(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Log2(UInt64&)**

Performs a base 2 logarithm operation on an integer using a LUT.

```csharp
public static int Log2(UInt64& value)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **RoundUpToEven(UInt32)**

If the value is even, returns the value; otherwise increases the value by one.

```csharp
public static uint RoundUpToEven(uint value)
```

#### Parameters

`value` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **FillTailingZeros(UInt32)**

Fills tailing zero b with ones.

```csharp
public static uint FillTailingZeros(uint value)
```

#### Parameters

`value` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **CountTrailingZeroBits(UInt32&)**

Counts the number of tailing zero b.

```csharp
public static int CountTrailingZeroBits(UInt32& value)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **GetBits(Double&)**

Returns the storage of the value.

```csharp
public static ulong GetBits(Double& v)
```

#### Parameters

`v` [Double&](https://docs.microsoft.com/en-us/dotnet/api/system.double&)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **FromBits(UInt64&)**

Returns the floating-point number of the storage.

```csharp
public static double FromBits(UInt64& b)
```

#### Parameters

`b` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

### **GetBits(Single&)**

Returns the storage of the value.

```csharp
public static uint GetBits(Single& v)
```

#### Parameters

`v` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **FromBits(UInt32&)**

Returns the floating-point number of the storage.

```csharp
public static float FromBits(UInt32& b)
```

#### Parameters

`b` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RotateLeft(UInt32&, Int32&)**

Rotates the specified value left by the specified number of bits.
 Similar in behavior to the x86 instruction ROL.

```csharp
public static uint RotateLeft(UInt32& value, Int32& offset)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..31] is treated as congruent mod 32.

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The rotated value.

### **RotateLeft(UInt64&, Int32&)**

Rotates the specified value left by the specified number of bits.
 Similar in behavior to the x86 instruction ROL.

```csharp
public static ulong RotateLeft(UInt64& value, Int32& offset)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..63] is treated as congruent mod 64.

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
The rotated value.

### **RotateLeft(UIntPtr, Int32&)**

Rotates the specified value left by the specified number of bits.
 Similar in behavior to the x86 instruction ROL.

```csharp
public static UIntPtr RotateLeft(UIntPtr value, Int32& offset)
```

#### Parameters

`value` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
            and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The rotated value.

### **RotateRight(UInt32&, Int32&)**

Rotates the specified value right by the specified number of bits.
 Similar in behavior to the x86 instruction ROR.

```csharp
public static uint RotateRight(UInt32& value, Int32& offset)
```

#### Parameters

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/system.uint32&)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..31] is treated as congruent mod 32.

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The rotated value.

### **RotateRight(UInt64&, Int32&)**

Rotates the specified value right by the specified number of bits.
 Similar in behavior to the x86 instruction ROR.

```csharp
public static ulong RotateRight(UInt64& value, Int32& offset)
```

#### Parameters

`value` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..63] is treated as congruent mod 64.

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
The rotated value.

### **RotateRight(UIntPtr, Int32&)**

Rotates the specified value right by the specified number of bits.
 Similar in behavior to the x86 instruction ROR.

```csharp
public static UIntPtr RotateRight(UIntPtr value, Int32& offset)
```

#### Parameters

`value` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The value to rotate.

`offset` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
The number of bits to rotate by.
            Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
            and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The rotated value.
