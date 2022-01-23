using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using HeaplessUtility.Exceptions;

namespace HeaplessUtility.Helpers
{
    internal static class SpanSortHelper<K, V>
    {
        // This is the threshold where Introspective sort switches to Insertion sort.
        // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        internal const int IntrosortSizeThreshold = 16;

        internal static void Sort(in Span<K> keys, in Span<V> values, Comparison<K> comparer)
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
                ThrowHelper.ThrowArgumentException_BadComparer(comparer);
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

                    InsertionSort(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);

                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(keys.Slice(p + 1, partitionSize - (p + 1)), values.Slice(p + 1, partitionSize - (p + 1)), depthLimit, comparer);
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
                    break;

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

                if (!(comparer(key, keys[child - 1]) < 0))
                    break;

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
}
