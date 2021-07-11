using System;
using System.Collections.Generic;

namespace HeaplessUtility
{
    public static class MemoryExtensions
    {
#region Split

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator)
        {
            return new(span, ParamsSpan.From(separator), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator), options, comparer);
        }
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1)
        {
            return new(span, ParamsSpan.From(separator0, separator1), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, comparer);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
        {
            return new(span, ParamsSpan.From(separators), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separators), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separators), options, comparer);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator)
        {
            return new(span, ParamsSpan.From(separator), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator), options, comparer);
        }
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1)
        {
            return new(span, ParamsSpan.From(separator0, separator1), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, comparer);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, ReadOnlySpan<T> separators)
        {
            return new(span, ParamsSpan.From(separators), SplitOptions.None, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, ReadOnlySpan<T> separators, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separators), options, null);
        }

        public static SpanSplitIterator<T> Split<T>(this Span<T> span, ReadOnlySpan<T> separators, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separators), options, comparer);
        }

#endregion

        /// <summary>
        ///     Determines whether two sequences are equal by comparing the elements.
        /// </summary>
        public static bool SequenceEquals<T>(this ParamsSpan<T> span, in ParamsSpan<T> other)
            where T : IEquatable<T>
        {
            // If we have 4 or less elements the == operator performs a sequence equals.
            // If we have more then 4 elements we compare the pointers of the internal span.
            if (span == other)
            {
                return true;
            }
            
            // Internal spans may not be
            if (span.Length != other.Length || span.Length <= 4)
            {
                return false;
            }

            // The internal spans are not the same, but are present, so the operation is always cheap.
            return span.ToSpan(true).SequenceEqual(other.ToSpan(true));
        }
    }
}