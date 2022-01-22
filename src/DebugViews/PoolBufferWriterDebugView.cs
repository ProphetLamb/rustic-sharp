using System;
using System.Diagnostics;
using HeaplessUtility.Exceptions;
using HeaplessUtility.Pool;

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
                if (_writerRef.TryGetTarget(out BufWriter<T>? writer))
                {
                    if (writer.RawStorage != null)
                    {
                        var span = writer.RawStorage.Slice(0, writer.Count);
                        return span.ToArray();
                    }

                    return Array.Empty<T>();
                }

                ThrowHelper.ThrowInvalidOperationException_ObjectDisposed();
                return default!; // unreachable
            }
        }
    }
}