using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Memory;

/// <summary>Utility methods assisting with handling sections of memory</summary>
public static class MemoryCopyHelper {
    /// <summary>Copies the source to the destination buffer, starting at the last element in source to the first element in destination.</summary>
    /// <param name="src">The source buffer</param>
    /// <param name="dst">The destination buffer</param>
    /// <param name="len">The number of elements to copy. Must be less or equal then the minimum of both buffer lengths.</param>
    /// <typeparam name="T">The type of the element to copy.</typeparam>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.NoInlining)]
    public static void CopyToReversed<T>(ref T src, ref T dst, nuint len) {
        ref T cur = ref Unsafe.Add(ref src, len);
        while (!Unsafe.AreSame(ref src, ref cur)) {
            dst = cur;
            dst = ref Unsafe.Add(ref dst, (nuint)1);
            cur = ref Unsafe.Subtract(ref cur, (nuint)1);
        }
    }

    /// <summary>Copies the source to the destination buffer, starting at the last element in source to the first element in destination.</summary>
    /// <param name="src">The source buffer. Must be smaller or equal in size to the destination.</param>
    /// <param name="dst">The destination buffer. Must be greater or equal in size ot the source.</param>
    /// <typeparam name="T">The type of the element to copy.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">If the source buffer is greater is size then the destination.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToReversed<T>(ReadOnlySpan<T> src, Span<T> dst) {
        if ((nuint)src.Length <= (nuint)dst.Length) {
            CopyToReversed(ref MemoryMarshal.GetReference(src), ref MemoryMarshal.GetReference(dst), (nuint)src.Length);
        }

        ThrowHelper.ThrowArgumentOutOfRangeException(nameof(dst), dst.Length);
    }

    /// <summary>Copies the source to the destination buffer, starting at the last element in source to the first element in destination.</summary>
    /// <param name="src">The source buffer. Must be smaller or equal in size to the destination.</param>
    /// <param name="dst">The destination buffer. Must be greater or equal in size ot the source.</param>
    /// <typeparam name="T">The type of the element to copy.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">If the source buffer is greater is size then the destination.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyToReversed<T>(ReadOnlySpan<T> src, Span<T> dst) {
        bool flag = false;
        if ((nuint)src.Length <= (nuint)dst.Length) {
            CopyToReversed(ref MemoryMarshal.GetReference(src), ref MemoryMarshal.GetReference(dst), (nuint)src.Length);
            flag = true;
        }

        return flag;
    }
}
