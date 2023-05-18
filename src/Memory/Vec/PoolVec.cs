using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rustic.Memory;

/// <summary>
///     Represents a strongly typed list of object that can be accessed by ref. Provides a similar interface as <see cref="System.Collections.Generic.List{T}"/>.
///     The list allocated from a <see cref="ArrayPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items of the list.</typeparam>
public sealed class PoolVec<T>
    : Vec<T>, IDisposable {
    /// <summary>The pool from which to rent and to wich to return the internal storage.</summary>
    private readonly ArrayPool<T> _pool;

    /// <summary>Initializes a new list.</summary>
    /// <param name="pool">The pool from which to allocate.</param>
    public PoolVec(ArrayPool<T>? pool = null) {
        _pool = pool ?? ArrayPool<T>.Shared;
    }

    /// <summary>Initializes a new list with a initial buffer.</summary>
    /// <param name="initialBuffer">The initial buffer.</param>
    /// <param name="pool">The pool from which to allocate.</param>
    public PoolVec(ArraySegment<T> initialBuffer, ArrayPool<T>? pool = null)
        : base(initialBuffer) {
        _pool = pool ?? ArrayPool<T>.Shared;
    }

    /// <summary>Initializes a new list with a specified minimum initial capacity.</summary>
    /// <param name="initialMinimumCapacity">The minimum initial capacity.</param>
    /// <param name="pool">The pool from which to allocate.</param>
    public PoolVec(int initialMinimumCapacity, ArrayPool<T>? pool = null) {
        _pool = pool ?? ArrayPool<T>.Shared;
        Data = new(_pool.Rent(initialMinimumCapacity));
    }

    /// <summary>Grows the list to have at least additional capacity beyond pos.</summary>
    /// <param name="additionalCapacityBeyondPos">Additional capacity beyond pos.</param>
    protected override void Grow(int additionalCapacityBeyondPos) {
        Debug.Assert(additionalCapacityBeyondPos > 0);

        if (Capacity != 0) {
            Debug.Assert(Count > Capacity - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            T[]? returnToPool = Data.Array;
            T[] temp = _pool.Rent((Count + additionalCapacityBeyondPos).Max(Capacity * 2));
            RawStorage.Slice(0, Count).CopyTo(temp);
            Data = new(temp);
            if (returnToPool is not null) {
                _pool.Return(returnToPool);
            }
        } else {
            Data = new(_pool.Rent(additionalCapacityBeyondPos));
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        Dispose(true);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    private void Dispose(bool disposing) {
        if (!disposing) {
            return;
        }

        T[]? returnToPool = Data.Array;
        Data = default;
        if (returnToPool is not null) {
            _pool.Return(returnToPool);
        }
    }
}
