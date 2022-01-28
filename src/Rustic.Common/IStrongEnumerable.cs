using System.Collections.Generic;

namespace Rustic.Common;

/// <summary>
///     Exposes the strongly-typed enumerator, which supports a simple iteration over a collection of a specified type.
/// </summary>
/// <typeparam name="TElement">The type of objects to enumerate.</typeparam>
/// <typeparam name="TEnumerator">The type of the enumerator.</typeparam>
public interface IStrongEnumerable<out TElement, out TEnumerator> : IEnumerable<TElement>
    where TEnumerator : IEnumerator<TElement>
{
    /// <summary>
    ///     Returns an strongly-typed enumerator that iterates through the collection.
    /// </summary>
    new TEnumerator GetEnumerator();
}
