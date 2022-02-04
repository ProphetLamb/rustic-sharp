using System;
using System.Collections.Generic;

namespace Rustic.Common;

/// <summary>Collection of extensions and utility functionality related to <see cref="Random"/> instances.</summary>
public static class LocalRandom
{
    [ThreadStatic]
    private static Random? SharedInstance;

    /// <summary>Gets the thread-local random pool.</summary>
    public static Random Shared => SharedInstance ??= new();

    /// <summary>Chooses a element from the collection using the random.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="random">The random used to seed the choice.</param>
    /// <param name="collection">The collection from which to choose from.</param>
    /// <returns>A element at a random position in the collection.</returns>
    public static ref readonly T ChooseFrom<T>(this Random random, ReadOnlySpan<T> collection)
    {
        return ref collection[random.Next(0, collection.Length)];
    }

    /// <summary>Chooses a element from the collection using the random.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="random">The random used to seed the choice.</param>
    /// <param name="collection">The collection from which to choose from.</param>
    /// <returns>A element at a random position in the collection.</returns>
    public static ref T ChooseFrom<T>(this Random random, T[] collection)
    {
        return ref collection[random.Next(0, collection.Length)];
    }

    /// <summary>Chooses a element from the collection using the random.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="random">The random used to seed the choice.</param>
    /// <param name="collection">The collection from which to choose from.</param>
    /// <returns>A element at a random position in the collection.</returns>
    public static T ChooseFrom<T>(this Random random, IReadOnlyList<T> collection)
    {
        return collection[random.Next(0, collection.Count)];
    }
}
