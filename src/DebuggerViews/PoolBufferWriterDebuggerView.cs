using System;
using System.Diagnostics;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility.DebuggerViews
{
    internal sealed class PoolBufferWriterDebuggerView<T>
    {
        private readonly WeakReference<PoolBufferWriter<T>> _writerRef;

        public PoolBufferWriterDebuggerView(PoolBufferWriter<T> writer)
        {
            _writerRef = new WeakReference<PoolBufferWriter<T>>(writer);
        }
    
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                if (_writerRef.TryGetTarget(out PoolBufferWriter<T>? writer))
                {
                    if (writer._buffer != null)
                    {
                        var span = writer._buffer.AsSpan(0, writer.Count);
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