using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Rustic.Common;

namespace Rustic.Memory.Common;

#if !NET5_0_OR_GREATER

// Source: https://source.dot.net/#System.Private.CoreLib/ArraySortHelper.cs
/// <summary>Provides the function <see cref="SpanSortHelper{T}.Sort"/> whith which a <see cref="Span{T}"/> can be sorted.</summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
internal static class SpanSortHelper<T>
{
    /// <summary>
    /// This is the threshold where Introspective sort switches to Insertion sort.
    /// Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    /// Large value types may benefit from a smaller number.
    /// </summary>
    public const int IntrosortSizeThreshold = 16;

    /// <summary>Sorts the elements of the span using the comparison.</summary>
    /// <param name="keys">The span to sort.</param>
    /// <param name="comparer">The comparison used to sort the span.</param>
    public static void Sort(Span<T> keys, Comparison<T> comparer)
    {
        Debug.Assert(comparer != null, "Check the values in the caller!");

        // Add a try block here to detect bogus comparisons
        try
        {
            IntrospectiveSort(keys, comparer);
        }
        catch (IndexOutOfRangeException)
        {
            ThrowHelper.ThrowArgumentException("Bad comparer.", nameof(comparer));
        }
        catch (Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException("The IComparer threw an exception.", e);
        }
    }

    internal static int InternalBinarySearch(T[] array, int index, int length, T value, IComparer<T> comparer)
    {
        Debug.Assert(array != null, "Check the values in the caller!");
        Debug.Assert(index >= 0 && length >= 0 && array.Length - index >= length, "Check the values in the caller!");

        int lo = index;
        int hi = index + length - 1;
        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);
            int order = comparer.Compare(array[i], value);

            if (order == 0)
            {
                return i;
            }

            if (order < 0)
            {
                lo = i + 1;
            }
            else
            {
                hi = i - 1;
            }
        }

        return ~lo;
    }

    private static void SwapIfGreater(Span<T> keys, Comparison<T> comparer, int i, int j)
    {
        Debug.Assert(i != j);

        if (comparer(keys[i], keys[j]) > 0)
        {
            T key = keys[i];
            keys[i] = keys[j];
            keys[j] = key;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(Span<T> a, int i, int j)
    {
        Debug.Assert(i != j);

        T t = a[i];
        a[i] = a[j];
        a[j] = t;
    }

    internal static void IntrospectiveSort(Span<T> keys, Comparison<T> comparer)
    {
        Debug.Assert(comparer != null);

        if (keys.Length > 1)
        {
            IntroSort(keys, 2 * (((uint)keys.Length).Log2() + 1), comparer);
        }
    }

    private static void IntroSort(Span<T> keys, int depthLimit, Comparison<T> comparer)
    {
        Debug.Assert(!keys.IsEmpty);
        Debug.Assert(depthLimit >= 0);
        Debug.Assert(comparer != null);

        int partitionSize = keys.Length;
        while (partitionSize > 1)
        {
            if (partitionSize <= IntrosortSizeThreshold)
            {

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, comparer, 0, 1);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, comparer, 0, 1);
                    SwapIfGreater(keys, comparer, 0, 2);
                    SwapIfGreater(keys, comparer, 1, 2);
                    return;
                }

                InsertionSort(keys[..partitionSize], comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys[..partitionSize], comparer);
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(keys[..partitionSize], comparer);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], depthLimit, comparer);
            partitionSize = p;
        }
    }

    private static int PickPivotAndPartition(Span<T> keys, Comparison<T> comparer)
    {
        Debug.Assert(keys.Length >= IntrosortSizeThreshold);
        Debug.Assert(comparer != null);

        int hi = keys.Length - 1;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = hi >> 1;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, comparer, 0, middle);  // swap the low with the mid point
        SwapIfGreater(keys, comparer, 0, hi);   // swap the low with the high
        SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

        T pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer(keys[++left], pivot) < 0) { }
            while (comparer(pivot, keys[--right]) < 0) { }

            if (left >= right)
            {
                break;
            }

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - 1)
        {
            Swap(keys, left, hi - 1);
        }
        return left;
    }

    private static void HeapSort(Span<T> keys, Comparison<T> comparer)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(!keys.IsEmpty);

        int n = keys.Length;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, i, n, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(keys, 0, i - 1);
            DownHeap(keys, 1, i - 1, comparer);
        }
    }

    private static void DownHeap(Span<T> keys, int i, int n, Comparison<T> comparer)
    {
        Debug.Assert(comparer != null);

        T d = keys[i - 1];
        while (i <= n >> 1)
        {
            int child = 2 * i;
            if (child < n && comparer(keys[child - 1], keys[child]) < 0)
            {
                child++;
            }

            if (comparer(d, keys[child - 1]) >= 0)
            {
                break;
            }

            keys[i - 1] = keys[child - 1];
            i = child;
        }

        keys[i - 1] = d;
    }

    private static void InsertionSort(Span<T> keys, Comparison<T> comparer)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            T t = keys[i + 1];

            int j = i;
            while (j >= 0 && comparer(t, keys[j]) < 0)
            {
                keys[j + 1] = keys[j];
                j--;
            }

            keys[j + 1] = t;
        }
    }
}

#endif

/// <summary>Provides the function <see cref="SpanSortHelper{T}.Sort"/> whith which a <see cref="Span{T}"/> can be sorted.</summary>
/// <typeparam name="K">The type of the keys.</typeparam>
/// <typeparam name="V">The type of the values.</typeparam>
internal static class SpanSortHelper<K, V>
{
    /// <summary>
    /// This is the threshold where Introspective sort switches to Insertion sort.
    /// Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    /// Large value types may benefit from a smaller number.
    /// </summary>
    public const int IntrosortSizeThreshold = 16;

    /// <summary>Sorts <paramref name="keys"/> using the comparison, mirroring changed to the <paramref name="values"/>.</summary>
    /// <param name="keys">The keys to sort.</param>
    /// <param name="values">The values to sort by the keys.</param>
    /// <param name="comparer">The comparer used to sort the keys.</param>
    public static void Sort(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
    {
        Debug.Assert(comparer != null, "Check the arguments in the caller!");
        Debug.Assert(keys.Length == values.Length, "Check the arguments in the caller!");

        // Add a try block here to detect bogus comparisons
        try
        {
            IntrospectiveSort(keys, values, comparer);
        }
        catch (IndexOutOfRangeException)
        {
            ThrowHelper.ThrowArgumentException("Bad comparer", nameof(comparer));
        }
        catch (Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException("The IComparer threw an exception.", e);
        }
    }

#nullable disable
    private static void SwapIfGreater(in Span<K> keys, in Span<V> values, Comparison<K> comparer, int i, int j)
    {
        Debug.Assert(i != j);

        if (comparer(keys[i], keys[j]) > 0)
        {
            Swap(keys, values, i, j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(in Span<K> keys, in Span<V> values, int i, int j)
    {
        Debug.Assert(i != j);

        K key = keys[i];
        keys[i] = keys[j];
        V value = values[i];
        values[i] = values[j];
        keys[j] = key;
        values[j] = value;
    }

    internal static void IntrospectiveSort(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(keys.Length == values.Length);

        if (keys.Length > 1)
        {
            IntroSort(keys, values, 2 * (((uint)keys.Length).Log2() + 1), comparer);
        }
    }

    private static void IntroSort(in Span<K> keys, in Span<V> values, int depthLimit, Comparison<K> comparer)
    {
        Debug.Assert(!keys.IsEmpty);
        Debug.Assert(depthLimit >= 0);
        Debug.Assert(comparer != null);

        int partitionSize = keys.Length;
        while (partitionSize > 1)
        {
            if (partitionSize <= IntrosortSizeThreshold)
            {

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, values, comparer, 0, 1);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, values, comparer, 0, 1);
                    SwapIfGreater(keys, values, comparer, 0, 2);
                    SwapIfGreater(keys, values, comparer, 1, 2);
                    return;
                }

                InsertionSort(keys[..partitionSize], values[..partitionSize], comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys[..partitionSize], values[..partitionSize], comparer);
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(keys[..partitionSize], values[..partitionSize], comparer);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], values[(p + 1)..partitionSize], depthLimit, comparer);
            partitionSize = p;
        }
    }

    private static int PickPivotAndPartition(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
    {
        Debug.Assert(keys.Length >= IntrosortSizeThreshold);
        Debug.Assert(comparer != null);

        int hi = keys.Length - 1;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = hi >> 1;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, values, comparer, 0, middle);  // swap the low with the mid point
        SwapIfGreater(keys, values, comparer, 0, hi);   // swap the low with the high
        SwapIfGreater(keys, values, comparer, middle, hi); // swap the middle with the high

        K pivot = keys[middle];
        Swap(keys, values, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer(keys[++left], pivot) < 0) { }
            while (comparer(pivot, keys[--right]) < 0) { }

            if (left >= right)
            {
                break;
            }

            Swap(keys, values, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - 1)
        {
            Swap(keys, values, left, hi - 1);
        }
        return left;
    }

    private static void HeapSort(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(!keys.IsEmpty);

        int n = keys.Length;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, values, i, n, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(keys, values, 0, i - 1);
            DownHeap(keys, values, 1, i - 1, comparer);
        }
    }

    private static void DownHeap(in Span<K> keys, in Span<V> values, int i, int n, Comparison<K> comparer)
    {
        Debug.Assert(comparer != null);

        K key = keys[i - 1];
        V value = values[i - 1];
        while (i <= n >> 1)
        {
            int child = 2 * i;
            if (child < n && comparer(keys[child - 1], keys[child]) < 0)
            {
                child++;
            }

            if (comparer(key, keys[child - 1]) >= 0)
            {
                break;
            }

            keys[i - 1] = keys[child - 1];
            values[i - 1] = values[child - 1];
            i = child;
        }

        keys[i - 1] = key;
        values[i - 1] = value;
    }

    private static void InsertionSort(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            K key = keys[i + 1];
            V value = values[i + 1];

            int j = i;
            while (j >= 0 && comparer(key, keys[j]) < 0)
            {
                keys[j + 1] = keys[j];
                values[j + 1] = values[j];
                j--;
            }

            keys[j + 1] = key;
            values[j + 1] = value;
        }
    }
#nullable restore
}
