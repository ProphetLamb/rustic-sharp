﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic;

/// <summary>Collection of extension methods and utility functions related to integer arithmetic.</summary>
/// <remarks>Most functions are ported from https://graphics.stanford.edu/~seander/bithacks.html</remarks>
public static class Arithmetic {
    private static ReadOnlySpan<ulong> DigitCountTable => new ulong[32] {
        4294967296,
        8589934582,
        8589934582,
        8589934582,
        12884901788,
        12884901788,
        12884901788,
        17179868184,
        17179868184,
        17179868184,
        21474826480,
        21474826480,
        21474826480,
        21474826480,
        25769703776,
        25769703776,
        25769703776,
        30063771072,
        30063771072,
        30063771072,
        34349738368,
        34349738368,
        34349738368,
        34349738368,
        38554705664,
        38554705664,
        38554705664,
        41949672960,
        41949672960,
        41949672960,
        42949672960,
        42949672960,
    };

    private static ReadOnlySpan<byte> MultiplyDeBruijnBitPosition => new byte[32] {
        00,
        09,
        01,
        10,
        13,
        21,
        02,
        29,
        11,
        14,
        16,
        18,
        22,
        25,
        03,
        30,
        08,
        12,
        20,
        28,
        15,
        17,
        24,
        07,
        19,
        27,
        23,
        06,
        26,
        05,
        04,
        31,
    };

    private static ReadOnlySpan<int> DeBruijnSequenceBitPosition => new int[32] {
        0,
        1,
        28,
        2,
        29,
        14,
        24,
        3,
        30,
        22,
        20,
        15,
        25,
        17,
        4,
        8,
        31,
        27,
        13,
        23,
        21,
        19,
        16,
        7,
        26,
        12,
        18,
        6,
        11,
        5,
        10,
        9,
    };

    private static ReadOnlySpan<uint> Int32Power10 =>
        new uint[10] {1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000,};

    private static ReadOnlySpan<ulong> Int64Power10 => new ulong[19] {
        1,
        10,
        100,
        1000,
        10000,
        100000,
        1000000,
        10000000,
        100000000,
        1000000000,
        10000000000,
        100000000000,
        1000000000000,
        10000000000000,
        100000000000000,
        1000000000000000,
        10000000000000000,
        100000000000000000,
        1000000000000000000,
    };

    /// <summary>The size of any given 32bit signed integer.</summary>
    /// <remarks>Its 32.</remarks>
    public const int IntWidth = sizeof(int) * 8;

    /// <summary>The number of shifts required to obtain the number of b in a 32-bit signed integer and vice versa.</summary>
    public static readonly int IntShift = Log2(IntWidth);

    /// <summary>The size of any given native signed integer.</summary>
    public static readonly int PtrWidth = IntPtr.Size * 8;

    /// <summary>The number of integers required to represent a minimum of <paramref name="n"/> b.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IntsToContainBits(in int n) {
        return n > 0 ? ((n - 1) >> IntShift) + 1 : 0;
    }

    /// <summary>Returns the number of b represented by the number of <paramref name="n"/> b.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitsContainedInInts(in int n) {
        return n << IntShift;
    }

    /// <summary>Negates the <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Negate(in this int value) {
        return (value ^ -1) + 1;
    }

    /// <summary>Negates the <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Negate(in this long value) {
        return (value ^ -1) + 1;
    }

    /// <summary>Negates the <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Negate(this nint value) {
        return (value ^ -1) + 1;
    }

    /// <summary>Negates the value <paramref name="negate"/>, if the condition <paramref name="value"/> is 1.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NegateIf(in this int negate, in int value) {
        Debug.Assert((negate & 1) == negate);
        return (value ^ -negate) + negate;
    }

    /// <summary>Negates the value <paramref name="negate"/>, if the condition <paramref name="value"/> is 1.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NegateIf(in this long negate, in long value) {
        Debug.Assert((negate & 1) == negate);
        return (value ^ -negate) + negate;
    }

    /// <summary>Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).</summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static ulong GetFastModMultiplier(in uint divisor) {
        return 0xFFFFFFFFFFFFFFFFul / divisor + 1;
    }

    /// <summary>Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.</summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static uint FastMod(in uint value, in uint divisor, in ulong multiplier) {
        Debug.Assert(IntPtr.Size == 8);
        // We use modified Daniel Lemire's fastmod algorithm (https://github.com/dotnet/runtime/pull/406),
        // which allows to avoid the long multiplication if the divisor is less than 2**31.
        Debug.Assert(divisor <= int.MaxValue);

        // This is equivalent of (uint)Math.BigMul(multiplier * value, divisor, out _). This version
        // is faster than BigMul currently because we only need the high b.
        uint hi = (uint) (((((multiplier * value) >> 32) + 1) * divisor) >> 32);

        Debug.Assert(hi == value % divisor);
        return hi;
    }

    /// <summary>Performs a mod operation on a 32bit signed integer where the divisor is a multiple of 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FastMod2(in this int value, in int divisor) {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 32bit unsigned integer where the divisor is a multiple of 2.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static uint FastMod2(in this uint value, in uint divisor) {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 64bit signed integer where the divisor is a multiple of 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FastMod2(in this long value, in long divisor) {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 64bit unsigned integer where the divisor is a multiple of 2.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static ulong FastMod2(in this ulong value, in ulong divisor) {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Determines whether the <pramref name="value"/> contains one or more zeroed bytes.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static uint HasZeroByte(in this uint value) {
        return (value - 0x01010101u) & ~value & 0x80808080u;
    }

    /// <summary>Determines whether the <pramref name="value"/> contains one or more zeroed bytes.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static ulong HasZeroByte(in this ulong value) {
        return (value - 0x0101010101010101ul) & ~value & 0x8080808080808080ul;
    }

    /// <summary>MurrMurrHash3 bit mixer.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static ulong Mix(this ulong key) {
        key ^= key >> 33;
        key *= 0xff51afd7ed558ccd;
        key ^= key >> 33;
        key *= 0xc4ceb9fe1a85ec53;
        key ^= key >> 33;

        return key;
    }

    /// <summary>Trained low entropy bit mixer.</summary>
    /// <remarks>Source: https://zimbry.blogspot.com/2011/09/better-bit-mixing-improving-on.html</remarks>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static ulong Mix2(this ulong key) {
        key ^= key >> 33;
        key *= 0x3cd0eb9d47532dfb;
        key ^= key >> 33;
        key *= 0x63660277528772bb;
        key ^= key >> 33;

        return key;
    }

    /// <summary>Computes the unchecked absolute of a value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(in this int v) {
        int mask = v >> (sizeof(int) * 8 - 1);
        return (v + mask) ^ mask;
    }

    /// <summary>Computes the unchecked absolute of a value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Abs(in this long v) {
        long mask = v >> (sizeof(long) * 8 - 1);
        return (v + mask) ^ mask;
    }

    /// <summary>Computes the unchecked absolute of a value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Abs(this nint v) {
        unsafe {
            nint mask = v >> (sizeof(nint) * 8 - 1);
            return (v + mask) ^ mask;
        }
    }

    /// <summary>Computes the maximum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(in this int x, in int y) {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return x - ((x - y) & ((x - y) >> 31));
    }

    /// <summary>Computes the minimum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(in this int x, in int y) {
        Debug.Assert(x.Abs() == x && y.Abs() == y);
        return y + ((x - y) & ((x - y) >> 31));
    }

    /// <summary>Computes the maximum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(in this long x, in long y) {
        Debug.Assert(x.Abs() == x && y.Abs() == y);
        return x - ((x - y) & ((x - y) >> 63));
    }

    /// <summary>Computes the minimum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(in this long x, in long y) {
        Debug.Assert(x.Abs() == x && y.Abs() == y);
        return y + ((x - y) & ((x - y) >> 63));
    }

    /// <summary>Computes the maximum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(this nint x, nint y) {
        Debug.Assert(x.Abs() == x && y.Abs() == y);
        return x - ((x - y) & ((x - y) >> (PtrWidth - 1)));
    }

    /// <summary>Computes the minimum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(this nint x, nint y) {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return y + ((x - y) & ((x - y) >> (PtrWidth - 1)));
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(in int value) {
        return (value & (value - 1)) == 0 && value > 0;
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static bool IsPow2(in uint value) {
        return (value & (value - 1)) == 0 && value != 0;
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(in long value) {
        return (value & (value - 1)) == 0 && value > 0;
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static bool IsPow2(in ulong value) {
        return (value & (value - 1)) == 0 && value != 0;
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(nint value) {
        return (value & (value - 1)) == 0 && value > 0;
    }

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static bool IsPow2(nuint value) {
        return (value & (value - 1)) == 0 && value != 0;
    }

    /// <summary>Computes the number of decimal digits required to represent the integer value.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int DigitCount(in this uint value) {
        return (int) ((value + Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(DigitCountTable),
            (IntPtr) Log2(value)
        )) >> 32);
    }

    /// <summary>Computes the base 10 logarithm of on a 64bit unsigned integer value.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int Log10(in this uint value) {
        int l2 = ((Log2(value) + 1) * 1233) >> 12;
        return l2 - (value < Int32Power10[l2] ? 1 : 0);
    }

    /// <summary>Computes the base 10 logarithm of on a 64bit unsigned integer value.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int Log10(in this ulong value) {
        int l2 = ((Log2(value) + 1) * 1233) >> 12;
        return l2 - (value < Int64Power10[l2] ? 1 : 0);
    }

    /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
    /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int Log2(in this uint value) {
        return Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(MultiplyDeBruijnBitPosition),
            (IntPtr) (int) (((value & 0xFFFFFFFE) * 0x07C4ACDDU) >> 27)
        );
    }

    /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
    /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int Log2(in this ulong value) {
        uint hi = (uint) (value >> 32);
        if (hi == 0) // Log2 < 32 ?
        {
            return Log2((uint) value);
        }

        return 32 + Log2(hi);
    }

    /// <summary>If the value is even, returns the value; otherwise increases the value by one.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static uint RoundUpToEven(this uint value) {
        value--;
        value = FillTailingZeros(value);
        value++;

        return value;
    }

    /// <summary>Fills tailing zero b with ones.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static uint FillTailingZeros(this uint value) {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        return value;
    }

    /// <summary>Counts the number of tailing zero b.</summary>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining),]
    public static int CountTrailingZeroBits(in this uint value) {
        // Unsafe AddByteOffset is remarkably faster then the indexer.
        return Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(DeBruijnSequenceBitPosition),
            (IntPtr) ((int) (uint) ((value & -value) * 0x077CB531U) >> 27)
        );
    }

    [StructLayout(LayoutKind.Explicit)]
    private ref struct DFBits {
        [FieldOffset(0)]
        public ulong Bits;
        [FieldOffset(0)]
        public double Value;
    }

    /// <summary>Returns the storage of the value.</summary>
    [CLSCompliant(false)]
    public static ulong GetBits(in this double v) {
        DFBits union = new();
        union.Value = v;
        return union.Bits;
    }

    /// <summary>Returns the floating-point number of the storage.</summary>
    [CLSCompliant(false)]
    public static double FromBits(in ulong b) {
        DFBits union = new();
        union.Bits = b;
        return union.Value;
    }

    [StructLayout(LayoutKind.Explicit)]
    private ref struct SFBits {
        [FieldOffset(0)]
        public uint Bits;
        [FieldOffset(0)]
        public float Value;
    }

    /// <summary>Returns the storage of the value.</summary>
    [CLSCompliant(false)]
    public static uint GetBits(in this float v) {
        SFBits union = new();
        union.Value = v;
        return union.Bits;
    }

    /// <summary>Returns the floating-point number of the storage.</summary>
    [CLSCompliant(false)]
    public static float FromBits(in uint b) {
        SFBits union = new();
        union.Bits = b;
        return union.Value;
    }


    /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static uint RotateLeft(in this uint value, in int offset) {
        return (value << offset) | (value >> (32 - offset));
    }

    /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static ulong RotateLeft(in this ulong value, in int offset) {
        return (value << offset) | (value >> (64 - offset));
    }

    /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
    /// and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static nuint RotateLeft(in this nuint value, in int offset) {
#if TARGET_64BIT
            return (nuint)RotateLeft((ulong)value, offset);
#else
        return (nuint) RotateLeft((uint) value, offset);
#endif
    }

    /// <summary>
    /// Rotates the specified value right by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROR.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static uint RotateRight(in this uint value, in int offset) {
        return (value >> offset) | (value << (32 - offset));
    }

    /// <summary>
    /// Rotates the specified value right by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROR.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static ulong RotateRight(in this ulong value, in int offset) {
        return (value >> offset) | (value << (64 - offset));
    }

    /// <summary>
    /// Rotates the specified value right by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROR.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
    /// and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false),]
    public static nuint RotateRight(in this nuint value, in int offset) {
#if TARGET_64BIT
            return (nuint)RotateRight((ulong)value, offset);
#else
        return (nuint) RotateRight((uint) value, offset);
#endif
    }

    // Benchmarks show, that the bitwise iterative approach is significantly faster: https://gist.github.com/ec6c6fe21f7b2b59fb604d116d8b6261
    // |                            Method | Base | Exponent |      Mean |     Error |    StdDev |    Median | Rank | Allocated |
    // |---------------------------------- |----- |--------- |----------:|----------:|----------:|----------:|-----:|----------:|
    // |       Benchmark_pow_bitwise_qword |   10 |        6 |  4.375 ns | 0.0421 ns | 0.0352 ns |  4.376 ns |    1 |         - |
    // |     Benchmark_pow_bitwise_ptrsize |   12 |        6 |  4.806 ns | 0.0978 ns | 0.0915 ns |  4.792 ns |    2 |         - |
    // |       Benchmark_pow_bitwise_qword |   12 |        5 |  4.854 ns | 0.0742 ns | 0.0579 ns |  4.849 ns |    2 |         - |
    // |       Benchmark_pow_bitwise_qword |   18 |        5 |  4.942 ns | 0.0912 ns | 0.0761 ns |  4.909 ns |    2 |         - |
    // ...
    // |   Benchmark_pow_native_cast_dword |   18 |        9 | 38.327 ns | 0.7831 ns | 0.7692 ns | 38.248 ns |   12 |         - |
    // | Benchmark_pow_native_cast_ptrsize |   12 |        5 | 45.079 ns | 3.4366 ns | 9.9154 ns | 40.139 ns |   13 |         - |
    // |   Benchmark_pow_native_cast_dword |   10 |        5 | 78.301 ns | 1.2663 ns | 1.1226 ns | 78.322 ns |   14 |         - |

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    [CLSCompliant(false)]
    public static uint Pow(this uint a, uint n) {
        // Benchmarks show that there is a small benefit to running on native sized integer
        return (uint) Pow((nuint) a, (nuint) n);
    }

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    public static int Pow(this int a, int n) {
        return (int) Pow((nint) a, (nint) n);
    }

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    [CLSCompliant(false)]
    public static ulong Pow(this ulong a, ulong n) {
        ulong ans = 1;

        while (n > 0) {
            ulong even = n & 1;

            // Check if current LSB is set
            ans *= even > 0 ? a : 1;
            a *= a;

            // Right shift
            n >>= 1;
        }

        return ans;
    }

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    public static long Pow(this long a, long n) {
        long ans = 1;

        while (n > 0) {
            long even = n & 1;

            // Check if current LSB is set
            ans *= even > 0 ? a : 1;
            a *= a;

            // Right shift
            n >>= 1;
        }

        return ans;
    }

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    [CLSCompliant(false)]
    public static nuint Pow(this nuint a, nuint n) {
        nuint ans = 1;

        while (n > 0) {
            nuint even = n & 1;

            // Check if current LSB is set
            ans *= even > 0 ? a : 1;
            a *= a;

            // Right shift
            n >>= 1;
        }

        return ans;
    }

    /// <summary>
    /// Computes the base <paramref name="a"/> to the power of <paramref name="n"/>.
    /// </summary>
    /// <param name="a">The base</param>
    /// <param name="n">The exponent</param>
    /// <returns>The result of the operation <c>a^n</c>.</returns>
    [CLSCompliant(false)]
    public static nint Pow(this nint a, nint n) {
        nint ans = 1;

        while (n > 0) {
            nint even = n & 1;

            // Check if current LSB is set
            ans *= even > 0 ? a : 1;
            a *= a;

            // Right shift
            n >>= 1;
        }

        return ans;
    }
}
