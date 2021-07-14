using System;
using System.Diagnostics;
using HeaplessUtility.Exceptions;
using HeaplessUtility.Pool;

namespace HeaplessUtility.DebuggerViews
{
    internal sealed class PoolBufferWriterDebuggerView<T>
    {
        private readonly WeakReference<BufferWriter<T>> _writerRef;

        public PoolBufferWriterDebuggerView(BufferWriter<T> writer)
        {
            _writerRef = new WeakReference<BufferWriter<T>>(writer);
        }
    
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                if (_writerRef.TryGetTarget(out BufferWriter<T>? writer))
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