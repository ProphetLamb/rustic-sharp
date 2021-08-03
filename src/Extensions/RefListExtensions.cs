using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using HeaplessUtility.Pool;

namespace HeaplessUtility
{
    /// <summary>
    /// Extensions for <see cref="RefList{T}"/>
    /// </summary>
    public static class RefListExtensions
    {
        /// <summary>
        /// Determines whether two lists are equal by comparing the elements using IEquatable{T}.Equals(T).
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual<T>(this RefList<T> list, RefList<T> other)
            where T : IEquatable<T>
        {
            int count = list.Count;
            if (count != other.Count)
                return false;
            return list.RawStorage.Slice(0, count).SequenceEqual(other.RawStorage.Slice(0, count));
        }
        
        /// <summary>
        /// Determines the relative order of the lists being compared by comparing the elements using IComparable{T}.CompareTo(T).
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceCompareTo<T>(this RefList<T> list, RefList<T> other)
            where T : IComparable<T>
        {
            return list.RawStorage.Slice(0, list.Count).SequenceCompareTo(other.RawStorage.Slice(0, other.Count));
        }

        internal static int SequenceCompareHelper<T>(ref T first, int firstLength, ref T second, int secondLength, IComparer<T> comparer)
        {
            Debug.Assert(firstLength >= 0);
            Debug.Assert(secondLength >= 0);
 
            int minLength = firstLength;
            if (minLength > secondLength)
                minLength = secondLength;
            for (int i = 0; i < minLength; i++)
            {
                int result = comparer.Compare(Unsafe.Add(ref first, i), Unsafe.Add(ref second, i));
                if (result != 0)
                    return result;
            }
            return firstLength.CompareTo(secondLength);
        }
    }
}
