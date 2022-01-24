using System;
using System.Diagnostics;

using HeaplessUtility.Common;
using HeaplessUtility.IO;

namespace HeaplessUtility.DebuggerViews
{
    internal sealed class PoolBufWriterDebuggerView<T>
    {
        private readonly WeakReference<BufWriter<T>> _writerRef;

        public PoolBufWriterDebuggerView(BufWriter<T> writer)
        {
            _writerRef = new WeakReference<BufWriter<T>>(writer);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                if (_writerRef.TryGetTarget(out BufWriter<T>? writer) && !writer.RawStorage.IsEmpty)
                {
                    var span = writer.RawStorage[..writer.Length];
                    return span.ToArray();
                }
                return Array.Empty<T>();
            }
        }
    }
}