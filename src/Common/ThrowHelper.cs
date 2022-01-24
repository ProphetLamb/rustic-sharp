using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Validation;

namespace HeaplessUtility.Common
{
    internal static class ThrowHelper
    {
        #region ArgumentExceptions

        [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentException(string message, string? name = null, Exception? inner = null)
        {
            throw new ArgumentException(message, name, inner);
        }

        [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullException(string name, string? message = null)
        {
            throw new ArgumentNullException(name, message);
        }

        [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentOutOfRangeException(string name, object actual, string? message = null)
        {
            throw new ArgumentOutOfRangeException(name, actual, message);
        }

        #endregion

        #region InvalidOperation

        [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException(string? message = null, Exception? ex = null)
        {
            throw new InvalidOperationException(message, ex);
        }

        [DoesNotReturn, DebuggerStepThrough, MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowNotSupportedException(string? message = null, Exception? ex = null)
        {
            throw new NotSupportedException(message, ex);
        }

        #endregion

        #region Validations

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateArg<T>(this T value, [DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("value")] string name = null!, [CallerArgumentExpression("condition")] string message = null!)
            where T : notnull
        {
            if (!condition)
            {
                ThrowArgumentException($"The argument \"{value}\" does not fulfill \"{message}\".", name);
            }
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateArgRange<T>(this T value, [DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("value")] string name = null!, [CallerArgumentExpression("condition")] string message = null!)
            where T : notnull
        {
            if (!condition)
            {
                ThrowArgumentOutOfRangeException(name, value, $"The argument does not fulfill \"{message}\".");
            }
        }

        [DebuggerStepThrough]
        public static void ValidateArgNotNull([ValidatedNotNull] this object? self, [CallerMemberName] string name = null!)
        {
            if (self is null)
            {
                ThrowArgumentNullException(name);
            }
        }

        #endregion
    }
}