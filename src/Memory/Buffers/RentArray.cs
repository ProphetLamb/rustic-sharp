using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rustic.Memory;

/// <summary>Allows renting a array without necessarily allocating.</summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// <para>To be used when tiny arrays of exact size are required by hot API, to reduce load on the GC.
/// Each thread used a separate array pool from which arrays of continuously increasing size are stored.</para>
/// <para>Only use <see cref="Rent"/> tiny arrays.</para>
/// <para>Can grow, but should be avoided where possible because the grow increment is one item.</para>
/// <code>
/// using (RentArray&lt;Type&gt; rent = new(2) { typeof(int), typeof(T) })
/// {
///     DoSomething(rent);
/// }
///
/// void DoSomething(params Type[] paramTypes)
/// {
///     [...]
/// }
/// </code>
/// </remarks>
public struct RentArray<T> : IEnumerable<T>, IDisposable
{
    [ThreadStatic] internal static T[]?[]? ExactPool;
    private T[] _array;
    private int _pos;

    public RentArray(int length)
    {
        _array = Rent(length);
        _pos = 0;
    }

    public T[] Array => _array;

    public ref T this[int index] => ref _array[index];

    public int Count
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0 && value <= Capacity);
            _pos = value;
        }
    }

    public int Capacity => _array.Length;

    public void Add(in T item)
    {
        var pos = Count;
        var array = Array;

        if (pos >= Capacity)
        {
            var poolArray = array;
            array = Rent(Capacity + 1);
            poolArray.AsSpan().CopyTo(array.AsSpan());
            Return(poolArray);
            _array = array;
        }

        array[pos] = item;
        Count = pos + 1;
    }

    public void Dispose()
    {
        Return(Array);
        this = default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static T[] Rent(int length)
    {
        Debug.Assert(length >= 0);
        var pool = EnsurePool(length);
        var rent = pool[length];
        if (rent is null)
        {
            return new T[length];
        }
        pool[length] = null;
        return rent;
    }

    private static T[]?[] EnsurePool(int length)
    {
        var pool = ExactPool;
        if (pool is null || pool.Length <= length)
        {
            var cap = length.Max(16).Max((pool?.Length ?? 0) * 2);
            var newPool = new T[]?[cap];
            pool?.AsSpan().CopyTo(newPool.AsSpan());
            ExactPool = newPool;
            pool = newPool;
        }

        return pool;
    }

    public static void Return(T[] rented)
    {
        Debug.Assert(ExactPool is null || rented.Length < ExactPool.Length);
        var pool = ExactPool;
        if (pool is not null)
        {
            pool[rented.Length] = rented;
        }
    }

    public static implicit operator T[](RentArray<T> rent) => rent.Array;
}
