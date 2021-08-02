using System.Collections.Generic;

namespace HeaplessUtility
{
    public interface IStrongEnumerable<TElement, out TEnumerator> : IEnumerable<TElement>
        where TEnumerator : IEnumerator<TElement>
    {
        TEnumerator GetEnumerator();
    }
}
