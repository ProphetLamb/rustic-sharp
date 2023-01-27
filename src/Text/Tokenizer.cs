using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
public ref struct Tokenizer<T> {
    private ReversibleIndexedSpan<T> _source;
    private IEqualityComparer<T>? _comparer;
    private int _pos;
    private int _tokenLength;

    /// <summary>
    ///     Initializes a new instance of <see cref="Tokenizer{T}"/> with the specified <paramref name="input"/> and <paramref name="comparer"/>.
    /// </summary>
    /// <param name="input">The input sequence.</param>
    /// <param name="comparer">The comparer used to determine whether two objects are equal.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tokenizer(ReversibleIndexedSpan<T> input, IEqualityComparer<T>? comparer) {
        _source = input;
        _comparer = comparer;
        _pos = 0;
        _tokenLength = 0;
    }

    /// <summary>The reference to the source buffer.</summary>
    public ReadOnlySpan<T> Raw {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _source.Span;
    }

    /// <summary>The comparer used to determine whether two elements are equal.</summary>
    public IEqualityComparer<T>? Comparer => _comparer;

    /// <summary>Defines the current cursor position of the iterator.</summary>
    public int CursorPosition {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pos + _tokenLength;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            ThrowHelper.ArgumentInRange(value, value >= 0 && value <= Length);

            if (value > _pos) {
                _tokenLength = value - _pos;
            } else {
                _pos = value;
                _tokenLength = 0;
            }
        }
    }

    /// <summary>Represents the zero-based start-index of the current token inside the span.</summary>
    public int Position {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pos;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            ThrowHelper.ArgumentInRange(value, value >= 0 && value <= Length - Width);
            _pos = value;
        }
    }

    /// <summary>Represents the length of the current token.</summary>
    public int Width {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tokenLength;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            ThrowHelper.ArgumentInRange(value, value >= 0 && value <= Length - Position);
            _tokenLength = value;
        }
    }

    /// <summary>Indicates whether the end of the source sequence is reached.</summary>
    public bool IsCursorEnd {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CursorPosition >= Length;
    }
    /// <summary>Indicates whether the cursor is at the beginning of the source sequence.</summary>
    public bool IsCursorStart {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CursorPosition == 0;
    }

    /// <inheritdoc cref="Span{T}.Length"/>
    public int Length {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _source.Length;
    }

    /// <summary>Represents the token currently being built from <see cref="Position"/> to see <see cref="CursorPosition"/>.</summary>
    public ReversibleIndexedSpan<T> Token {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _source.Slice(_pos, _tokenLength);
    }

    /// <summary>The reference to the current element at <see cref="Position"/>.</summary>
    public ref readonly T Current {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _source[_pos];
    }

    /// <summary>The reference to the next element after the cursor at index <see cref="CursorPosition"/>.</summary>
    public ref readonly T Cursor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _source[CursorPosition];
    }

    /// <summary>Allows access to an arbitrary element inside the source buffer.</summary>
    /// <param name="index"></param>
    public ref readonly T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            ThrowHelper.ArgumentInRange(index, index >= 0 && index < Length);
            return ref _source[index];
        }
    }

    /// <summary>Accesses the element at a specific offset from the <see cref="CursorPosition"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T GetAtCursor(int offset) {
        return ref this[CursorPosition + offset];
    }

    /// <summary>Attempts to obtain the element at a specific offset from the <see cref="CursorPosition"/>.</summary>
    /// <param name="offset">The offset from the cursor</param>
    /// <param name="value">The element at the offset, if any.</param>
    /// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetAtCursor(int offset, [NotNullWhen(true)] out T? value) {
        int index = CursorPosition + offset;
        if (index >= 0 && index <= Length) {
            value = _source[index]!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Accesses the element at a specific offset from the <see cref="Position"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T GetAtPosition(int offset) {
        return ref this[Position + offset];
    }

    /// <summary>Attempts to obtain the element at a specific offset from the <see cref="Position"/>.</summary>
    /// <param name="offset">The offset from the position</param>
    /// <param name="value">The element at the offset, if any.</param>
    /// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetAtPosition(int offset, [NotNullWhen(true)] out T? value) {
        int index = Position + offset;
        if (index >= 0 && index <= Length) {
            value = _source[index]!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Consumes one element.</summary>
    /// <returns><see langword="true"/> if the element could be consumed; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Consume() {
        if (_pos != _source.Length - _tokenLength) {
            _tokenLength += 1;
            return true;
        }

        return false;
    }

    /// <summary>Consumes a specified amount of elements. Moves the cursor by amount</summary>
    /// <returns><see langword="true"/> if the elements could be consumed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If `amount &lt; -Token.Length`: Cannot move the cursor to before the start of the current token. The token length cannot be negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Consume(int amount) {
        ThrowHelper.ArgumentInRange(amount, amount >= -_tokenLength, "Cannot move the cursor to before the start of the current token. The token length cannot be negative.");

        if (_pos <= Length - _tokenLength - amount) {
            _tokenLength += amount;
            return true;
        }

        // Move to end.
        _tokenLength = Length - _pos;
        return false;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() {
        this = default;
    }

    /// <summary>
    ///     Returns &amp; clears the <see cref="Token"/>, and moves the iterator to the first element after the token.
    /// </summary>
    /// <returns>The span representing the token.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReversibleIndexedSpan<T> FinalizeToken() {
        var token = Token;
        _pos += _tokenLength;
        _tokenLength = 0;
        return token;
    }

    /// <summary>Discards the current token &amp; resets the iterator to the start of the token.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Discard() {
        _tokenLength = 0;
    }

    /// <summary>
    ///     Resets teh builder to the initial state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() {
        CursorPosition = 0;
    }

    /// <summary>
    ///     Advances the <see cref="CursorPosition"/> to a specific <paramref name="position"/>, always consumes elements.
    /// </summary>
    /// <param name="position">The target position</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int position) {
        ThrowHelper.ArgumentInRange(position, position >= 0 && position < Length);
        ThrowHelper.ArgumentInRange(position, position >= CursorPosition);
        _tokenLength = position - CursorPosition;
    }

    /// <summary>
    ///     Advances the <see cref="CursorPosition"/> to a specific <paramref name="position"/>, consumes elements only if successful.
    /// </summary>
    /// <param name="position">The target position</param>
    /// <returns><see langword="true"/> if the elements could be consumed; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    ///     If the target <paramref name="position"/> is behind the <see cref="CursorPosition"/> the state won't change.
    /// </remarks>
    public bool TryAdvance(int position) {
        if ((uint)position >= (uint)_source.Length) {
            return false;
        }

        var head = CursorPosition;
        if (head < position) {
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
        where S : IEnumerable<T> {
        var success = Peek(expectedSequence, out var head, out len);
        CursorPosition = head;
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
        where S : IEnumerable<T> {
        if (Peek(expectedSequence, out var head, out len)) {
            CursorPosition = head;
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
        where S : IEnumerable<T> {
        head = CursorPosition;
        if (head == _source.Length) {
            len = 0;
            return false;
        }

        var matched = 0;
        var comparer = _comparer;

        do {
            len = 0;

            if (comparer is null) {
                if (typeof(T).IsValueType) {
                    foreach (var match in expectedSequence) {
                        if (head >= _source.Length) {
                            matched = -1;
                            break;
                        }
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(_source[head], match)) {
                            matched = 0;
                        } else {
                            matched += 1;
                        }
                        head += 1;
                        len += 1;
                    }

                    if (matched == -1) {
                        return false;
                    }
                    continue;
                }

                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
                // so cache in a local rather than get EqualityComparer per loop iteration.
                comparer = EqualityComparer<T>.Default;
            }

            foreach (var match in expectedSequence) {
                if (head >= _source.Length) {
                    matched = -1;
                    break;
                }
                if (!comparer.Equals(_source[head], match)) {
                    matched = 0;
                } else {
                    matched += 1;
                }
                head += 1;
                len += 1;
            }

            if (matched == -1) {
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
        where S : IEnumerable<T> {
        var success = PeekNext(expectedSequence, out var head);
        CursorPosition = head;
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
        where S : IEnumerable<T> {
        if (PeekNext(expectedSequence, out var head)) {
            CursorPosition = head;
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
        where S : IEnumerable<T> {
        head = CursorPosition;
        if (head == _source.Length) {
            return false;
        }

        var comparer = _comparer;
        var matched = 0;

        if (comparer is null) {
            if (typeof(T).IsValueType) {
                foreach (var match in expectedSequence) {
                    if (head >= _source.Length) {
                        return false;
                    }
                    // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    if (EqualityComparer<T>.Default.Equals(_source[head], match)) {
                        matched += 1;
                        head += 1;
                    } else {
                        matched = 0;
                    }
                }

                return head - CursorPosition == matched;
            }

            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
            // so cache in a local rather than get EqualityComparer per loop iteration.
            comparer = EqualityComparer<T>.Default;
        }

        foreach (var match in expectedSequence) {
            if (head >= _source.Length) {
                return false;
            }
            if (comparer.Equals(_source[head], match)) {
                matched += 1;
                head += 1;
            } else {
                matched = 0;
            }
        }

        return head - CursorPosition == matched;
    }

    #endregion Sequence

    #region Span

    /// <summary>
    ///     Consumes elements until the specified sequence of elements has been encountered.
    /// </summary>
    /// <param name="expectedSequence">The expected sequence.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Read(in TinyRoSpan<T> expectedSequence) {
        var success = Peek(expectedSequence, out var head);
        CursorPosition = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the remaining span contains the sequence, consumes the elements only if it does.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the remaining elements contain the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead(in TinyRoSpan<T> expectedSequence) {
        if (Peek(expectedSequence, out var head)) {
            CursorPosition = head;
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
    public bool Peek(in TinyRoSpan<T> expectedSequence, out int head) {
        head = CursorPosition;
        if (head == _source.Length) {
            return false;
        }

        var matched = 0;
        var comparer = _comparer;

        if (comparer is null) {
            if (typeof(T).IsValueType) {
                do {
                    // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    if (!EqualityComparer<T>.Default.Equals(_source[head], expectedSequence[matched])) {
                        matched = 0;
                    } else {
                        matched += 1;
                    }
                } while ((matched != expectedSequence.Length) & (++head < _source.Length));

                return matched == expectedSequence.Length;
            }

            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
            // so cache in a local rather than get EqualityComparer per loop iteration.
            comparer = EqualityComparer<T>.Default;
        }

        do {
            if (!comparer.Equals(_source[head], expectedSequence[matched])) {
                matched = 0;
            } else {
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
    public bool ReadNext(in TinyRoSpan<T> expectedSequence) {
        var success = PeekNext(expectedSequence, out var head);
        CursorPosition = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the following elements are the sequence, consumes elements only if the successful.
    /// </summary>
    /// <param name="expectedSequence">The sequence of elements.</param>
    /// <returns><see langword="true"/> if the following elements are equal to the sequence of elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadNext(in TinyRoSpan<T> expectedSequence) {
        if (PeekNext(expectedSequence, out var head)) {
            CursorPosition = head;
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
    public bool PeekNext(in TinyRoSpan<T> expectedSequence, out int head) {
        head = CursorPosition;
        if (head == _source.Length) {
            return false;
        }

        int matched = 0; // the number of elements in the sequence matched by the toke
        IEqualityComparer<T>? comparer = _comparer;

        if (comparer is null) {
            if (typeof(T).IsValueType) {
                do {
                    // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    if (EqualityComparer<T>.Default.Equals(_source[head], expectedSequence[matched])) {
                        matched += 1;
                    } else {
                        return false;
                    }
                } while ((matched != expectedSequence.Length) & (++head < _source.Length));

                return matched == expectedSequence.Length;
            }

            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize (https://github.com/dotnet/runtime/issues/10050),
            // so cache in a local rather than get EqualityComparer per loop iteration.
            comparer = EqualityComparer<T>.Default;
        }

        do {
            if (comparer.Equals(_source[head], expectedSequence[matched])) {
                matched += 1;
            } else {
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
    public bool ReadAny(in TinyRoSpan<T> expected) {
        var success = PeekAny(expected);
        CursorPosition += 1;
        return success;
    }

    /// <summary>
    ///     Returns whether the element matches one of the expected elements, and consumes the element only if it does.
    /// </summary>
    /// <param name="expected">The expected elements.</param>
    /// <returns><see langword="true"/> if the element is as expected; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadAny(in TinyRoSpan<T> expected) {
        if (PeekAny(expected)) {
            CursorPosition += 1;
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
    public bool PeekAny(in TinyRoSpan<T> expected) {
        var head = CursorPosition;
        if (head == _source.Length) {
            return false;
        }

        var current = _source[head];
        var comparer = _comparer;

        if (comparer is null) {
            for (var i = 0; i < expected.Length; i += 1) {
                // If ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                if (!EqualityComparer<T>.Default.Equals(expected[i], current)) {
                    continue;
                }

                return true;
            }
        } else {
            for (var i = 0; i < expected.Length; i += 1) {
                if (!comparer.Equals(expected[i], current)) {
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
    public bool ReadUntilAny(in TinyRoSpan<T> expected) {
        var success = PeekUntilAny(expected, out var head);
        CursorPosition = head;
        return success;
    }

    /// <summary>
    ///     Returns whether the remaining span contains one of the expected elements, and consumes the elements only if it does.
    /// </summary>
    /// <param name="expected">The expected elements.</param>
    /// <returns><see langword="true"/> if one or more of the expected elements occur in the remaining elements; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadUntilAny(in TinyRoSpan<T> expected) {
        if (PeekUntilAny(expected, out var head)) {
            CursorPosition = head;
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
    public bool PeekUntilAny(in TinyRoSpan<T> expected, out int head) {
        head = CursorPosition;
        if (head == _source.Length) {
            return false;
        }

        var consumeNext = true;
        var comparer = _comparer;

        if (comparer is null) {
            if (typeof(T).IsValueType) {
                do {
                    var current = _source[head];
                    for (var i = 0; i < expected.Length; i++) {
                        // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                        if (!EqualityComparer<T>.Default.Equals(expected[i], current)) {
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

        do {
            var current = _source[head];
            for (var i = 0; i < expected.Length; i++) {
                if (!comparer.Equals(expected[i], current)) {
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


/// <summary>Tokenizer specific <see cref="StrBuilder"/> extension methods</summary>
public static class StrBuilderExtensions {
    /// <inheritdoc cref="StrBuilder.Append(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(ref this StrBuilder builder, ReversibleIndexedSpan<char> span) {
        if (!span.IsEmpty) {
            Span<char> dst = builder.AppendSpan(span.Length);
            span.TryCopyTo(dst);
        }
    }
}
