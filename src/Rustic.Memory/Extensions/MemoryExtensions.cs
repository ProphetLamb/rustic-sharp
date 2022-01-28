using System.Collections.Generic;

using Rustic.Common;
using Rustic.Memory.Common;

#pragma warning disable IDE0130
namespace System;
#pragma warning restore IDE0130

/// <summary>
///     Extensions for <see cref="Span{T}"/>, <see cref="ReadOnlySpan{T}"/>.
/// </summary>
public static class MemoryExtensions
{
#if !NET5_0_OR_GREATER

    /// <summary>
    ///     Sorts the elements in the entire <see cref="Span{T}" /> using the <see cref="IComparable{T}" /> implementation of each element of the <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="span">>The <see cref="Span{T}"/> to sort.</param>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    public static void Sort<T>(this Span<T> span)
    {
        Sort(span, Comparer<T>.Default);
    }

    /// <summary>
    ///     Sorts the elements in the entire <see cref="Span{T}" /> using the <typeparamref name="C" />.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
    /// <param name="comparer"></param>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    /// <typeparam name="C">The type of the comparer to use to compare elements.</typeparam>
    public static void Sort<T, C>(this Span<T> span, C comparer)
        where C : IComparer<T>
    {
        if (span.Length > 1)
        {
            SpanSortHelper<T>.Sort(span, comparer.Compare);
        }
    }

    /// <summary>
    ///     Sorts the elements in the entire <see cref="Span{T}" /> using the specified <see cref="Comparison{T}" />.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
    /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    public static void Sort<T>(this Span<T> span, Comparison<T>? comparison)
    {
        comparison.ValidateArgNotNull();
        if (span.Length > 1)
        {
            SpanSortHelper<T>.Sort(span, comparison!);
        }
    }

#endif

    /// <summary>
    ///     Computes the union of two sets.
    /// </summary>
    /// <param name="set">The left set.</param>
    /// <param name="other">The right set.</param>
    /// <param name="output">The output buffer.</param>
    /// <typeparam name="T">The type of the elements of the sets.</typeparam>
    public static void Union<T>(in this ReadOnlySpan<T> set, in ReadOnlySpan<T> other, in Span<T> output)
        where T : IComparable<T>
    {
        int outI = 0, leftI = 0, rightI = 0;
        while ((leftI < set.Length) & (rightI < other.Length))
        {
            T v1 = set[leftI];
            T v2 = other[rightI];
            output[outI++] = v1.CompareTo(v2) <= 0 ? v1 : v2;
            leftI = v1.CompareTo(v2) <= 0 ? leftI + 1 : leftI;
            rightI = v1.CompareTo(v2) >= 0 ? rightI + 1 : rightI;
        }
    }

    /// <summary>
    ///     Computes the union of two sets.
    /// </summary>
    /// <param name="set">The left set.</param>
    /// <param name="other">The right set.</param>
    /// <param name="output">The output buffer.</param>
    /// <param name="comparer">The comparer used to compare two elements.</param>
    /// <typeparam name="T">The type of the elements of the sets.</typeparam>
    /// <typeparam name="C">The type of the comparer.</typeparam>
    public static void Union<T, C>(in this ReadOnlySpan<T> set, in ReadOnlySpan<T> other, in Span<T> output, in C comparer)
        where C : IComparer<T>
    {
        int outI = 0, leftI = 0, rightI = 0;
        while ((leftI < set.Length) & (rightI < other.Length))
        {
            T v1 = set[leftI];
            T v2 = other[rightI];
            output[outI++] = comparer.Compare(v1, v2) <= 0 ? v1 : v2;
            leftI = comparer.Compare(v1, v2) <= 0 ? leftI + 1 : leftI;
            rightI = comparer.Compare(v1, v2) >= 0 ? rightI + 1 : rightI;
        }
    }
}
