using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rustic.Memory;

/// <summary>Proxy class used for displaying a <see cref="IReadOnlyCollection{T}"/> in the debugger.</summary>
/// <typeparam name="T"></typeparam>
public sealed class IReadOnlyCollectionDebugView<T> {
    private readonly WeakReference<IReadOnlyCollection<T>> _ref;

    /// <summary>Intializes a new instance of <see cref="IReadOnlyCollectionDebugView{T}"/>.</summary>
    /// <param name="collection">The collection to display.</param>
    public IReadOnlyCollectionDebugView(IReadOnlyCollection<T> collection) {
        _ref = new WeakReference<IReadOnlyCollection<T>>(collection);
    }

    /// <summary>A shallow-copy of the items of the collection.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items {
        get {
            if (_ref.TryGetTarget(out IReadOnlyCollection<T>? col) && col.Count > 0) {
                return col.ToArray();
            }

            return Array.Empty<T>();
        }
    }
}
