using System;
using System.Buffers;
using System.Diagnostics;

namespace Rustic.Memory;

public class VecQuePooled<T> : VecQue<T>
{
    /// <inheritdoc />
    protected override void GrowContiguous(int additionalCapacity)
    {
        if (Storage is null)
        {
            Storage = ArrayPool<T>.Shared.Rent(additionalCapacity);
            return;
        }

        int newCapacity = (Capacity * 2).Max(Capacity + additionalCapacity);
        var newStorage = ArrayPool<T>.Shared.Rent(newCapacity);

        bool success = TryCopyTo(newStorage.AsSpan(0, Length));
        Debug.Assert(success, "This can never fail!");

        ArrayPool<T>.Shared.Return(Storage);

        Storage = newStorage;
    }

    protected override void GrowFixedSize(int additionalCapacity, bool requireContiguous)
    {
        if (Storage is null)
        {
            Storage = ArrayPool<T>.Shared.Rent(additionalCapacity);
            return;
        }

        if (requireContiguous)
        {
            if (additionalCapacity > Tail)
            {
                ThrowGrowFixedSize(nameof(Tail));
            }

            ShiftRightInternal(Tail);
            return;
        }

        if (additionalCapacity > Capacity)
        {
            ThrowGrowFixedSize(nameof(Capacity));
        }
    }
}
