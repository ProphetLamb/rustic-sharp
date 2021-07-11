using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        
        [DoesNotReturn]
        public static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

        [DoesNotReturn]
        public static void ThrowInvalidOperationException_ObjectDisposed()
        {
            throw new InvalidOperationException("The operation cannot be performed on an object is disposed.");
        }
        
        [DoesNotReturn]
        public static void ThrowInvalidOperationException_ObjectNotInitialized()
        {
            throw new InvalidOperationException("The operation cannot be performed before the object is initialized.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "The index is outside of the range of the array.");
        }

        public static void ThrowIfObjectDisposed([DoesNotReturnIf(true)] bool disposed)
        {
            if (disposed)
            {
                ThrowInvalidOperationException_ObjectDisposed();
            }
        }

        public static void ThrowIfObjectNotInitialized([DoesNotReturnIf(true)] bool notInitialized)
        {
            if (notInitialized)
            {
                ThrowInvalidOperationException_ObjectNotInitialized();
            }
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_LessEqualZero(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "The value cannot be less then or equal to zero.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_LessZero(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "The value cannot be less then zero.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument argument, object? value, object? maximum)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), $"The value {value} cannot be greater then or equal to the maximum {maximum}.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_UnderMin(ExceptionArgument argument, object? value, object? minimum)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), $"The value {value} cannot be less then the minimum {minimum}.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentException_ArrayCapacityOverMax(ExceptionArgument argument, int index)
        {
            throw new ArgumentException($"The index {index} is outside the of the array ", GetArgumentName(argument));
        }

        [DoesNotReturn]
        public static void ThrowArgumentException_CollectionEmpty(ExceptionArgument argument)
        {
            throw new ArgumentException("The collection cannot be empty.", GetArgumentName(argument));
        }
    }

    internal enum ExceptionArgument
    {
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
        position
    }
}