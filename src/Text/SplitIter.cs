using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Exceptions;

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
            if (separators.Length == 0)
            {
                ThrowHelper.ThrowArgumentException_CollectionEmpty(ExceptionArgument.separators);
            }

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

        /// <summary>
        ///     Returns a new <see cref="SplitIter{T}"/> enumerator with the same input in the initial state.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitIter<T> GetEnumerator()
        {
            if ((_index == 0) & (_segmentLength == 0))
            {
                return this;
            }

            return new SplitIter<T>(_source, _separators, _options, _comparer);
        }

        /// <summary>
        ///     Attempts to move the enumerator to the next segment.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator successfully moved to the next segment; otherwise, <see langword="false"/>.</returns>
        public bool MoveNext()
        {
            // Moves index to after the previous separator & zeros length
            SkipCurrent();

            if (_index == _source.Length)
            {
                return false;
            }

            int position = _index;
            bool consumeNext = true;
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        T current = _source[position];
                        for (int i = 0; i < _separators.Length; i++)
                        {
                            // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                            if (!EqualityComparer<T>.Default.Equals(_separators[i], current))
                            {
                                continue;
                            }

                            consumeNext = false;
                            break;
                        }
                    } while (consumeNext & (++position < _source.Length));

                    _segmentLength = position - _index;
                    return EnsureOptionsAllowMoveNext(position);
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                T current = _source[position];
                for (int i = 0; i < _separators.Length; i++)
                {
                    if (!comparer.Equals(_separators[i], current))
                    {
                        continue;
                    }

                    consumeNext = false;
                    break;
                }
            } while (consumeNext & (++position < _source.Length));

            _segmentLength = position - _index;
            return EnsureOptionsAllowMoveNext(position);
        }

        private bool EnsureOptionsAllowMoveNext(int position)
        {
            bool success;

            if ((_options & SplitOptions.RemoveEmptyEntries) == 0 | _segmentLength != 0 // RemoveEmpty & length == 0 => MoveNext
                || (_options & SplitOptions.SkipLeadingSegment) == 0 | _index != 0) // SkipLeading & _index == 0 => MoveNext
            {
                if ((_options & SplitOptions.SkipTailingSegment) == 0 | position != _source.Length) // SkipTailing & at end => false
                {
                    success = true; // None of the above => true
                }
                else
                {
                    SkipCurrent(); // Moves index to end & zeros length
                    success = false;
                }
            }
            else
            {
                success = MoveNext();
            }

            return success;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipCurrent()
        {
            _index += _segmentLength;
            _segmentLength = 0;
        }
    }
}