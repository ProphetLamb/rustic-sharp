using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rustic.Memory;

/// <summary>Represents a collection of bits with a list-like interface. Resorts to <see cref="ArrayPool{T}.Shared"/> when growing.</summary>
public ref struct BitVec
{
    private int[]? _returnToPool;
    private BitSet _raw;
    private int _pos;

    /// <summary>Initializes a new <see cref="BitVec"/> with the specified <paramref name="initialStorage"/>.</summary>
    public BitVec(Span<int> initialStorage)
    {
        _returnToPool = null;
        _raw = new(initialStorage);
        _pos = 0;
    }

    /// <summary>Initializes a new <see cref="BitVec"/> able to contain the specified number of bits.</summary>
    /// <param name="capacity">The lower limit to the number of bits the collection can contain.</param>
    public BitVec(int capacity)
    {
        int cap = Arithmetic.IntsToContainBits(capacity);
        int[]? temp = ArrayPool<int>.Shared.Rent(cap);
        _returnToPool = temp;
        _raw = new BitSet(temp);
        _pos = 0;
    }

    /// <summary>The number of bits in the collection.</summary>
    public int Count
    {
        get => _pos;
        set
        {
            value.ValidateArgRange(value >= 0);
            value.ValidateArgRange(value <= Capacity);
            _pos = Count;
        }
    }

    /// <summary>The upper limit to the amount of bits the collection can hold without reallocating.</summary>
    public int Capacity => _raw.RawStorage.Length << Arithmetic.IntWidth;

    /// <summary>Gets or sets the value of the bit at the specified index.</summary>
    public bool this[int index]
    {
        get
        {
            index.ValidateArgRange(index >= 0 && index < Count);
            return _raw.IsMarked(index);
        }
        set
        {
            index.ValidateArgRange(index >= 0 && index < Count);
            _raw.Set(index, Convert.ToInt32(value));
        }
    }

    /// <summary>Add the value to the end of the collection.</summary>
    public void Add(bool value)
    {
        Reserve(1);
        int pos = _pos;
        _raw.Set(pos, Convert.ToInt32(value));
        _pos = pos + 1;
    }

    /// <summary>Ensures that the collection can hold at least <paramref name="additionalCapacityBeyondPos"/> bits.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reserve(int additionalCapacityBeyondPos)
    {
        if (Count > Capacity - additionalCapacityBeyondPos)
        {
            Grow(additionalCapacityBeyondPos);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(Count > Capacity - additionalCapacityBeyondPos);

        int req = (Capacity * 2).Max(Count + additionalCapacityBeyondPos);
        int[]? temp = ArrayPool<int>.Shared.Rent(Arithmetic.IntsToContainBits(req));
        Span<int> span = temp;
        _raw.RawStorage.CopyTo(span);
        _raw = new(span);
        ReturnToPool();
        _returnToPool = temp;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ReturnToPool();
        this = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReturnToPool()
    {
        int[]? temp = _returnToPool;
        _returnToPool = null;
        if (temp is not null)
        {
            ArrayPool<int>.Shared.Return(temp);
        }
    }
}
