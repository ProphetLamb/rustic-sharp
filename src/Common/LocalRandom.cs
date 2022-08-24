using System;
using System.Collections.Generic;

namespace Rustic;

/// <summary>Collection of extensions and utility functionality related to <see cref="Random"/> instances.</summary>
public static class LocalRandom
{
    [ThreadStatic]
    private static Random? SharedInstance;

    /// <summary>Gets the thread-local random pool.</summary>
    public static Random Shared => SharedInstance ??= new();

    /// <summary>Chooses a element from the collection using the random.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static ref readonly T ChooseFrom<T>(this Random random, ReadOnlySpan<T> collection)
    {
        return ref collection[random.Next(0, collection.Length)];
    }

    /// <summary>Chooses a element from the collection using the random.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static T ChooseFrom<T>(this Random random, IReadOnlyList<T> collection)
    {
        return collection[random.Next(0, collection.Count)];
    }

    /// <summary>Posix portable file name characters.</summary>
    public static ReadOnlySpan<char> PosixPortable => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_-.".AsSpan();

    /// <summary>Returns a random string of a specific length, with characters exclusively from an alphabet.</summary>
    public static string GetString(this Random random, ReadOnlySpan<char> alphabet, int length)
    {
        StrBuilder builder = length > 2048 ? new(length) : new(stackalloc char[length]);
        for (var i = 0; i < length; i++)
        {
            builder.Append(random.ChooseFrom(alphabet));
        }

        return builder.ToString();
    }

    /// <summary>Chooses a number of elements from the collection.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static IEnumerable<T> ChooseMany<T>(this Random random, IReadOnlyList<T> collection, int number)
    {
        for (var i = 0; i < number; i++)
        {
            yield return random.ChooseFrom(collection);
        }
    }
}
