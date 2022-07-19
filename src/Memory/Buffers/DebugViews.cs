using System;
using System.Diagnostics;

namespace Rustic.Memory;

public sealed class PoolBufWriterDebuggerView<T>
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
                var span = writer.AsSpan();
                return span.ToArray();
            }
            return Array.Empty<T>();
        }
    }
}
