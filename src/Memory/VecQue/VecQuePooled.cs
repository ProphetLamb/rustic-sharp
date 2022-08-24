using System;
using System.Buffers;
using System.Diagnostics;

namespace Rustic.Memory;

/// <summary>
///     <see cref="VecQue{T}"/> implementation allocating from an <see cref="ArrayPool{T}"/>.
/// </summary>
/// <remarks>
///     Beware any initial buffer passed must be part of the pool.
/// </remarks>
public class VecQuePooled<T> : VecQue<T>, IDisposable
{
    private ArrayPool<T> _pool = ArrayPool<T>.Shared;

    /// <summary>
    /// Gets or sets the underlying <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <remarks>
    /// Only cheap if `IsEmpty`, otherwise copies elements to new array
    /// </remarks>
    public ArrayPool<T> Pool
    {
        get => _pool;
        set
        {
            if (!IsEmpty)
            {
                var newStorage = value.Rent(Capacity);
                bool success = TryCopyTo(newStorage.AsSpan(0, Length));
                Debug.Assert(success, "This can never fail!");
                _pool.Return(Storage);
                Storage = newStorage;
            }
            else if (Storage is not null)
            {
                _pool.Return(Storage);
                Storage = null;
            }

            _pool = value;
        }
    }

    /// <inheritdoc />
    protected override void GrowContiguous(int additionalCapacity)
    {
        if (Storage is null)
        {
            Storage = _pool.Rent(additionalCapacity);
            return;
        }

        int newCapacity = (Capacity * 2).Max(Capacity + additionalCapacity);
        var newStorage = _pool.Rent(newCapacity);

        bool success = TryCopyTo(newStorage.AsSpan(0, Length));
        Debug.Assert(success, "This can never fail!");
        _pool.Return(Storage);

        Storage = newStorage;
        SetTailValue(0);
    }

    /// <inheritdoc />
    protected override void GrowFixedSize(int additionalCapacity, bool requireContiguous)
    {
        if (Storage is null)
        {
            Storage = _pool.Rent(additionalCapacity);
            return;
        }

        if (additionalCapacity + Length > Capacity)
        {
            ThrowGrowFixedSize(nameof(Capacity));
        }

        if (!requireContiguous)
        {
            return;
        }

        if (GetContiguousCapacityIndexVirtual(additionalCapacity, out _) >= 0)
        {
            return;
        }

        MoveLeftVirtual(Tail, Length, Tail);
        SetTailValue(0);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Storage is not null)
        {
            _pool.Return(Storage);
        }
        GC.SuppressFinalize(this);
    }
}
