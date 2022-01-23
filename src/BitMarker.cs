using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HeaplessUtility
{
    /// <summary>
    ///     Enables unaligned marking of bits in a memory area.
    /// </summary>
    [CLSCompliant(false)]
    public readonly ref struct BitMarker
    {
        private const int IntSize = sizeof(int) * 8;
        private static readonly int IntShift = Log2Floor(IntSize);
        private static ReadOnlySpan<int> MultiplyDeBruijnBitPosition => new[]
        {
            0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
            8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
        };
        private static ReadOnlySpan<int> MultiplyDeBruijnBitPosition2 => new[]
        {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };

        private readonly Span<int> _span;

        /// <summary>
        ///     initializes a new instance of <see cref="BitMarker"/>.
        /// </summary>
        /// <param name="span">The span of 32bit integers to be used for marking bits.</param>
        public BitMarker(Span<int> span)
        {
            _span = span;
        }

        /// <summary>Sets the bit at the index to one.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mark(int bitIndex)
        {
            int bitArrayIndex = bitIndex >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                _span[bitArrayIndex] |= (1 << (int)FastMod2((uint)bitIndex, IntSize));
            }
        }

        /// <summary>Sets the bit at the index to zero.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unmark(int bitIndex)
        {
            int bitArrayIndex = bitIndex >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                _span[bitArrayIndex] &= ~(1 << (int)FastMod2((uint)bitIndex, IntSize));
            }
        }

        /// <summary>Sets the bit at the index to the least significant bit if the value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int bitIndex, int value)
        {
            Debug.Assert((value & 1) == value, "Value must be a boolean.");
            int bitArrayIndex = bitIndex >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                int mask = ~(1 << (bitIndex % IntSize));
                _span[bitArrayIndex] = (_span[bitArrayIndex] & mask) | (value * ~mask);
            }
        }

        /// <summary>Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).</summary>
        /// <remarks>This should only be used on 64-bit.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetFastModMultiplier(uint divisor)
        {
            Debug.Assert(IntPtr.Size == 8);
            return ulong.MaxValue / divisor + 1;
        }

        /// <summary>Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.</summary>
        /// <remarks>This should only be used on 64-bit.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastMod(uint value, uint divisor, ulong multiplier)
        {
            Debug.Assert(IntPtr.Size == 8);
            // We use modified Daniel Lemire's fastmod algorithm (https://github.com/dotnet/runtime/pull/406),
            // which allows to avoid the long multiplication if the divisor is less than 2**31.
            Debug.Assert(divisor <= int.MaxValue);

            // This is equivalent of (uint)Math.BigMul(multiplier * value, divisor, out _). This version
            // is faster than BigMul currently because we only need the high bits.
            uint hi = (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);

            Debug.Assert(hi == value % divisor);
            return hi;
        }

        /// <summary>Performs a mod operation on a 32bit unsigned integer where the divisor is a multiple of 2.</summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastMod2(uint value, uint divisor)
        {
            Debug.Assert((divisor & 1) == 0, "The divisor has to be a multiple of 2.");
            return value & (divisor - 1);
        }

        /// <summary>Performs a mod operation on a 64bit unsigned integer where the divisor is a multiple of 2.</summary>
        /// <remarks>This should only be used on 64-bit.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FastMod2(ulong value, ulong divisor)
        {
            Debug.Assert(IntPtr.Size == 8);
            Debug.Assert((divisor & 1) == 0, "The divisor has to be a multiple of 2.");
            return value & (divisor - 1);
        }

        /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(MultiplyDeBruijnBitPosition), (IntPtr)(int)((value * 0x07C4ACDDU) >> 27));
        }

        /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(ulong value)
        {
            uint hi = (uint)(value >> 32);
            if (hi == 0) // Log2 < 32 ?
            {
                return Log2((uint)value);
            }

            return 32 + Log2((uint)value);
        }

        /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
        /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(uint value)
        {
            return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(MultiplyDeBruijnBitPosition2), (IntPtr)(int)(((value & 0xFFFFFFFE) * 0x07C4ACDDU) >> 27));
        }

        /// <summary>Performs a base 2 logarithm operation on an integer using a LUT.</summary>
        /// <remarks>The value is floored to the next lowest multiple of two from the value, i.e. Log2Floor(3) == Log2(2), and Log2Floor(16) == Log2(16).</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(ulong value)
        {
            uint hi = (uint)(value >> 32);
            if (hi == 0) // Log2 < 32 ?
            {
                return Log2Floor((uint)value);
            }

            return 32 + Log2Floor(hi);
        }
    }
}
