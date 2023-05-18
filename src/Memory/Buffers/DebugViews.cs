using System;
using System.Diagnostics;

namespace Rustic.Memory;

internal sealed class PoolBufWriterDebuggerView<T> {
    private readonly WeakReference<BufWriter<T>> _ref;

    public PoolBufWriterDebuggerView(BufWriter<T> writer) {
        _ref = new WeakReference<BufWriter<T>>(writer);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items {
        get {
            if (_ref.TryGetTarget(out BufWriter<T>? writer) && !writer.RawStorage.IsEmpty) {
                ReadOnlySpan<T> span = writer.AsSpan();
                return span.ToArray();
            }

            return Array.Empty<T>();
        }
    }
}
