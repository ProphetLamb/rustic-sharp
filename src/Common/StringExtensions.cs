using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.Validation;

namespace Rustic;

/// <inheritdoc cref="String"/>
public static class StringExtensions
{
    /// <inheritdoc cref="String.IsNullOrEmpty(String)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty([ValidatedNotNull] this string? s)
    {
        return String.IsNullOrEmpty(s);
    }

    /// <inheritdoc cref="String.IsNullOrWhiteSpace(String)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace([ValidatedNotNull] this string? s)
    {
        return String.IsNullOrWhiteSpace(s);
    }

    /// <inheritdoc cref="String.Join(String, IEnumerable{String})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this string sep, IEnumerable<string> values)
    {
        return String.Join(sep, values);
    }

    /// <inheritdoc cref="String.Join{T}(String, IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this string sep, IEnumerable<T> values)
    {
        return String.Join(sep, values);
    }

    /// <inheritdoc cref="String.Join(String, String[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this string sep, params string[] values)
    {
        return String.Join(sep, values);
    }

    /// <inheritdoc cref="String.Join(String, Object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this string sep, params object?[] values)
    {
        return String.Join(sep, values);
    }
}
