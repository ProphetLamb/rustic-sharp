using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using Rustic.Memory;

namespace Rustic.Memory;

/// <summary>
///     Reusable <see cref="IBufferWriter{T}"/> intended for use as a thread-static singleton.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("Length: {Count}")]
[DebuggerTypeProxy(typeof(PoolBufWriterDebuggerView<>))]
public class BufWriter<T> :
    IBufferWriter<T>,
    IVector<T>,
    IDisposable
{
    /// <summary>
    /// The internal storage.
    /// </summary>
    protected T[]? Buffer;
    private int _index;

    /// <summary>
    ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
    /// </summary>
    public BufWriter()
    {
        Buffer = null;
        _index = 0;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
    /// </summary>
    /// <param name="initialCapacity">The minimum capacity of the writer.</param>
    public BufWriter(int initialCapacity)
    {
        ThrowHelper.ArgumentInRange(initialCapacity, initialCapacity >= 0);
        if (initialCapacity != 0)
        {
            Buffer = new T[initialCapacity];
        }
        _index = 0;
    }

    /// <summary>
    ///     Returns the underlying storage of the list.
    /// </summary>
    internal Span<T> RawStorage => Buffer;

    /// <inheritdoc cref="List{T}.Count" />
    public int Length
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= Capacity);
            _index = value;
        }
    }

    /// <inheritdoc />
    bool ICollection.IsSynchronized => false;

    /// <inheritdoc />
    object ICollection.SyncRoot => null!;

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    public int Capacity => (Buffer?.Length) ?? 0;

    /// <inheritdoc />
    public bool IsEmpty => 0u >= (uint)Length;

    /// <inheritdoc />
    public bool IsDefault => Buffer is null;

    /// <inheritdoc />
    public int Count => Length;


    /// <inheritdoc />
    public ref T this[int index]
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ThrowHelper.ArgumentInRange(index, index >= 0 && index < Length);
            ThrowHelper.ArgumentIs(this, Buffer is not null);
            return ref Buffer[index];
        }
    }

    T IReadOnlyList<T>.this[int index] => this[index];

    ref readonly T IReadOnlyVector<T>.this[int index] => ref this[index];

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc />
    public ref T this[Index index] => ref this[index.GetOffset(Length)];

    /// <inheritdoc />
    ref readonly T IReadOnlyVector<T>.this[Index index] => ref this[index];
#endif

    /// <inheritdoc />
    T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        ThrowHelper.ArgumentInRange(count, count >= 0);
        ThrowHelper.ArgumentInRange(count, Length <= Capacity - count);
        _index += count;
    }

    /// <inheritdoc />
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        if (_index > Capacity - sizeHint)
        {
            Grow(sizeHint);
        }

        Debug.Assert(Capacity > _index);
        return Buffer.AsMemory(_index);
    }

    /// <inheritdoc />
    public Span<T> GetSpan(int sizeHint = 0)
    {
        if (_index > Capacity - sizeHint)
        {
            Grow(sizeHint);
        }

        Debug.Assert(Capacity > _index);
        return Buffer.AsSpan(_index);
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        var pos = _index;
        if (pos > Capacity - 1)
        {
            Grow(1);
        }

        Buffer![pos] = item;
        _index = pos + 1;
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (Buffer is not null)
        {
            Array.Clear(Buffer, 0, _index);
        }

        _index = 0;
    }

    /// <inheritdoc />
    bool ICollection<T>.Contains(T item) => this.IndexOf(in item) >= 0;

    int IList<T>.IndexOf(T item) => this.IndexOf(in item);

    /// <inheritdoc />
    [CLSCompliant(false)]
    public void Insert(int index, in T item)
    {
        ThrowHelper.ArgumentInRange(index, index >= 0 && index <= Count);

        var pos = _index;
        if (pos > Capacity - 1)
        {
            Grow(1);
        }
        Debug.Assert(Buffer is not null);

        var remaining = pos - index;

        if (remaining != 0)
        {
            Array.Copy(Buffer, index, Buffer!, index + 1, remaining);
        }
        else
        {
            Buffer![pos] = item;
        }

        _index = pos + 1;
    }

    void IList<T>.Insert(int index, T item) => Insert(index, in item);

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        ThrowHelper.ArgumentInRange(index, index >= 0 && index < Count);
        Debug.Assert(Buffer is not null);

        var pos = _index - 1;
        var remaining = pos - index;

        if (remaining != 0)
        {
            Array.Copy(Buffer!, index + 1, Buffer!, index, remaining);
        }

        Buffer![pos] = default!;
        _index = pos;
    }

    /// <inheritdoc />
    bool ICollection<T>.Remove(T item) => this.Remove(in item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

    /// <summary>
    /// Returns the <see cref="Span{T}"/> representing the written / requested portion of the buffer.
    /// <see cref="Reset"/>s the buffer.
    /// </summary>
    /// <param name="array">The internal array</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> ToSpan(out T[]? array)
    {
        if (Buffer is null)
        {
            array = null;
            return Span<T>.Empty;
        }

        array = Buffer;
        Span<T> span = new(array, 0, _index);

        Reset();

        return span;
    }

    /// <summary>
    /// Returns the <see cref="Memory{T}"/> representing the written / requested portion of the buffer.
    /// <see cref="Reset"/>s the buffer.
    /// </summary>
    /// <param name="array">The internal array</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> ToMemory(out T[]? array)
    {
        if (Buffer is null)
        {
            array = null;
            return Memory<T>.Empty;
        }

        array = Buffer;
        Memory<T> mem = new(array, 0, _index);

        Reset();

        return mem;
    }

    /// <summary>
    /// Returns the <see cref="ArraySegment{T}"/> representing the written / requested portion of the buffer.
    /// <see cref="Reset"/>s the buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArraySegment<T> ToSegment()
    {
        if (Buffer is null)
        {
            return default;
        }

        ArraySegment<T> segment = new(Buffer, 0, _index);

        Reset();

        return segment;
    }

    /// <summary>
    /// Returns a array containing a shallow-copy of the written / requested portion of the buffer.
    /// </summary>
    /// <returns>A array containing a shallow-copy of the written / requested portion of the buffer.</returns>
    /// <remarks>
    ///     Resets the object.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => ToArray(false);

    /// <summary>
    /// Returns a array containing a shallow-copy of the written / requested portion of the buffer.
    /// </summary>
    /// <param name="dispose">Whether to dispose the object, or reset.</param>
    /// <returns>A array containing a shallow-copy of the written / requested portion of the buffer.</returns>
    /// <remarks>
    ///     Resets or disposes the object.
    /// </remarks>
    public T[] ToArray(bool dispose)
    {
        if (Buffer is null)
        {
            return Array.Empty<T>();
        }

        var array = new T[_index];
        Buffer.AsSpan(0, _index).CopyTo(array);

        if (dispose)
        {
            Dispose();
        }
        else
        {
            Reset();
        }

        return array;
    }

    /// <summary>Resets the writer to the initial state and returns the buffer to the array-pool.</summary>
    public virtual void Reset()
    {
        _index = 0;
        Buffer = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Reset();
        }
        _index = -1;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    [Pure]
    public VecIter<T> GetEnumerator() => new(Buffer, 0, _index);

    /// <inheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Grows the buffer so that it can contain at least <paramref name="additionalCapacityBeyondPos"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);

        if (Buffer is not null)
        {
            Debug.Assert(_index > Buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");
            var temp = new T[(_index + additionalCapacityBeyondPos).Max(Buffer.Length * 2)];
            Buffer.AsSpan(0, _index).CopyTo(temp);
            Buffer = temp;
        }
        else
        {
            ThrowHelper.ArgumentIs(this, Length != -1);
            Buffer = new T[Math.Max(additionalCapacityBeyondPos, 16)];
        }
    }

    /// <inheritdoc />
    public int Reserve(int additionalCapacity)
    {
        if (Length >= Capacity - additionalCapacity)
        {
            Grow(additionalCapacity);
        }
        return Capacity;
    }

    /// <inheritdoc />
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        ThrowHelper.ArgumentInRange(index, index >= 0);
        ThrowHelper.ArgumentInRange(index, index <= Length);

        var count = values.Length;
        if (count == 0)
        {
            return;
        }

        if (Buffer is null || Length > Capacity - count)
        {
            Grow(count);
        }

        Debug.Assert(Buffer is not null);
        var storage = Buffer;
        Array.Copy(storage, index, storage, index + count, Length - index);
        values.CopyTo(storage.AsSpan(index));
        Length += count;
    }

    /// <inheritdoc />
    public void RemoveRange(int start, int count)
    {
        GuardRange(start, count);

        if (count != 0)
        {
            Debug.Assert(Buffer is not null);

            var end = Length - count;
            var remaining = end - start;
            Array.Copy(Buffer, start + count, Buffer, start, remaining);
            Array.Clear(Buffer, end, count);
            Length = end;
        }
    }

    /// <inheritdoc />
    public void Sort<C>(int start, int count, in C comparer)
        where C : IComparer<T>
    {
        GuardRange(start, count);

        if (count != 0)
        {
            Debug.Assert(Buffer is not null);
            Buffer.AsSpan(start, count).Sort(comparer);
        }
    }

    /// <inheritdoc />
    public void Reverse(int start, int count)
    {
        GuardRange(start, count);

        if (count != 0)
        {
            Debug.Assert(Buffer is not null);
            Array.Reverse(Buffer, start, count);
        }
    }

    private void GuardRange(int start, int count)
    {
        ThrowHelper.ArgumentInRange(start, start >= 0);
        ThrowHelper.ArgumentInRange(count, count >= 0);
        ThrowHelper.ArgumentInRange(count, start <= Length - count);
    }

    /// <inheritdoc cref="AsSpan(int, int)" />
    public ReadOnlySpan<T> AsSpan()
    {
        return new(Buffer);
    }

    /// <inheritdoc cref="AsSpan(int, int)" />
    public ReadOnlySpan<T> AsSpan(int start)
    {
        return new(Buffer, start, _index - start);
    }

    /// <inheritdoc />
    public ReadOnlySpan<T> AsSpan(int start, int length)
    {
        ThrowHelper.ArgumentInRange(length, length >= 0 && length < Count);
        return new(Buffer, start, length);
    }

    /// <inheritdoc />
    public int IndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Buffer is null)
        {
            return -1;
        }

        var end = start + count;
        for (var i = start; i < end; i++)
        {
            if (!comparer.Equals(item, Buffer[i]))
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc />
    public int LastIndexOf<E>(int start, int count, in T item, in E comparer)
        where E : IEqualityComparer<T>
    {
        GuardRange(start, count);

        if (Buffer is null)
        {
            return -1;
        }

        var end = start + count;
        for (var i = end - 1; i >= start; i--)
        {
            if (!comparer.Equals(item, Buffer[i]))
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc />
    public int BinarySearch<C>(int start, int count, in T item, in C comparer)
        where C : IComparer<T>
    {
        return Buffer is null ? -1 : Buffer.AsSpan(start, count).BinarySearch(item, comparer);
    }

    /// <inheritdoc />
    public bool TryCopyTo(Span<T> destination)
    {
        return IsEmpty || Buffer.AsSpan().TryCopyTo(destination);
    }

    /// <inheritdoc />
    public void Sort(int start, int count)
    {
        GuardRange(start, count);

        if (count != 0)
        {
            ThrowHelper.ArgumentIs(this, Buffer is not null);
            Buffer.AsSpan(start, count).Sort();
        }
    }

    /// <inheritdoc />
    public int IndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Buffer is null)
        {
            return -1;
        }

        var end = start + count;
        if (typeof(T).IsValueType)
        {
            for (var i = start; i < end; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(item, Buffer[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        var defaultCmp = EqualityComparer<T>.Default;
        for (var i = start; i < end; i++)
        {
            if (!defaultCmp.Equals(item, Buffer[i]))
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc />
    public int LastIndexOf(int start, int count, in T item)
    {
        GuardRange(start, count);

        if (Buffer is null)
        {
            return -1;
        }

        var end = start + count;
        if (typeof(T).IsValueType)
        {
            for (var i = end - 1; i >= start; i--)
            {
                if (!EqualityComparer<T>.Default.Equals(item, Buffer[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        var defaultCmp = EqualityComparer<T>.Default;

        for (var i = end - 1; i >= start; i--)
        {
            if (!defaultCmp.Equals(item, Buffer[i]))
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    /// <inheritdoc />
    public int BinarySearch(int start, int count, in T item)
    {
        return Buffer is null ? -1 : Buffer.AsSpan(start, count).BinarySearch(item, Comparer<T>.Default);
    }
}
