using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory;

namespace Rustic.Text;

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
    private ReadOnlySpan<T> _source;
    private IEqualityComparer<T>? _comparer;
    private int _pos;
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
        _pos = 0;
        _tokenLength = 0;
    }

    public ReadOnlySpan<T> Raw => _source;

    public IEqualityComparer<T>? Comparer => _comparer;

    /// <summary>
    ///     Defines the current position of the iterator.
    /// </summary>
    public int Head
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pos + _tokenLength;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            value.ValidateArgRange(value >= 0 && value <= Length);

            if (value > _pos)
            {
                _tokenLength += value - _pos;
            }
            else
            {
                _pos = value;
                _tokenLength = 0;
            }
        }
    }

    /// <summary>Represents the zero-based start-index of the current token inside the span.</summary>
    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pos;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            value.ValidateArgRange(value >= 0 && value <= Length - Width);
            _pos = value;
        }
    }

    /// <summary>Represents the length of the current token.</summary>
    public int Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tokenLength;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            value.ValidateArgRange(value >= 0 && value <= Length - Position);
            _tokenLength = value;
        }
    }

    /// <inheritdoc cref="Span{T}.Length"/>
    public int Length => _source.Length;

    /// <summary>Represents the token currently being built.</summary>
    public ReadOnlySpan<T> Token => _source.Slice(_pos, _tokenLength);

    public ref readonly T Current => ref _source[_pos];

    /// <summary>Represents the token at an offset relative to the <see cref="Head"/>.</summary>
    public ref readonly T Offset(int offset)
    {
        return ref this[Head + offset];
    }

    public ref readonly T this[int index]
    {
        get
        {
            index.ValidateArgRange(index >= 0 && index < Length);
            return ref _source[index];
        }
    }

    /// <summary>Consumes one element.</summary>
    /// <returns><see langword="true"/> if the element could be consumed; otherwise, <see langword="false"/>.</returns>
    public bool Consume()
    {
        if (_pos != _source.Length - _tokenLength)
        {
            _tokenLength += 1;
            return true;
        }

        return false;
    }

    /// <summary>Consumes a specified amount of elements.</summary>
    /// <returns><see langword="true"/> if the elements could be consumed; otherwise, <see langword="false"/>.</returns>
    public bool Consume(int amount)
    {
        amount.ValidateArgRange(amount <= _tokenLength);

        if (_pos < Length - _tokenLength - amount)
        {
            _tokenLength += amount;
            return true;
        }

        // Move to end.
        _tokenLength = Length - _pos;
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
        var token = Token;
        _pos += _tokenLength;
        _tokenLength = 0;
        return token;
    }

    /// <summary>Discards the current token &amp; resets the iterator to the start of the token.</summary>
    public void Discard()
    {
        _tokenLength = 0;
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
        position.ValidateArgRange(position >= 0 && position < Length);
        position.ValidateArgRange(position >= Head);
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

    #region Sequence

    /// <summary>
    ///     Consumes elements until the specified sequence of elements has been encountered.
    /// </summary>
    /// <param name="expectedSequence">The expected sequence.</param>
    /// <param name="len">The length of the sequence.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Read<S>(in S expectedSequence, out int len)
        where S : IEnumerable<T>
    {
        bool success = Peek(expectedSequence, out int head, out len);
        Head = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the remaining span contains the sequence, consumes the elements only if it does.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <param name="len">The length of the sequence.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead<S>(in S expectedSequence, out int len)
        where S : IEnumerable<T>
    {
        if (Peek(expectedSequence, out int head, out len))
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
    /// <param name="len">The length of the sequence.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [Pure]
    public bool Peek<S>(in S expectedSequence, out int head, out int len)
        where S : IEnumerable<T>
    {
        head = Head;
        if (head == _source.Length)
        {
            len = 0;
            return false;
        }

        var matched = 0;
        var comparer = _comparer;

        do
        {
            len = 0;

            if (comparer is null)
            {
                if (typeof(T).IsValueType)
                {
                    foreach (T? match in expectedSequence)
                    {
                        if (head >= _source.Length)
                        {
                            matched = -1;
                            break;
                        }
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(_source[head], match))
                        {
                            matched = 0;
                        }
                        else
                        {
                            matched += 1;
                        }
                        head += 1;
                        len += 1;
                    }

                    if (matched == -1)
                    {
                        return false;
                    }
                    continue;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            foreach (T? match in expectedSequence)
            {
                if (head >= _source.Length)
                {
                    matched = -1;
                    break;
                }
                if (!comparer.Equals(_source[head], match))
                {
                    matched = 0;
                }
                else
                {
                    matched += 1;
                }
                head += 1;
                len += 1;
            }

            if (matched == -1)
            {
                return false;
            }
        } while (matched == 0 && head < _source.Length);

        return matched != 0;
    }

    /// <summary>
    ///     Returns whether the following elements are the sequence, consumes elements.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadNext<S>(in S expectedSequence)
        where S : IEnumerable<T>
    {
        bool success = PeekNext(expectedSequence, out int head);
        Head = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the following elements are the sequence, consumes elements only if the successful.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadNext<S>(in S expectedSequence)
        where S : IEnumerable<T>
    {
        if (PeekNext(expectedSequence, out int head))
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
    /// <typeparam name="S">The type of the sequence.</typeparam>
    [Pure]
    public bool PeekNext<S>(in S expectedSequence, out int head)
        where S : IEnumerable<T>
    {
        head = Head;
        if (head == _source.Length)
        {
            return false;
        }

        var comparer = _comparer;
        var matched = 0;

        if (comparer is null)
        {
            if (typeof(T).IsValueType)
            {
                foreach (T? match in expectedSequence)
                {
                    if (head >= _source.Length)
                    {
                        return false;
                    }
                    // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    if (EqualityComparer<T>.Default.Equals(_source[head], match))
                    {
                        matched += 1;
                        head += 1;
                    }
                    else
                    {
                        matched = 0;
                    }
                }

                return true;
            }

            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
            // so cache in a local rather than get EqualityComparer per loop iteration.
            comparer = EqualityComparer<T>.Default;
        }

        foreach (T? match in expectedSequence)
        {
            if (head >= _source.Length)
            {
                return false;
            }
            if (comparer.Equals(_source[head], match))
            {
                matched += 1;
                head += 1;
            }
            else
            {
                matched = 0;
            }
        }

        return true;
    }

    #endregion Sequence

    #region Span

    /// <summary>
    ///     Consumes elements until the specified sequence of elements has been encountered.
    /// </summary>
    /// <param name="expectedSequence">The expected sequence.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Read(in TinySpan<T> expectedSequence)
    {
        bool success = Peek(expectedSequence, out int head);
        Head = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the remaining span contains the sequence, consumes the elements only if it does.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead(in TinySpan<T> expectedSequence)
    {
        if (Peek(expectedSequence, out int head))
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
    public bool Peek(in TinySpan<T> expectedSequence, out int head)
    {
        head = Head;
        if (head == _source.Length)
        {
            return false;
        }

        var matched = 0;
        var comparer = _comparer;

        if (comparer is null)
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

    /// <summary>
    ///     Returns whether the following elements are the sequence, consumes elements.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadNext(in TinySpan<T> expectedSequence)
    {
        bool success = PeekNext(expectedSequence, out int head);
        Head = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the following elements are the sequence, consumes elements only if the successful.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadNext(in TinySpan<T> expectedSequence)
    {
        if (PeekNext(expectedSequence, out int head))
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
    public bool PeekNext(in TinySpan<T> expectedSequence, out int head)
    {
        head = Head;
        if (head == _source.Length)
        {
            return false;
        }

        var matched = 0;
        var comparer = _comparer;

        if (comparer is null)
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
        } while ((matched != expectedSequence.Length) && (++head < _source.Length));

        return matched == expectedSequence.Length;
    }

    #endregion Span

    #region Any Span

    /// <summary>
    ///     Consumes one element, and returns whether the element matches one of the expected elements.
    /// </summary>
    /// <param name="expected">The expected elements.</param>
    /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadAny(in TinySpan<T> expected)
    {
        bool success = PeekAny(expected);
        Head += 1;
        return success;
    }

    /// <summary>
    ///     Returns whether the element matches one of the expected elements, and consumes the element only if it does.
    /// </summary>
    /// <param name="expected">The expected elements.</param>
    /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadAny(in TinySpan<T> expected)
    {
        if (PeekAny(expected))
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
    public bool PeekAny(in TinySpan<T> expected)
    {
        int head = Head;
        if (head == _source.Length)
        {
            return false;
        }

        T? current = _source[head];
        var comparer = _comparer;

        if (comparer is null)
        {
            for (var i = 0; i < expected.Length; i += 1)
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
            for (var i = 0; i < expected.Length; i += 1)
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
    public bool ReadUntilAny(in TinySpan<T> expected)
    {
        bool success = PeekUntilAny(expected, out int head);
        Head = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.
    /// </summary>
    /// <param name="expected">The expected elements.</param>
    /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadUntilAny(in TinySpan<T> expected)
    {
        if (PeekUntilAny(expected, out int head))
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
    public bool PeekUntilAny(in TinySpan<T> expected, out int head)
    {
        head = Head;
        if (head == _source.Length)
        {
            return false;
        }

        var consumeNext = true;
        var comparer = _comparer;

        if (comparer is null)
        {
            if (typeof(T).IsValueType)
            {
                do
                {
                    T? current = _source[head];
                    for (var i = 0; i < expected.Length; i++)
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
            T? current = _source[head];
            for (var i = 0; i < expected.Length; i++)
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

    #endregion Any Span
}
