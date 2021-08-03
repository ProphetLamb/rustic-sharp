using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using HeaplessUtility.Exceptions;

namespace HeaplessUtility
{
    /// <summary>
    ///     Partially inlined immutable collection of function parameters. 
    /// </summary>
    public static class ParamsSpan
    {
        /// <summary>
        ///     Returns an empty <see cref="ParamsSpan{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the span.</typeparam>
        /// <returns>An empty <see cref="ParamsSpan{T}"/>.</returns>
        public static ParamsSpan<T> Empty<T>() => default;
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        public static ParamsSpan<T> From<T>(in T arg0)
        {
            return new(1, arg0, default!, default!, default!);
        }
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        public static ParamsSpan<T> From<T>(in T arg0, in T arg1)
        {
            return new(2, arg0, arg1, default!, default!);
        }
        
        /// <summary>
        ///     Initializes a new parameter span with one argument.
        /// </summary>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <param name="arg2">The third argument.</param>
        public static ParamsSpan<T> From<T>(in T arg0, in T arg1, in T arg2)
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
        public static ParamsSpan<T> From<T>(in T arg0, in T arg1, in T arg2, in T arg3)
        {
            return new(4, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        public static ParamsSpan<T> From<T>(in ReadOnlySpan<T> arguments)
        {
            return new(arguments);
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments array.</param>
        public static ParamsSpan<T> From<T>(T[] arguments)
        {
            return new(new ReadOnlySpan<T>(arguments, 0, arguments.Length));
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        /// <param name="start">The zero-based index of the first argument.</param>
        public static ParamsSpan<T> From<T>(T[] arguments, int start)
        {
            return new(new ReadOnlySpan<T>(arguments, start, arguments.Length - start));
        }

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        /// <param name="start">The zero-based index of the first argument.</param>
        /// <param name="length">The number of arguments form the <paramref name="start"/>.</param>
        public static ParamsSpan<T> From<T>(T[] arguments, int start, int length)
        {
            return new(new ReadOnlySpan<T>(arguments, start, length));
        }
    }
    
    /// <summary>
    ///     A structure representing a immutable sequence of function parameters.
    /// </summary>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly ref struct ParamsSpan<T>
    {
#nullable disable
        private readonly int _length;
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
        [AllowNull]
#endif
        private readonly T _arg0;
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
        [AllowNull]
#endif
        private readonly T _arg1; 
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
        [AllowNull]
#endif
        private readonly T _arg2; 
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
        [AllowNull]
#endif
        private readonly T _arg3;
        private readonly ReadOnlySpan<T> _arguments;
        
        /// <summary>
        ///     Initializes a new parameter span with arguments.
        /// </summary>
        /// <param name="length">The number of non default arguments.</param>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <param name="arg2">The third argument.</param>
        /// <param name="arg3">The fourth argument.</param>
        internal ParamsSpan(int length,
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
            [AllowNull]
#endif
            in T arg0,
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
            [AllowNull]
#endif
            in T arg1,
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
            [AllowNull]
#endif
            in T arg2, 
#if NETSTANDARD2_1 || NET5_0 || NETCOREAPP3_1
            [AllowNull]
#endif
            in T arg3)
        {
            if ((uint)length > 4)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_OverEqualsMax(ExceptionArgument.length, length, 5);
            }
            
            _arguments = default;
            _length = length;
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }
#nullable restore

        /// <summary>
        ///     Initializes a new parameter span with a sequence of arguments.
        /// </summary>
        /// <param name="arguments">The arguments collection.</param>
        internal ParamsSpan(in ReadOnlySpan<T> arguments)
        {
            _arguments = arguments;
            _length = arguments.Length;

            _arg0 = arguments.Length > 0 ? arguments[0] : default!;
            _arg1 = arguments.Length > 1 ? arguments[1] : default!;
            _arg2 = arguments.Length > 2 ? arguments[2] : default!;
            _arg3 = arguments.Length > 3 ? arguments[3] : default!;
        }

        /// <summary>The number of items in the params span.</summary>
        public int Length => _length;

        /// <summary>Returns true if Length is 0.</summary>
        public bool IsEmpty => 0 >= (uint)_length;

        /// <inheritdoc cref="IReadOnlyList{T}.this" />
        public T this[int index]
        {
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
                    _ => _arguments[index]!,
                };
            }
        }

        /// <inheritdoc cref="Object.Equals(Object)" />
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj)
        {
            ThrowHelper.ThrowNotSupportedException();
            return default!; // unreachable.
        }

        /// <inheritdoc cref="Object.GetHashCode" />
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            ThrowHelper.ThrowNotSupportedException();
            return default!; // unreachable.
        }

        /// <inheritdoc cref="Span{T}.CopyTo"/>
        public void CopyTo(Span<T> destination)
        {
            if (!_arguments.IsEmpty)
            {
                _arguments.CopyTo(destination);
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

            if (!_arguments.IsEmpty)
            {
                _arguments.TryCopyTo(destination);
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

        /// <summary>
        ///     Returns <see langword="false"/> if left and right point at the same memory and have the same length.  Note that
        ///     this does *not* check to see if the *contents* are equal.
        /// </summary>
        public static bool operator ==(ParamsSpan<T> left, ParamsSpan<T> right)
        {
            if (left._length != right._length)
            {
                return false;
            }

            if (left._length > 4)
            {
                return left._arguments == right._arguments;
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
        public static bool operator !=(ParamsSpan<T> left, ParamsSpan<T> right) => !(left == right);

        /// <summary>Retrieves the backing span of the <see cref="ParamsSpan{T}"/> or allocates a array which is returned as a span.</summary>
        /// <returns>The span containing all items.</returns>
        public ReadOnlySpan<T> ToSpan() => ToSpan(false);
        
        /// <summary>Returns the span representation of the <see cref="ParamsSpan{T}"/>.</summary>
        /// <param name="onlyIfCheap">Whether return an empty span instead of allocating an array, if no span is backing the <see cref="ParamsSpan{T}"/>.</param>
        /// <returns>The span containing all items.</returns>
        public ReadOnlySpan<T> ToSpan(bool onlyIfCheap)
        {
            if (onlyIfCheap || IsEmpty || !_arguments.IsEmpty)
            {
                return _arguments;
            }

            T[]? array = _length switch
            {
                4 => new[] {_arg0!, _arg1!, _arg2!, _arg3!},
                3 => new[] {_arg0!, _arg1!, _arg2!},
                2 => new[] {_arg0!, _arg1!},
                1 => new[] {_arg0!},
                _ => default // unreachable
            };

            return new ReadOnlySpan<T>(array!, 0, _length);
        }

        private string GetDebuggerDisplay()
        {
            StringBuilder sb = new();
            sb.Append("Length = ");
            sb.Append(Length.ToString());
            sb.Append(", Params = {");

            int last = _length - 1;
            for (int i = 0; i < last; i++)
            {
                sb.Append(this[i]);
                sb.Append(", ");
            }
            
            if (!IsEmpty)
            {
                sb.Append(this[last]);
            }

            sb.Append('}');
            return sb.ToString();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        /// <summary>
        /// Creates a new <see cref="ParamsSpan{T}"/> with the contents of the <paramref name="span"/>. 
        /// </summary>
        /// <param name="span">The span to wrap.</param>
        /// <returns>The <see cref="ParamsSpan{T}"/> representation of the <paramref name="span"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ParamsSpan<T>(in ReadOnlySpan<T> span) => new(span);

        /// <summary>
        /// Creates a new <see cref="ParamsSpan{T}"/> containing the first and second element of the tuple.
        /// </summary>
        /// <param name="tuple">The tuple to wrap.</param>
        /// <returns>The <see cref="ParamsSpan{T}"/> representation of the <paramref name="tuple"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ParamsSpan<T>(in ValueTuple<T, T> tuple) => ParamsSpan.From(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Creates a new <see cref="ParamsSpan{T}"/> containing the first, second and third element of the tuple.
        /// </summary>
        /// <param name="tuple">The tuple to wrap.</param>
        /// <returns>The <see cref="ParamsSpan{T}"/> representation of the <paramref name="tuple"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ParamsSpan<T>(in ValueTuple<T, T, T> tuple) => ParamsSpan.From(tuple.Item1, tuple.Item2, tuple.Item3);

        /// <summary>
        /// Creates a new <see cref="ParamsSpan{T}"/> containing the first, second, third and fourth element of the tuple.
        /// </summary>
        /// <param name="tuple">The tuple to wrap.</param>
        /// <returns>The <see cref="ParamsSpan{T}"/> representation of the <paramref name="tuple"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ParamsSpan<T>(in ValueTuple<T, T, T, T> tuple) => ParamsSpan.From(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        
        /// <summary>Enumerates the elements of a <see cref="ParamsSpan{T}"/>.</summary>
        public ref struct Enumerator
        {
            /// <summary>The span being enumerated.</summary>
            private readonly ParamsSpan<T> _span;
            /// <summary>The next index to yield.</summary>
            private int _index;
 
            /// <summary>Initialize the enumerator.</summary>
            /// <param name="paramsSpan">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(in ParamsSpan<T> paramsSpan)
            {
                _span = paramsSpan;
                _index = -1;
            }
 
            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _index + 1;

                if ((uint)index < (uint)_span.Length)
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
                get => _span[_index]!;
            }
            
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