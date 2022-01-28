using System;
using System.Buffers;
using System.Diagnostics;

using Rustic.Common;

namespace Rustic.Memory.IO;

/// <summary>
///     Reusable <see cref="IBufferWriter{T}"/> intended for use as a thread-static singleton.
/// </summary>
/// <typeparam name="T">The type of the items.</typeparam>
/// <remarks>
///     Usage:
/// <code>
///     var obj = [...]
/// <br/>
///     PoolBufWriter&lt;byte&gt; writer = new();
/// <br/>
///     Serializer.Serialize(writer, obj);
/// <br/>
///     DoWork(writer.ToSpan(out byte[] poolArray));
/// <br/>
///     ArrayPool&lt;byte&gt;.Return(poolArray);
/// </code>
///  - or -
/// <code>
///     var obj = [...]
/// <br/>
///     PoolBufWriter&lt;byte&gt; writer = new();
/// <br/>
///     Serializer.Serialize(writer, obj);
/// <br/>
///     return writer.ToArray(dispose: true);
/// </code>
/// </remarks>
public class PoolBufWriter<T> : BufWriter<T>
{
    private readonly ArrayPool<T> _pool;

    /// <summary>
    ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
    /// </summary>
    /// <param name="pool">The array pool from which to rent.</param>
    public PoolBufWriter(ArrayPool<T>? pool = null)
    {
        _pool = pool ?? ArrayPool<T>.Shared;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="BufWriter{T}"/>.
    /// </summary>
    /// <param name="initialCapacity">The minimum capacity of the writer.</param>
    /// <param name="pool">The array pool from which to rent.</param>
    public PoolBufWriter(int initialCapacity, ArrayPool<T>? pool = null)
    {
        _pool = pool ?? ArrayPool<T>.Shared;
        Buffer = _pool.Rent(initialCapacity);
    }

    ///<inheritdoc/>
    protected override void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);

        if (Buffer != null)
        {
            Debug.Assert(Length > Buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");
            T[] temp = _pool.Rent(Math.Max(Length + additionalCapacityBeyondPos, Buffer.Length * 2));
            Buffer.AsSpan(0, Length).CopyTo(temp);
            Buffer = temp;
        }
        else
        {
            this.ValidateArg(Length != -1);
            Buffer = _pool.Rent(additionalCapacityBeyondPos);
        }
    }

    /// <summary>Resets the writer.</summary>
    public override void Reset()
    {
        var poolArray = Buffer;
        base.Reset();
        if (poolArray is not null)
        {
            _pool.Return(poolArray);
        }
    }
}
