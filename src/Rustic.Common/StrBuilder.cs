using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Rustic;

/// <summary>
///     This class represents a mutable string. Initially allocated in the stack, resorts to the <see cref="ArrayPool{T}.Shared"/> when growing.
/// </summary>
[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public ref struct StrBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    /// <summary>
    ///     Initializes a new <see cref="StrBuilder"/> with the specified buffer.
    /// </summary>
    /// <param name="initialBuffer">The stack-buffer used to build the string.</param>
    public StrBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    /// <summary>
    ///     Initializes a new <see cref="StrBuilder"/> with a array from the <see cref="ArrayPool{T}.Shared"/> with the specific size.
    /// </summary>
    /// <param name="initialCapacity">The minimum capacity of the pool-array.</param>
    public StrBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    /// <summary>
    ///     The length of the string.
    /// </summary>
    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    /// <summary>
    ///     The current capacity of the builder.
    /// </summary>
    public int Capacity => _chars.Length;

    /// <summary>
    ///     Ensures that the builder has at least the given capacity.
    /// </summary>
    /// <param name="capacity">The minimum capacity of the pool-array.</param>
    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_chars.Length)
        {
            Grow(capacity - _pos);
        }
    }

    /// <summary>
    ///     Get a pinnable reference to the builder.
    ///     Does not ensure there is a null char after <see cref="Length" />
    ///     This overload is pattern matched in the C# 7.3+ compiler so you can omit
    ///     the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    [Pure]
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    ///     Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length" /></param>
    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }

        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    ///     Gets the char at the given index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    public ref char this[int index]
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(index >= 0 && index < Length);
            return ref _chars[index];
        }
    }

    /// <summary>
    ///     Creates the string from the builder and disposes the instance.
    /// </summary>
    /// <returns>The string represented by the builder.</returns>
    public override string ToString()
    {
        var s = _chars.Slice(0, _pos).ToString();
        Dispose();
        return s;
    }

    private string GetDebuggerDisplay()
    {
        if (Length == 0)
        {
            return "Capacity = 0, Values = []";
        }

        StringBuilder sb = new(256);
        return sb
            .Append("Capacity = ").Append(_pos)
            .Append(", Values = [")
            .Append(_chars.Slice(0, _pos).ToString())
            .Append(']')
            .ToString();
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    ///     Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length" /></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }

        return _chars.Slice(0, _pos);
    }

    /// <summary>
    ///     Returns the span representing the current string.
    /// </summary>
    public ReadOnlySpan<char> AsSpan()
    {
        return _chars.Slice(0, _pos);
    }

    /// <summary>
    ///     Returns the span representing a portion of the current string.
    /// </summary>
    /// <param name="start">The zero-based index of the first char.</param>
    public ReadOnlySpan<char> AsSpan(int start)
    {
        return _chars.Slice(start, _pos - start);
    }

    /// <summary>
    ///     Returns the span representing a portion of the current string.
    /// </summary>
    /// <param name="start">The zero-based index of the first char.</param>
    /// <param name="length">The number of characters after the <paramref name="start"/>.</param>
    public ReadOnlySpan<char> AsSpan(int start, int length)
    {
        return _chars.Slice(start, length);
    }

    /// <inheritdoc cref="Span{T}.TryCopyTo"/>
    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars.Slice(0, _pos).TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }

        charsWritten = 0;
        Dispose();
        return false;
    }

    /// <summary>
    ///     Inserts a character a specific number of times at the <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index at which to insert the characters.</param>
    /// <param name="value">The value of the characters to insert.</param>
    /// <param name="count">The number of characters to insert.</param>
    public void Insert(int index, char value, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }

    /// <summary>
    ///     Inserts a character at the <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index at which to insert the character.</param>
    /// <param name="value">The value of the character to insert.</param>
    public void Insert(int index, char value)
    {
        if (_pos > _chars.Length - 1)
        {
            Grow(1);
        }

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + 1));
        _chars[index] = value;
        _pos += 1;
    }

    /// <summary>
    ///     Inserts a string at the <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index at which to insert the character.</param>
    /// <param name="value">The string to insert.</param>
    public void Insert(int index, string? value)
    {
        if (value.IsEmpty())
        {
            return;
        }

        var count = value!.Length;

        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        value
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
                .CopyTo(_chars.Slice(index));
        _pos += count;
    }

    /// <summary>
    ///     Appends the character to the end of the builder.
    /// </summary>
    /// <param name="value">The character.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
        var pos = _pos;
        if ((uint)pos < (uint)_chars.Length)
        {
            _chars[pos] = value;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(value);
        }
    }

    /// <summary>
    ///     Appends the string to the end of the builder.
    /// </summary>
    /// <param name="value">The string to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? value)
    {
        if (value.IsEmpty())
        {
            return;
        }

        var pos = _pos;
        if (value!.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = value[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(value);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
                .CopyTo(_chars.Slice(pos));
        _pos += s.Length;
    }

    /// <summary>
    ///     Appends a character a specific number of times at the end of the builder.
    /// </summary>
    /// <param name="value">The value of the characters to insert.</param>
    /// <param name="count">The number of characters to insert.</param>
    public void Append(char value, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var dst = _chars.Slice(_pos, count);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = value;
        }

        _pos += count;
    }

    /// <summary>
    ///     Appends a unmanaged char-array to the builder
    /// </summary>
    /// <param name="value">The pointer to the first character to append.</param>
    /// <param name="length">The number of characters after the <paramref name="value"/> pointer.</param>
    [CLSCompliant(false)]
    public unsafe void Append(char* value, int length)
    {
        if (value == (char*)0 || length == 0)
        {
            return;
        }

        var pos = _pos;
        if (pos > _chars.Length - length)
        {
            Grow(length);
        }

        var dst = _chars.Slice(_pos, length);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }

        _pos += length;
    }

    /// <summary>
    ///     Appends a span to the builder.
    /// </summary>
    /// <param name="value">The span to append.</param>
    public void Append(ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    /// <summary>
    ///     Appends a mutable span of a specific length to the builder.
    /// </summary>
    /// <param name="length">The length of the span to append.</param>
    /// <returns>The span at the end of the builder.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var origPos = _pos;
        if (origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    ///     Resize the internal buffer either by doubling current buffer size or
    ///     by adding <paramref name="additionalCapacityBeyondPos" /> to
    ///     <see cref="_pos" /> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    ///     Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
        var poolArray = ArrayPool<char>.Shared.Rent((_pos + additionalCapacityBeyondPos).Max(_chars.Length * 2));

        _chars.Slice(0, _pos).CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}
