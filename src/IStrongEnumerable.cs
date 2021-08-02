using System.Collections.Generic;

namespace HeaplessUtility
{
    public interface IStrongEnumerable<TElement, out TEnumerator>
        where TEnumerator : IEnumerator<TElement>
    {
        TEnumerator GetEnumerator();
    }
}
