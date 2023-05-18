using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
public struct RentArray<T> : IEnumerable<T>, IDisposable {
    [ThreadStatic]
    private static T[]?[]? s_exactPool;
    private T[] _array;
    private int _pos;

    /// <summary>Rents a new array of the specified length from the pool.</summary>
    /// <param name="length">The length of the array.</param>
    public RentArray(int length) {
        _array = Rent(length);
        _pos = 0;
    }

    /// <summary>The array backing the list</summary>
    public T[] Array => _array;

    /// <summary>Returns the reference to the element at the specified position inside the backing.</summary>
    /// <param name="index">The index.</param>
    /// <remarks>Does not respect the <see cref="Count"/> as an upper limit.</remarks>
    public ref T this[int index] => ref _array[index];

    /// <summary>Returns the number of elements added to the list.</summary>
    public int Count {
        get => _pos;
        set {
            Debug.Assert(value >= 0 && value <= Capacity);
            _pos = value;
        }
    }

    /// <summary>The number of elements the array can hold.</summary>
    public int Capacity => _array.Length;

    /// <summary>Adds the specified item to the list.</summary>
    /// <param name="item">The item to add.</param>
    public void Add(in T item) {
        int pos = Count;
        T[] array = Array;

        if (pos >= Capacity) {
            T[] poolArray = array;
            array = Rent(Capacity + 1);
            poolArray.AsSpan().CopyTo(array.AsSpan());
            Return(poolArray);
            _array = array;
        }

        array[pos] = item;
        Count = pos + 1;
    }

    /// <summary>Returns the array to the pool and resets this instance.</summary>
    public void Dispose() {
        Return(Array);
        this = default;
    }


    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    public Span<T>.Enumerator GetEnumerator() {
        return Array.AsSpan().GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
        return ((IEnumerable<T>) Array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable<T>) Array).GetEnumerator();
    }

    /// <summary>Rents an array with the exact number of elements.</summary>
    /// <param name="length">The number of elements.</param>
    /// <returns>The rented array.</returns>
    public static T[] Rent(int length) {
        Debug.Assert(length >= 0);
        T[]?[] pool = EnsurePool(length);
        T[]? rent = pool[length];
        if (rent is null) {
            return new T[length];
        }

        pool[length] = null;
        return rent;
    }

    private static T[]?[] EnsurePool(int length) {
        T[]?[]? pool = s_exactPool;
        if (pool is null || pool.Length <= length) {
            int cap = length.Max(16).Max((pool?.Length ?? 0) * 2);
            T[]?[] newPool = new T[]?[cap];
            pool?.AsSpan().CopyTo(newPool.AsSpan());
            s_exactPool = newPool;
            pool = newPool;
        }

        return pool;
    }

    /// <summary>Returns a rented array to the pool, or discards the array for GC.</summary>
    /// <param name="rented">The array to return.</param>
    public static void Return(T[] rented) {
        Debug.Assert(s_exactPool is null || rented.Length < s_exactPool.Length);
        T[]?[]? pool = s_exactPool;
        if (pool is not null) {
            pool[rented.Length] = rented;
        }
    }

    /// <inheritdoc cref="Array"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T[](in RentArray<T> arr) {
        return arr.Array;
    }
}
