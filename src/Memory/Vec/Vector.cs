using System;
using System.Diagnostics.Contracts;

namespace Rustic.Memory;

/// <summary>
///     Extension for <see cref="Vec{T}"/>.
/// </summary>
public static class VectorExtensions
{
    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this Vec<T> self)
    {
        return self.AsSpan(0, self.Count);
    }

    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this Vec<T> self, Index index)
    {
        int len = self.Count;
        int off = index.GetOffset(len);
        return self.AsSpan(off, len - off);
    }

    /// <summary>Creates a new span over a target vector.</summary>
    /// <typeparam name="T">The type of elements in the vector.</typeparam>
    /// <returns>The span representation of the vector.</returns>
    [Pure]
    public static ReadOnlySpan<T> AsSpan<T>(this Vec<T> self, Range range)
    {
        (int off, int cnt) = range.GetOffsetAndLength(self.Count);
        return self.AsSpan(off, cnt);
    }
}
