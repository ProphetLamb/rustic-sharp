using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HeaplessUtility
{
    [CLSCompliant(false)]
    public ref struct BitHelper
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

        private static ReadOnlySpan<byte> DeBruijnSequenceLog2 => new byte[32]
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

        private static ReadOnlySpan<int> Int32Power10 => new int[10]
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
        };

        private static ReadOnlySpan<long> Int64Power10 => new long[19]
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000, 100000000000, 1000000000000,
            10000000000000, 100000000000000, 1000000000000000, 10000000000000000, 100000000000000000, 1000000000000000000
        };

        private const int IntSize = 32;
        private const int IntShift = 5;

        private readonly Span<int> _span;

        public BitHelper(Span<int> span, bool clear)
        {
            if (clear)
            {
                span.Clear();
            }
            _span = span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkBit(int bitPosition)
        {
            int bitArrayIndex = bitPosition >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                _span[bitArrayIndex] |= 1 << FastModulo2(bitPosition, IntSize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToggleBit(int bitPosition)
        {
            int bitArrayIndex = bitPosition >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                return (_span[bitArrayIndex] ^= 1 << FastModulo2(bitPosition, IntSize)) != 0;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBit(int bitPosition, int value)
        {
            Debug.Assert((value & 1) == value, "Value must be either zero or one, false or true respectively.");
            int bitArrayIndex = bitPosition >> IntShift;
            int mask = 1 << FastModulo2(bitPosition, IntSize);
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                _span[bitArrayIndex] = (_span[bitArrayIndex] & ~mask) | (_span[bitArrayIndex] & (mask * value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMarked(int bitPosition)
        {
            int bitArrayIndex = bitPosition >> IntShift;
            return (uint)bitArrayIndex < (uint)_span.Length && (_span[bitArrayIndex] & (1 << (bitPosition % IntSize))) != 0;
        }

        /// <summary>
        ///     How many ints must be allocated to represent n bits. Returns (n+31)/32, but avoids overflow.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIntArrayLength(int n) => n > 0 ? (n - 1) / IntSize + 1 : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Negate(int value) => (value ^ -1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Negate(long value) => (value ^ -1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NegateIf(int negate, int value)
        {
            Debug.Assert((negate & 1) == negate);
            return (value ^ -negate) + negate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NegateIf(long negate, long value)
        {
            Debug.Assert((negate & 1) == negate);
            return (value ^ -negate) + negate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastRange(uint value, uint maximum)
        {
            return (uint)(((ulong)value * (ulong)maximum) >> 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FastModuloMultiplier(uint divisor)
        {
            return 0xFFFFFFFFFFFFFFFFul / divisor + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastModulo(uint value, uint divisor, ulong multiplier)
        {
            Debug.Assert(divisor <= Int32.MaxValue);

            uint hi = (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);

            Debug.Assert(hi == value % divisor);
            return hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastModulo2(int value, int divisor)
        {
            Debug.Assert((divisor & 1) == 0);
            return value & (divisor - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastModulo2(uint value, uint divisor)
        {
            Debug.Assert((divisor & 1) == 0);
            return value & (divisor - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FastModulo2(long value, long divisor)
        {
            Debug.Assert((divisor & 1) == 0);
            return value & (divisor - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FastModulo2(ulong value, ulong divisor)
        {
            Debug.Assert((divisor & 1) == 0);
            return value & (divisor - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint HasAlignedZeroByte(uint value) => (value - 0x01010101u) & ~value & 0x80808080u;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong HasAlignedZeroByte(ulong value) => (value - 0x0101010101010101ul) & ~value & 0x8080808080808080ul;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Mix(ulong key)
        {
            key ^= key >> 33;
            key *= 0xff51afd7ed558ccd;
            key ^= key >> 33;
            key *= 0xc4ceb9fe1a85ec53;
            key ^= key >> 33;

            return key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int x, int y)
        {
            Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
            return x - ((x - y) & ((x - y) >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int x, int y)
        {
            Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
            return y + ((x - y) & ((x - y) >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(long x, long y)
        {
            Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
            return x - ((x - y) & ((x - y) >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(long x, long y)
        {
            Debug.Assert(Math.Abs(x) == x && Math.Abs(y) == y);
            return y + ((x - y) & ((x - y) >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DigitCount(uint value)
        {
            return (int)((value + DigitCountTable[Log2(value)]) >> 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(uint value)
        {
            return DeBruijnSequenceLog2[(int)((FillTailingZeros(value) * 0x07C4ACDDu) >> 27)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log10(uint value)
        {
            int l2 = (Log2(value) + 1) * 1233 >> 12;
            return l2 - (value < Int32Power10[l2] ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RoundUpToEven(uint value)
        {
            value--;
            value = FillTailingZeros(value);
            value++;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FillTailingZeros(uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountTrailingZeroBits(uint value)
        {
            return DeBruijnSequenceBitPosition[(int)(uint)((value & -value) * 0x077CB531U) >> 27];
        }
    }
}
