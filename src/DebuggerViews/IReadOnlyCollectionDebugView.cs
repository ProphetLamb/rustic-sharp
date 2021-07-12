using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HeaplessUtility.DebuggerViews
{
    public class IReadOnlyCollectionDebugView<T>
    {
        private readonly IReadOnlyCollection<T> _collection;
        
        public IReadOnlyCollectionDebugView(IReadOnlyCollection<T> collection)
        {
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return _collection.ToArray();
            }
        }
    }
}