using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Rustic.Common;

namespace Rustic.Memory;

/// <summary>
///     Enables unaligned marking of bits in a memory area.
/// </summary>
public readonly ref struct BitSet
{
    private const int IntSize = sizeof(int) * 8;
    private static readonly int IntShift = Arithmetic.Log2(IntSize);
    private readonly Span<int> _span;

    /// <summary>
    ///     initializes a new instance of <see cref="BitSet"/>.
    /// </summary>
    /// <param name="span">The span of 32bit integers to be used for marking bits.</param>
    public BitSet(Span<int> span)
    {
        _span = span;
    }

    /// <summary>Returns the raw storage of the <see cref="BitSet"/> readonly.</summary>
    public ReadOnlySpan<int> RawStorage => _span;

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

    /// <summary>Sets the bit at the index.</summary>
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

    /// <summary>Gets the value indicating wether the bit at the index is marked.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsMarked(int bitIndex)
    {
        int bitArrayIndex = bitIndex >> IntShift;
        if ((uint)bitArrayIndex < (uint)_span.Length)
        {
            int mask = 1 << (bitIndex % IntSize);
            return (_span[bitArrayIndex] & mask) != 0;
        }
        Debug.Fail("Bit index out of range");
        return false;
    }
}
