using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.VisualStudio.Validation;

namespace Rustic.Json;

/// <summary>Centralized functionality related to validation and throwing exceptions.</summary>
#pragma warning disable RCS1138,CS1591
public static class ThrowHelper
{

    #region JsonExceptions

    [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowJsonException(string message, string? path = null, Exception? inner = null)
    {
        throw new JsonException(message, path, default, default, inner);
    }

    [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowJsonUnexpectedTokenException(JsonTokenType expected, JsonTokenType actual, string? path = null, Exception? inner = null)
    {
        ThrowJsonException($"Expected the JsonTokenType {expected}, but was {actual}.", path, inner);
    }

    #endregion JsonExceptions
}
#pragma warning disable RCS1138,CS1591
