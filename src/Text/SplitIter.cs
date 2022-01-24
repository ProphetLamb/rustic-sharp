using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Common;
using HeaplessUtility.IO;

namespace HeaplessUtility.Text
{
    /// <summary>
    ///     Iterates a span in segments determined by separators.
    /// </summary>
    /// <typeparam name="T">The type of an element of the span.</typeparam>
    public ref struct SplitIter<T>
    {
        private readonly ReadOnlySpan<T> _source;
        private readonly TinySpan<T> _separators;
        private readonly SplitOptions _options;
        private readonly IEqualityComparer<T>? _comparer;
        private int _index;
        private int _segmentLength;

        internal SplitIter(ReadOnlySpan<T> values, in TinySpan<T> separators, SplitOptions options, IEqualityComparer<T>? comparer)
        {
            _source = values;
            _separators = separators;
            _options = options;
            _comparer = comparer;
            _index = 0;
            _segmentLength = 0;
        }

        /// <summary>
        ///     The segment of the current state of the enumerator.
        /// </summary>
        public ReadOnlySpan<T> Current
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source.Slice(_index, _segmentLength);
        }

        /// <summary>
        ///     Represents the zero-based start-index of the current segment inside the source span.
        /// </summary>
        public int SegmentIndex => _index;

        /// <summary>
        ///     Represents the length of the current segment.
        /// </summary>
        public int SegmentLength => _segmentLength;

        public bool IncludesSeparator => (_options & SplitOptions.IncludeSeparator) != 0 && _index + _segmentLength < _source.Length;

        /// <summary>
        ///     Returns a new <see cref="SplitIter{T}"/> enumerator with the same input in the initial state.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitIter<T> GetEnumerator()
        {
            return new SplitIter<T>(_source, _separators, _options, _comparer);
        }

        /// <summary>
        ///     Attempts to move the enumerator to the next segment.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator successfully moved to the next segment; otherwise, <see langword="false"/>.</returns>
        public bool MoveNext()
        {
            // Moves index to after the previous separator & zeros length
            int position = SkipCurrent();
            if (position >= _source.Length)
            {
                return false;
            }

            IEqualityComparer<T>? comparer = _comparer;
            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    return EnsureMoveNext(position, MoveNextValueType(position));
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }
            return EnsureMoveNext(position, MoveNextComparer(position, comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipCurrent()
        {
            var pos = _index + _segmentLength;

            if ((_options & SplitOptions.IncludeSeparator) == 0 && pos != 0)
            {
                pos += 1;
            }

            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int MoveNextValueType(int pos)
        {
            var len = _source.Length;
            for (; pos < len; pos += 1)
            {
                for (int seg = 0; seg < _separators.Length; seg += 1)
                {
                    if (EqualityComparer<T>.Default.Equals(_separators[seg], _source[pos]))
                    {
                        return pos;
                    }
                }
            }
            return len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int MoveNextComparer(int pos, IEqualityComparer<T> comparer)
        {
            var len = _source.Length;
            for (; pos < len; pos += 1)
            {
                for (int seg = 0; seg < _separators.Length; seg += 1)
                {
                    if (comparer.Equals(_separators[seg], _source[pos]))
                    {
                        return pos;
                    }
                }
            }
            return len;
        }

        private bool EnsureMoveNext(int start, int end)
        {
            var len = end - start;
            _index = start;
            _segmentLength = len;

            if ((_options & SplitOptions.IncludeSeparator) != 0 && end != _source.Length)
            {
                _segmentLength += 1;
            }

            if ((_options & SplitOptions.RemoveEmptyEntries) != 0 && len == 0)
            {
                return MoveNext();
            }
            return true;
        }

        /// <summary>
        ///     Resets the enumerator to the initial state.
        /// </summary>
        public void Reset()
        {
            _index = 0;
            _segmentLength = 0;
        }

        /// <summary>
        ///     Disposes the enumerator.
        /// </summary>
        public void Dispose()
        {
            this = default;
        }
    }
}