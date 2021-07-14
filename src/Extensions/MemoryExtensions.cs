using System;
using System.Collections.Generic;
using HeaplessUtility.Exceptions;
using HeaplessUtility.Helpers;

namespace HeaplessUtility
{
    /// <summary>
    ///     Extensions for <see cref="Span{T}"/>, <see cref="ReadOnlySpan{T}"/>, <see cref="ParamsSpan{T}"/>, and <see cref="ParamsArray"/>.
    /// </summary>
    public static class MemoryExtensions
    {
#region Split

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator)
        {
            return new(span, ParamsSpan.From(separator), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator), options, null);
        }
        
        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator), options, comparer);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1)
        {
            return new(span, ParamsSpan.From(separator0, separator1), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, in T separator0, in T separator1, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, comparer);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
        {
            return new(span, ParamsSpan.From(separators), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separators), options, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separators), options, comparer);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator)
        {
            return new(span, ParamsSpan.From(separator), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator), options, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separator"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator">The separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator), options, comparer);
        }
        
        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1)
        {
            return new(span, ParamsSpan.From(separator0, separator1), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the separators.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separator0">The fist separator by which to split the <paramref name="span"/>.</param>
        /// <param name="separator1">The second separator by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, in T separator0, in T separator1, SplitOptions options, IEqualityComparer<T> comparer)
        {
            return new(span, ParamsSpan.From(separator0, separator1), options, comparer);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, ReadOnlySpan<T> separators)
        {
            return new(span, ParamsSpan.From(separators), SplitOptions.None, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
        public static SpanSplitIterator<T> Split<T>(this Span<T> span, ReadOnlySpan<T> separators, SplitOptions options)
        {
            return new(span, ParamsSpan.From(separators), options, null);
        }

        /// <summary>
        ///     Splits the <paramref name="span"/> span at the positions defined by the <paramref name="separators"/>.
        /// </summary>
        /// <param name="span">The span span.</param>
        /// <param name="separators">The separators by which to split the <paramref name="span"/>.</param>
        /// <param name="options">The options defining how to return the segments.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        /// <typeparam name="T">The type of the elements of the <paramref name="span"/>.</typeparam>
        /// <returns>The iterator splitting the <paramref name="span"/> span with the specified parameters.</returns>
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

#if !NET50_OR_GREATER
        
        /// <summary>
        ///     Sorts the elements in the entire <see cref="Span{T}" /> using the <see cref="IComparable{T}" /> implementation of each element of the <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="span">>The <see cref="Span{T}"/> to sort.</param>
        /// <typeparam name="T">The type of the elements of the span.</typeparam>
        public static void Sort<T>(this Span<T> span)
        {
            Sort(span, Comparer<T>.Default);
        }

        /// <summary>
        ///     Sorts the elements in the entire <see cref="Span{T}" /> using the <typeparamref name="TComparer" />.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
        /// <param name="comparer"></param>
        /// <typeparam name="T">The type of the elements of the span.</typeparam>
        /// <typeparam name="TComparer">The type of the comparer to use to compare elements.</typeparam>
        public static void Sort<T, TComparer>(this Span<T> span, TComparer comparer)
            where TComparer : IComparer<T>
        {
            if (span.Length > 1)
            {
                SpanSortHelper<T>.Sort(span, comparer.Compare);
            }
        }

        /// <summary>
        ///     Sorts the elements in the entire <see cref="Span{T}" /> using the specified <see cref="Comparison{T}" />.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
        /// <typeparam name="T">The type of the elements of the span.</typeparam>
        public static void Sort<T>(this Span<T> span, Comparison<T>? comparison)
        {
            if (comparison == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
            }
            if (span.Length > 1)
            {
                SpanSortHelper<T>.Sort(span, comparison);
            }
        }
#endif
    }
}