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
        private static readonly int IntShift = BitHelper.Log2(IntSize);
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
                _span[bitArrayIndex] |= 1 << (int)((uint)bitIndex).FastMod2(IntSize);
            }
        }

        /// <summary>Sets the bit at the index to zero.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unmark(int bitIndex)
        {
            int bitArrayIndex = bitIndex >> IntShift;
            if ((uint)bitArrayIndex < (uint)_span.Length)
            {
                _span[bitArrayIndex] &= ~(1 << (int)((uint)bitIndex).FastMod2(IntSize));
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
    }
}
