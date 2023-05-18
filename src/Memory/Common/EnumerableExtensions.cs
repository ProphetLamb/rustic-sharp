using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rustic.Memory;

/// <summary>Extensions for types implementing <see cref="IEnumerable{T}"/>.</summary>
public static class EnumerableExtensions {
    /// <summary>Zips the sequence of elements with a incrementing count from zero, indicating the index of the item in a array-list.</summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> source) {
        return new IndexIterator<T>(source);
    }

    private sealed class IndexIterator<T> : IEnumerator<(int, T)>, IEnumerable<(int, T)> {
        private IEnumerable<T>? _source;
        private IEnumerator<T>? _enumerator;
        private int _index = -1;

        public IndexIterator(IEnumerable<T> source) {
            _source = source;
        }

        public (int, T) Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_index, _enumerator!.Current);
        }

        object IEnumerator.Current => Current;

        public bool MoveNext() {
            if (_source is null) {
                ThrowHelper.ObjectDisposedException(nameof(IndexIterator<T>));
            }

            _enumerator ??= _source.GetEnumerator();
            _index++;
            return _enumerator.MoveNext();
        }

        public void Dispose() {
            _source = null;
            Reset();
        }

        public void Reset() {
            _enumerator = null;
            _index = -1;
        }

        public IEnumerator<(int, T)> GetEnumerator() {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
