using System;

namespace HeaplessUtility
{
    /// <summary>
    ///     Defines how the results of the <see cref="SpanSplitIterator{T}"/> are transformed.
    /// </summary>
    [Flags]
    public enum SplitOptions : byte
    {
        /// <summary>Default behavior. No transformation.</summary>
        None = 0,

        /// <summary>Do not return zero-length segments. Instead return the next result, if any.</summary>
        RemoveEmptyEntries = 1 << 0,

        /// <summary>Do not return the leading segment before the first separator. Instead return the next result, if any.</summary>
        SkipLeadingSegment = 1 << 1,

        /// <summary>Do not return the last segment form the last separator to the end. Instead end.</summary>
        SkipTailingSegment = 1 << 2,

        /// <summary>All options.</summary>
        /// <remarks><c>RemoveEmptyEntries | SkipLeadingSegment | SkipTailingSegment</c></remarks>
        All = 0xff,
    }
}