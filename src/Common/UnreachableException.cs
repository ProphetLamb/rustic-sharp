// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !NET7_0_OR_GREATER
#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace System.Diagnostics;
#pragma warning restore IDE0130

/// <summary>
/// Exception thrown when the program executes an instruction that was thought to be unreachable.
/// </summary>
public sealed class UnreachableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="System.Diagnostics.UnreachableException"/> class with the default error message.
    /// </summary>
    public UnreachableException()
        : base("The program executed an instruction that was thought to be unreachable.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="System.Diagnostics.UnreachableException"/>
    /// class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnreachableException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="System.Diagnostics.UnreachableException"/>
    /// class with a specified error message and a reference to the inner exception that is the cause of
    /// this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnreachableException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Throws a new instance of <see cref="UnreachableException"/>.
    /// </summary>
    /// <exception cref="UnreachableException"></exception>
    [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Throw() {
        throw new UnreachableException();
    }

    /// <inheritdoc cref="Throw"/>
    [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Throw<T>() {
        throw new UnreachableException();
    }
}
#endif