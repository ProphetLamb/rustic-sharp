using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Common;

/// <summary>Collection of extension methods and utility functions related to integer arithmetic.</summary>
/// <remarks>Most functions are ported from https://graphics.stanford.edu/~seander/bithacks.html</remarks>
public static class Arithmetic
{
    private static ReadOnlySpan<ulong> DigitCountTable => new ulong[32]
    {
            4294967296, 8589934582, 8589934582, 8589934582, 12884901788,
            12884901788, 12884901788, 17179868184, 17179868184, 17179868184,
            21474826480, 21474826480, 21474826480, 21474826480, 25769703776,
            25769703776, 25769703776, 30063771072, 30063771072, 30063771072,
            34349738368, 34349738368, 34349738368, 34349738368, 38554705664,
            38554705664, 38554705664, 41949672960, 41949672960, 41949672960,
            42949672960, 42949672960
    };

    private static ReadOnlySpan<byte> MultiplyDeBruijnBitPosition => new byte[32]
    {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
    };

    private static ReadOnlySpan<int> DeBruijnSequenceBitPosition => new int[32]
    {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
    };

    private static ReadOnlySpan<uint> Int32Power10 => new uint[10]
    {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
    };

    private static ReadOnlySpan<ulong> Int64Power10 => new ulong[19]
    {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000, 100000000000, 1000000000000,
            10000000000000, 100000000000000, 1000000000000000, 10000000000000000, 100000000000000000, 1000000000000000000
    };

    /// <summary>The size of any given 32bit signed interger.</summary>
    /// <remarks>Its 32.</remarks>
    public const int IntWidth = sizeof(int) * 8;

    /// <summary>The number of shifts required to obtain the number of bits in a 32-bit signed integer and vice versa.</summary>
    public static readonly int IntShift = Log2(IntWidth);

    /// <summary>The number of integers required to represent a minimum of <paramref name="n"/> bits.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IntsToContainBits(int n) => n > 0 ? ((n - 1) >> IntShift) + 1 : 0;

    /// <summary>Returns the number of bits represented by the number of <paramref name="n"/> bits.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitsContainedInInts(int n) => n << IntShift;

    /// <summary>Negates the <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Negate(this int value) => (value ^ -1) + 1;

    /// <summary>Negates the <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Negate(this long value) => (value ^ -1) + 1;

    /// <summary>Negates the value <paramref name="negate"/>, if the condition <paramref name="value"/> is 1.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NegateIf(this int negate, int value)
    {
        Debug.Assert((negate & 1) == negate);
        return (value ^ -negate) + negate;
    }

    /// <summary>Negates the value <paramref name="negate"/>, if the condition <paramref name="value"/> is 1.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NegateIf(this long negate, long value)
    {
        Debug.Assert((negate & 1) == negate);
        return (value ^ -negate) + negate;
    }

    /// <summary>Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).</summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetFastModMultiplier(uint divisor)
    {
        return (0xFFFFFFFFFFFFFFFFul / divisor) + 1;
    }

    /// <summary>Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.</summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint FastMod(uint value, uint divisor, ulong multiplier)
    {
        Debug.Assert(IntPtr.Size == 8);
        // We use modified Daniel Lemire's fastmod algorithm (https://github.com/dotnet/runtime/pull/406),
        // which allows to avoid the long multiplication if the divisor is less than 2**31.
        Debug.Assert(divisor <= Int32.MaxValue);

        // This is equivalent of (uint)Math.BigMul(multiplier * value, divisor, out _). This version
        // is faster than BigMul currently because we only need the high bits.
        uint hi = (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);

        Debug.Assert(hi == value % divisor);
        return hi;
    }

    /// <summary>Performs a mod operation on a 32bit signed integer where the divisor is a multiple of 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FastMod2(this int value, int divisor)
    {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 32bit unsigned integer where the divisor is a multiple of 2.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint FastMod2(this uint value, uint divisor)
    {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 64bit signed integer where the divisor is a multiple of 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FastMod2(this long value, long divisor)
    {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Performs a mod operation on a 64bit unsigned integer where the divisor is a multiple of 2.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong FastMod2(this ulong value, ulong divisor)
    {
        Debug.Assert((divisor & 1) == 0);
        return value & (divisor - 1);
    }

    /// <summary>Determines whether the <pramref name="value"/> contains one or more zeroed bytes.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HasZeroByte(this uint value) => (value - 0x01010101u) & ~value & 0x80808080u;

    /// <summary>Determines whether the <pramref name="value"/> contains one or more zeroed bytes.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong HasZeroByte(this ulong value) => (value - 0x0101010101010101ul) & ~value & 0x8080808080808080ul;

    /// <summary>MurrMurrHash3 bit mixer.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Mix(this ulong key)
    {
        key ^= key >> 33;
        key *= 0xff51afd7ed558ccd;
        key ^= key >> 33;
        key *= 0xc4ceb9fe1a85ec53;
        key ^= key >> 33;

        return key;
    }

    /// <summary>Trained low entropy bit mixer.</summary>
    /// <remarks>Source: https://zimbry.blogspot.com/2011/09/better-bit-mixing-improving-on.html</remarks>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Mix2(this ulong key)
    {
        key ^= key >> 33;
        key *= 0x3cd0eb9d47532dfb;
        key ^= key >> 33;
        key *= 0x63660277528772bb;
        key ^= key >> 33;

        return key;
    }

    /// <summary>Computes the maximum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(this int x, int y)
    {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return x - ((x - y) & ((x - y) >> 31));
    }

    /// <summary>Computes the minimum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(this int x, int y)
    {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return y + ((x - y) & ((x - y) >> 31));
    }

    /// <summary>Computes the maximum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(this long x, long y)
    {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return x - ((x - y) & ((x - y) >> 63));
    }

    /// <summary>Computes the minimum of two given positive integers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(this long x, long y)
    {
        Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
        return y + ((x - y) & ((x - y) >> 63));
    }

    /// <summary>Computes the number of decimal digits required to represent the integer value.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DigitCount(this uint value)
    {
        return (int)((value + Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(DigitCountTable), (IntPtr)Log2(value))) >> 32);
    }

    /// <summary>Computes the base 10 logarithm of on a 64bit unsigned integer value.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log10(this uint value)
    {
        int l2 = ((Log2(value) + 1) * 1233) >> 12;
        return l2 - (value < Int32Power10[l2] ? 1 : 0);
    }

    /// <summary>Computes the base 10 logarithm of on a 64bit unsigned integer value.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log10(this ulong value)
    {
        int l2 = ((Log2(value) + 1) * 1233) >> 12;
        return l2 - (value < Int64Power10[l2] ? 1 : 0);
    }

    /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
    /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(this uint value)
    {
        return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(MultiplyDeBruijnBitPosition), (IntPtr)(int)(((value & 0xFFFFFFFE) * 0x07C4ACDDU) >> 27));
    }

    /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
    /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(this ulong value)
    {
        uint hi = (uint)(value >> 32);
        if (hi == 0) // Log2 < 32 ?
        {
            return Log2((uint)value);
        }

        return 32 + Log2(hi);
    }

    /// <summary>If the value is even, returns the value; otherwise increases the value by one.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToEven(this uint value)
    {
        value--;
        value = FillTailingZeros(value);
        value++;

        return value;
    }

    /// <summary>Fills tailing zero bits with ones.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint FillTailingZeros(this uint value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        return value;
    }

    /// <summary>Counts the number of tailing zero bits.</summary>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountTrailingZeroBits(this uint value)
    {
        // Unsafe AddByteOffset is remarkably faster then the indexer.
        return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(DeBruijnSequenceBitPosition), (IntPtr)((int)(uint)((value & -value) * 0x077CB531U) >> 27));
    }
}
