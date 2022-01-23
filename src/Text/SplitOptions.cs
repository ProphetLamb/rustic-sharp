using System;

namespace HeaplessUtility.Text
{
    /// <summary>
    ///     Defines how the results of the <see cref="SplitIter{T}"/> are transformed.
    /// </summary>
    [Flags]
    public enum SplitOptions : byte
    {
        /// <summary>Default behavior. No transformation.</summary>
        None = 0,

        /// <summary>Do not return zero-length segments. Instead return the next result, if any.</summary>
        RemoveEmptyEntries = 1 << 0,

        /// <summary>All options.</summary>
        /// <remarks><c>RemoveEmptyEntries | SkipLeadingSegment | SkipTailingSegment</c></remarks>
        All = 0xff,
    }
}