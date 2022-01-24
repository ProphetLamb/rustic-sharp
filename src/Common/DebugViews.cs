using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HeaplessUtility.IO;

namespace HeaplessUtility
{
    internal class IReadOnlyCollectionDebugView<T>
    {
        private readonly WeakReference<IReadOnlyCollection<T>> _ref;

        public IReadOnlyCollectionDebugView(IReadOnlyCollection<T> collection)
        {
            _ref = new WeakReference<IReadOnlyCollection<T>>(collection);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                if (_ref.TryGetTarget(out var col) && col.Count > 0)
                {
                    return col.ToArray();
                }
                return Array.Empty<T>();
            }
        }
    }

    internal sealed class PoolBufWriterDebuggerView<T>
    {
        private readonly WeakReference<BufWriter<T>> _ref;

        public PoolBufWriterDebuggerView(BufWriter<T> writer)
        {
            _ref = new WeakReference<BufWriter<T>>(writer);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                if (_ref.TryGetTarget(out var writer) && !writer.RawStorage.IsEmpty)
                {
                    var span = writer.RawStorage[..writer.Length];
                    return span.ToArray();
                }
                return Array.Empty<T>();
            }
        }
    }
}