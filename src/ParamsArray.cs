using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HeaplessUtility.Exceptions;

using JetBrains.Annotations;

namespace HeaplessUtility
{
    /// <summary>
    ///     Partially inlined immutable collection of function parameters. 
    /// </summary>
    public static class ParamsArray
    {
        /// <summary>
        ///     Returns an empty <see cref="ParamsArray{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the span.</typeparam>
        /// <returns>An empty <see cref="ParamsArray{T}"/>.</returns>
        public static ParamsArray<T> Empty<T>() => default;
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        public static ParamsArray<T> From<T>(in T arg0)
        {
            return new(1, arg0, default!, default!, default!);
        }
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        public static ParamsArray<T> From<T>(in T arg0, in T arg1)
        {
            return new(2, arg0, arg1, default!, default!);
        }
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <param name="arg2">The third argument.</param>
        public static ParamsArray<T> From<T>(in T arg0, in T arg1, in T arg2)
        {
            return new(3, arg0, arg1, arg2, default!);
        }
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <param name="arg2">The third argument.</param>
        /// <param name="arg3">The fourth argument.</param>
        public static ParamsArray<T> From<T>(in T arg0, in T arg1, in T arg2, in T arg3)
        {
            return new(4, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments array.</param>
        public static ParamsArray<T> From<T>(T[] arguments)
        {
            return new(new ArraySegment<T>(arguments, 0, arguments.Length));
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        /// <param name="start">The zero-based index of the first argument.</param>
        public static ParamsArray<T> From<T>(T[] arguments, int start)
        {
            return new(new ArraySegment<T>(arguments, start, arguments.Length - start));
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        /// <param name="start">The zero-based index of the first argument.</param>
        /// <param name="length">The number of arguments form the <paramref name="start"/>.</param>
        public static ParamsArray<T> From<T>(T[] arguments, int start, int length)
        {
            return new(new ArraySegment<T>(arguments, start, length));
        }
    }
    
    /// <summary>
    ///     A structure representing a immutable sequence of function parameters.
    /// </summary>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct ParamsArray<T>
        : IReadOnlyList<T>,
          IStrongEnumerable<T, ParamsArray<T>.Enumerator>
    {
        private readonly int _length;
        [CanBeNull]
#if NETSTANDARD2_1
        [AllowNull]
#endif
        private readonly T _arg0;
        [CanBeNull]
#if NETSTANDARD2_1
        [AllowNull]
#endif
        private readonly T _arg1; 
        [CanBeNull]
#if NETSTANDARD2_1
        [AllowNull]
#endif
        private readonly T _arg2; 
        [CanBeNull]
#if NETSTANDARD2_1
        [AllowNull]
#endif
        private readonly T _arg3;
        
#if NETSTANDARD2_0 
        private readonly ArraySegmentIterator<T> _params;
#else
        private readonly ArraySegment<T> _params;
#endif

        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="length">The number of non default arguments.</param>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <param name="arg2">The third argument.</param>
        /// <param name="arg3">The fourth argument.</param>
        internal ParamsArray(int length,
            [CanBeNull]
#if NETSTANDARD2_1
            [AllowNull]
#endif
            in T arg0,
             
            [CanBeNull]
#if NETSTANDARD2_1
            [AllowNull]
#endif
            in T arg1,
             
            [CanBeNull]
#if NETSTANDARD2_1
            [AllowNull]
#endif
            in T arg2, 
            [CanBeNull]
#if NETSTANDARD2_1
            [AllowNull]
#endif
            in T arg3)
        {
            if ((uint)length > 4)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument.length, length, 5);
            }

            _params = default;
            _length = length;
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of parameters.
        /// </summary>
        /// <param name="segment">The segment of parameters.</param>
        internal ParamsArray(in ArraySegment<T> segment)
        {
#if NETSTANDARD2_0
            _params = segment.GetIterator();
#else
            _params = segment;
#endif

            _length = _params.Count;

            int i = 0;
            _arg0 = _length > 0 ? _params[i++] : default!;
            _arg1 = _length > 1 ? _params[i++] : default!;
            _arg2 = _length > 2 ? _params[i++] : default!;
            _arg3 = _length > 3 ? _params[i] : default!;
        }

        /// <summary>The number of items in the params span.</summary>
        public int Length => _length;

        /// <inheritdoc/> 
        int IReadOnlyCollection<T>.Count => _length;

        /// <summary>Returns true if Length is 0.</summary>
        public bool IsEmpty => 0 >= (uint)_length;

        /// <inheritdoc cref="IReadOnlyList{T}.this"/>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint) index >= _length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_ArrayIndexOverMax(ExceptionArgument.index, index);
                }
                
                return index switch
                {
                    0 => _arg0!,
                    1 => _arg1!,
                    2 => _arg2!,
                    3 => _arg3!,
                    _ => _params[index],
                };
            }
        }
        
        /// <inheritdoc cref="Span{T}.CopyTo"/>
        public void CopyTo(Span<T> destination)
        {
            if (_params.Count > 0)
            {
                _params.AsSpan().CopyTo(destination);
            }

            if ((uint)_length <= (uint)destination.Length)
            {
                SetBlock(destination);
            }
        }

        /// <inheritdoc cref="Span{T}.TryCopyTo"/>
        public bool TryCopyTo(Span<T> destination)
        {
            bool retVal = false;

            if (_params.Count > 0)
            {
                _params.AsSpan().TryCopyTo(destination);
                retVal = true;
            }
            else if ((uint)_length <= (uint)destination.Length)
            {
                SetBlock(destination);
                retVal = true;
            }
            
            return retVal;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBlock(Span<T> destination)
        {
            int index = 0;
            switch (_length)
            {
                case 4:
                    destination[index++] = _arg0!;
                    destination[index++] = _arg1!;
                    destination[index++] = _arg2!;
                    destination[index] = _arg3!;
                    break;
                case 3:
                    destination[index++] = _arg0!;
                    destination[index++] = _arg1!;
                    destination[index] = _arg2!;
                    break;
                case 2:
                    destination[index++] = _arg0!;
                    destination[index] = _arg1!;
                    break;
                case 1:
                    destination[index] = _arg0!;
                    break;
            }
        }

        /// <inheritdoc cref="Object.Equals(Object)" />
        public bool Equals(in ParamsArray<T> other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ParamsArray<T> other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
        ///     this does *not* necessarily check to see if the *contents* are equal.
        /// </summary>
        public static bool operator ==(ParamsArray<T> left, ParamsArray<T> right)
        {
            if (left._length != right._length)
            {
                return false;
            }

            if (left._length > 4)
            {
                return left._params.AsSpan() == right._params.AsSpan();
            }

            return EqualityComparer<T>.Default.Equals(left._arg0, right._arg0)
                && EqualityComparer<T>.Default.Equals(left._arg1, right._arg1)
                && EqualityComparer<T>.Default.Equals(left._arg2, right._arg2)
                && EqualityComparer<T>.Default.Equals(left._arg3, right._arg3);
        }

        /// <summary>
        ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
        ///     this does *not* check to see if the *contents* are equal.
        /// </summary>
        public static bool operator !=(ParamsArray<T> left, ParamsArray<T> right) => !(left == right);

        /// <summary>Retrieves the backing span of the <see cref="ParamsArray{T}"/> or allocates a array which is returned as a span.</summary>
        /// <returns>The span containing all items.</returns>
        [System.Diagnostics.Contracts.Pure]
        public ReadOnlySpan<T> ToSpan() => ToSpan(false);
        
        /// <summary>Returns the span representation of the <see cref="ParamsArray{T}"/>.</summary>
        /// <param name="onlyIfCheap">Whether return an empty span instead of allocating an array, if no span is backing the <see cref="ParamsArray{T}"/>.</param>
        /// <returns>The span containing all items.</returns>
        [System.Diagnostics.Contracts.Pure]
        public ReadOnlySpan<T> ToSpan(bool onlyIfCheap)
        {
            if (onlyIfCheap || IsEmpty || _params.Count > 0)
            {
                if (_params.Array != null)
                {
                    return new ReadOnlySpan<T>(_params.Array, _params.Offset, _params.Count);
                }

                return default;
            }

            T[] array = _length switch
            {
                4 => new[] {_arg0!, _arg1!, _arg2!, _arg3!},
                3 => new[] {_arg0!, _arg1!, _arg2!},
                2 => new[] {_arg0!, _arg1!},
                1 => new[] {_arg0!},
                _ => Array.Empty<T>()
            };

            return new ReadOnlySpan<T>(array, 0, _length);
        }

        private string GetDebuggerDisplay()
        {
            ValueStringBuilder vsb = new(stackalloc char[256]);
            vsb.Append("Length = ");
            vsb.Append(Length.ToString());
            vsb.Append(", Params = {");

            int last = _length - 1;
            for (int i = 0; i < last; i++)
            {
                vsb.Append(this[i]!.ToString());
                vsb.Append(", ");
            }
            
            if (!IsEmpty)
            {
                vsb.Append(this[last]!.ToString());
            }

            vsb.Append('}');
            return vsb.ToString();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates the elements of a <see cref="ParamsArray{T}"/>.</summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly ParamsArray<T> _array;
            private int _index;
 
            /// <summary>Initialize the enumerator.</summary>
            /// <param name="paramsArray">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(in ParamsArray<T> paramsArray)
            {
                _array = paramsArray;
                _index = -1;
            }
 
            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _index + 1;

                if ((uint)index < (uint)_array.Length)
                {
                    _index = index;
                    return true;
                }
 
                return false;
            }
 
            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _array[_index]!;
            }

            object? IEnumerator.Current => Current;
            
            /// <summary>Resets the enumerator to the initial state.</summary>
            public void Reset()
            {
                _index = -1;
            }

            /// <summary>Disposes the enumerator.</summary>
            public void Dispose()
            {
                this = default;
            }
        }
    }
}