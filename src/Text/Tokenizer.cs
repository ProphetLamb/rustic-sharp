using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HeaplessUtility.Common;

namespace HeaplessUtility.Text
{
    /// <summary>
    ///     Tokenizes a sequence, for use in lexer &amp; parser state machines.
    /// </summary>
    /// <typeparam name="T">The type of an element of the span.</typeparam>
    /// <remarks>
    ///     Provides utility to read the next element, to read until an element occurs, read a sequence of elements, and to read until a sequence of elements occurs,
    ///     in the modes TryRead, Peek, and Read:
    /// <br />
    ///     TryRead only consumes elements only if successful.
    /// <br />
    ///     Peek is <see cref="PureAttribute"/> and never consumes elements and informs about the position at which the condition was fulfilled.
    /// <br />
    ///     Read always consumes elements, if not successful elements will still be consumed. 
    /// </remarks>
    public ref struct Tokenizer<T>
    {
        private readonly ReadOnlySpan<T> _source;
        private readonly IEqualityComparer<T>? _comparer;
        private int _index;
        private int _tokenLength;

        /// <summary>
        ///     Initializes a new instance of <see cref="Tokenizer{T}"/> with the specified <paramref name="input"/> and <paramref name="comparer"/>. 
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
        public Tokenizer(ReadOnlySpan<T> input, IEqualityComparer<T>? comparer)
        {
            _source = input;
            _comparer = comparer;
            _index = 0;
            _tokenLength = 0;
        }

        /// <summary>
        ///     Defines the current position of the iterator.
        /// </summary>
        public int Head
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _index + _tokenLength;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                value.ValidateArg(value >= 0 && value < Length);

                if (value >= _index)
                {
                    _tokenLength = _index - value;
                }
                else
                {
                    _index = value;
                    _tokenLength = 0;
                }
            }
        }

        /// <inheritdoc cref="Span{T}.Length"/>
        public int Length => _source.Length;

        /// <summary>
        ///     Represents the token currently being built.
        /// </summary>
        public ReadOnlySpan<T> Token
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source.Slice(_index, _tokenLength);
        }

        /// <summary>
        ///     Represents the zero-based start-index of the current token inside the span.
        /// </summary>
        public int TokenIndex => _index;

        /// <summary>
        ///     Represents the length of the current token.
        /// </summary>
        public int TokenLength => _tokenLength;

        /// <summary>
        ///     Consumes one element.
        /// </summary>
        /// <returns><see langword="true"/> if the element could be consumed; otherwise, <see langword="false"/>.</returns>
        public bool Consume()
        {
            if (_index != _source.Length - _tokenLength)
            {
                _tokenLength += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Consumes a specified amount of elements.
        /// </summary>
        /// <returns><see langword="true"/> if the elements could be consumed; otherwise, <see langword="false"/>.</returns>
        public bool Consume(int amount)
        {
            amount.ValidateArg(amount >= 0);

            if (_index < Length - _tokenLength - amount)
            {
                _tokenLength += amount;
                return true;
            }

            // Move to end.
            _tokenLength = Length - _index;
            return false;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this = default;
        }

        /// <summary>
        ///     Returns &amp; clears the <see cref="Token"/>, and moves the iterator to the first element after the token.
        /// </summary>
        /// <returns>The span representing the token.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> FinalizeToken()
        {
            ReadOnlySpan<T> token = Token;
            _index += _tokenLength;
            _tokenLength = 0;
            return token;
        }

        /// <summary>
        ///     Resets teh builder to the initial state.
        /// </summary>
        public void Reset()
        {
            Head = 0;
        }

        /// <summary>
        ///     Advances the <see cref="Head"/> to a specific <paramref name="position"/>, always consumes elements.
        /// </summary>
        /// <param name="position">The target position</param>
        public void Advance(int position)
        {
            position.ValidateArg(position >= 0 && position < Length);
            position.ValidateArg(position >= Head);
            _tokenLength = position - Head;
        }

        /// <summary>
        ///     Advances the <see cref="Head"/> to a specific <paramref name="position"/>, consumes elements only if successful.
        /// </summary>
        /// <param name="position">The target position</param>
        /// <returns><see langword="true"/> if the elements could be consumed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        ///     If the target <paramref name="position"/> is behind the <see cref="Head"/> the state won't change.
        /// </remarks>
        public bool TryAdvance(int position)
        {
            if ((uint)position >= (uint)_source.Length)
            {
                return false;
            }

            int head = Head;
            if (head < position)
            {
                _tokenLength = position - head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the following elements are the sequence, consumes elements.
        /// </summary>
        /// <param name="expectedSequence">The sequence of elements.</param>
        /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadNextSequence(in ReadOnlySpan<T> expectedSequence)
        {
            bool success = PeekNextSequence(expectedSequence, out int head);
            Head = head;
            return success;
        }

        /// <summary>
        ///     Returns whether the following elements are the sequence, consumes elements only if the successful.
        /// </summary>
        /// <param name="expectedSequence">The sequence of elements.</param>
        /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNextSequence(in ReadOnlySpan<T> expectedSequence)
        {
            if (PeekNextSequence(expectedSequence, out int head))
            {
                Head = head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the following elements are the sequence.
        /// </summary>
        /// <param name="expectedSequence">The sequence of elements.</param>
        /// <param name="head">The position of the element after the sequence.</param>
        /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekNextSequence(in ReadOnlySpan<T> expectedSequence, out int head)
        {
            head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            int matched = 0;
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (EqualityComparer<T>.Default.Equals(_source[head], expectedSequence[matched]))
                        {
                            matched += 1;
                        }
                        else
                        {
                            return false;
                        }
                    } while ((matched != expectedSequence.Length) & (++head < _source.Length));

                    return matched == expectedSequence.Length;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                if (comparer.Equals(_source[head], expectedSequence[matched]))
                {
                    matched += 1;
                }
                else
                {
                    return false;
                }
            } while ((matched != expectedSequence.Length) & (++head < _source.Length));

            return matched == expectedSequence.Length;
        }

        /// <summary>
        ///     Consumes elements until the specified sequence of elements has been encountered.
        /// </summary>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadSequence(in ReadOnlySpan<T> expectedSequence)
        {
            bool success = PeekSequence(expectedSequence, out int head);
            Head = head;
            return success;
        }

        /// <summary>
        ///     Returns whether the remaining span contains the sequence, consumes the elements only if it does.
        /// </summary>
        /// <param name="expectedSequence">The sequence of elements.</param>
        /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadSequence(in ReadOnlySpan<T> expectedSequence)
        {
            if (PeekSequence(expectedSequence, out int head))
            {
                Head = head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the remaining span contains the sequence.
        /// </summary>
        /// <param name="expectedSequence">The sequence of elements.</param>
        /// <param name="head">The position of the element after the sequence.</param>
        /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekSequence(in ReadOnlySpan<T> expectedSequence, out int head)
        {
            head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            int matched = 0;
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(_source[head], expectedSequence[matched]))
                        {
                            matched = 0;
                        }
                        else
                        {
                            matched += 1;
                        }
                    } while ((matched != expectedSequence.Length) & (++head < _source.Length));

                    return matched == expectedSequence.Length;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                if (!comparer.Equals(_source[head], expectedSequence[matched]))
                {
                    matched = 0;
                }
                else
                {
                    matched += 1;
                }
            } while ((matched != expectedSequence.Length) & (++head < _source.Length));

            return matched == expectedSequence.Length;
        }

        #region Next & Until with TinySpan

        /// <summary>
        ///     Consumes one element, and returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadNext(in TinySpan<T> expected)
        {
            bool success = PeekNext(expected);
            Head += 1;
            return success;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements, and consumes the element only if it does.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNext(in TinySpan<T> expected)
        {
            if (PeekNext(expected))
            {
                Head += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekNext(in TinySpan<T> expected)
        {
            int head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            T current = _source[head];
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                for (var i = 0; i < expected.Length; i++)
                {
                    // If ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    if (!EqualityComparer<T>.Default.Equals(expected[i], current))
                    {
                        continue;
                    }

                    return true;
                }
            }
            else
            {
                for (var i = 0; i < expected.Length; i++)
                {
                    if (!comparer.Equals(expected[i], current))
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Consumes elements until one of the expected elements occur.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadUntil(in TinySpan<T> expected)
        {
            bool success = PeekUntil(expected, out int head);
            Head = head;
            return success;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadUntil(in TinySpan<T> expected)
        {
            if (PeekUntil(expected, out int head))
            {
                Head = head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <param name="head">The position at which the expected element occured.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekUntil(in TinySpan<T> expected, out int head)
        {
            head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            bool consumeNext = true;
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        T current = _source[head];
                        for (int i = 0; i < expected.Length; i++)
                        {
                            // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                            if (!EqualityComparer<T>.Default.Equals(expected[i], current))
                            {
                                continue;
                            }

                            consumeNext = false;
                            break;
                        }
                    } while (consumeNext & (++head < _source.Length));

                    // Consume next is true if and only if the iterator is at the end of the sequence and no match has been found.
                    return !consumeNext;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                T current = _source[head];
                for (int i = 0; i < expected.Length; i++)
                {
                    if (!comparer.Equals(expected[i], current))
                    {
                        continue;
                    }

                    consumeNext = false;
                    break;
                }
            } while (consumeNext & (++head < _source.Length));

            // Consume next is true if and only if the iterator is at the end of the sequence and no match has been found.
            return !consumeNext;
        }

        #endregion

        #region Next & Until with one arg

        /// <summary>
        ///     Consumes one element, and returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadNext(in T expected)
        {
            bool success = PeekNext(expected);
            Head += 1;
            return success;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements, and consumes the element only if it does.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNext(in T expected)
        {
            if (PeekNext(expected))
            {
                Head += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekNext(in T expected)
        {
            int head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            T current = _source[head];
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                return EqualityComparer<T>.Default.Equals(expected, current);
            }

            return comparer.Equals(expected, current);
        }

        /// <summary>
        ///     Consumes elements until one of the expected elements occur.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadUntil(in T expected)
        {
            bool success = PeekUntil(expected, out int head);
            Head = head;
            return success;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadUntil(in T expected)
        {
            if (PeekUntil(expected, out int head))
            {
                Head = head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <param name="head">The position at which the expected element occured.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekUntil(in T expected, out int head)
        {
            head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(expected, _source[head]))
                        {
                            continue;
                        }

                        return true;
                    } while (++head < _source.Length);

                    return false;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                if (!comparer.Equals(expected, _source[head]))
                {
                    continue;
                }

                return true;
            } while (++head < _source.Length);

            return false;
        }

        #endregion

        #region Next & Until with two args

        /// <summary>
        ///     Consumes one element, and returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadNext(in T expected0, in T expected1)
        {
            bool success = PeekNext(expected0, expected1);
            Head += 1;
            return success;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements, and consumes the element only if it does.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNext(in T expected0, in T expected1)
        {
            if (PeekNext(expected0, expected1))
            {
                Head += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the element matches one of the expected elements.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekNext(in T expected0, in T expected1)
        {
            int head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            T current = _source[head];
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                return EqualityComparer<T>.Default.Equals(expected0, current)
                    || EqualityComparer<T>.Default.Equals(expected1, current);
            }

            return comparer.Equals(expected0, current)
                || comparer.Equals(expected1, current);
        }

        /// <summary>
        ///     Consumes elements until one of the expected elements occur.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadUntil(in T expected0, in T expected1)
        {
            bool success = PeekUntil(expected0, expected1, out int head);
            Head = head;
            return success;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadUntil(in T expected0, in T expected1)
        {
            if (PeekUntil(expected0, expected1, out int head))
            {
                Head = head;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the remaining span contains one of the expected elements.
        /// </summary>
        /// <param name="expected0">The the expected element.</param>
        /// <param name="expected1">The the other expected element.</param>
        /// <param name="head">The position at which the expected element occured.</param>
        /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool PeekUntil(in T expected0, in T expected1, out int head)
        {
            head = Head;
            if (head == _source.Length)
            {
                return false;
            }

            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                if (typeof(T).IsValueType)
                {
                    do
                    {
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(expected0, _source[head])
                         && !EqualityComparer<T>.Default.Equals(expected1, _source[head]))
                        {
                            continue;
                        }

                        return true;
                    } while (++head < _source.Length);

                    return false;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            do
            {
                if (!comparer.Equals(expected0, _source[head])
                 && !comparer.Equals(expected1, _source[head]))
                {
                    continue;
                }

                return true;
            } while (++head < _source.Length);

            return false;
        }

        #endregion
    }
}