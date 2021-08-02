using System;
using System.Collections.Generic;

#if NETSTANDARD2_1
using System.Diagnostics.CodeAnalysis;
#endif

namespace HeaplessUtility.Exceptions
{
    internal static class ThrowHelper
    {
        private static readonly Dictionary<ExceptionArgument, string> s_argumentNameMap = new();

        private static string GetArgumentName(in ExceptionArgument argument)
        {
            if (s_argumentNameMap.TryGetValue(argument, out string? name))
            {
                return name!;
            }

            name = Enum.GetName(typeof(ExceptionArgument), argument);
            s_argumentNameMap.Add(argument, name!);

            return name!;
        }
        
#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowInvalidOperationException_ObjectDisposed()
        {
            throw new InvalidOperationException("The operation cannot be performed on an object is disposed.");
        }
        
#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowInvalidOperationException_ObjectNotInitialized()
        {
            throw new InvalidOperationException("The operation cannot be performed before the object is initialized.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument argument, int index)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), $"The index {index} is outside of the range of the array.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument argument)
        {
            throw new ArgumentException("The array has insufficient capacity.", GetArgumentName(argument));
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentException_CollectionEmpty(ExceptionArgument argument)
        {
            throw new ArgumentException("The collection cannot be empty.", GetArgumentName(argument));
        }

        public static void ThrowIfObjectDisposed(
#if NETSTANDARD2_1
            [DoesNotReturnIf(true)]
#endif
            bool disposed)
        {
            if (disposed)
            {
                ThrowInvalidOperationException_ObjectDisposed();
            }
        }

        public static void ThrowIfObjectNotInitialized(
#if NETSTANDARD2_1
            [DoesNotReturnIf(true)]
#endif
            bool notInitialized)
        {
            if (notInitialized)
            {
                ThrowInvalidOperationException_ObjectNotInitialized();
            }
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentOutOfRangeException_LessEqualZero(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "The value cannot be less then or equal to zero.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "The value cannot be less then zero.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument argument, object? value, object? maximum)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), $"The value {value} cannot be greater then or equal to the maximum {maximum}.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentOutOfRangeException_UnderMin(ExceptionArgument argument, object? value, object? minimum)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), $"The value {value} cannot be less then the minimum {minimum}.");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentException_BadComparer(object comparer)
        {
            throw new ArgumentException($"Bad comparer {comparer}");
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowInvalidOperationException(string message, Exception exception)
        {
            throw new InvalidOperationException(message, exception);
        }

#if NETSTANDARD2_1
        [DoesNotReturn]
#endif
        public static void ThrowArgumentNullException(ExceptionArgument comparison)
        {
            throw new ArgumentNullException(GetArgumentName(comparison), "The value cannot be null or default.");
        }

        public static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, string? message)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), message);
        }
    }

    internal enum ExceptionArgument
    {
// ReSharper disable InconsistentNaming
        index,
        arrayIndex,
        array,
        count,
        initialCapacity,
        startIndex,
        length,
        start,
        separators,
        value,
        amount,
        position,
        comparison,
        range
// ReSharper restore InconsistentNaming
    }
}