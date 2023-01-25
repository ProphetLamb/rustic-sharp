using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using static Rustic.Option;

namespace Rustic;

public static class Option
{
    /// <summary>Wraps the value in a <see cref="Option{T}"/> some.</summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(in T value) => new(value);

    /// <summary>Returns a new <see cref="Option{T}"/> none.</summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => default;

    /// <summary>Flattens a nested <see cref="Option{T}"/>.</summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Flatten<T>(this Option<Option<T>> self) => self.TrySome(out var inner) ? inner : default;

    /// <summary>Unzips the option of a tuple to a tuple of options.</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public static (Option<T>, Option<U>) Unzip<T, U>(in this Option<(T, U)> self)
    {
        if (self.TrySome(out var zipped))
        {
            return (zipped.Item1, zipped.Item2);
        }

        return (default, default);
    }

    #region TAP

    /// <summary>Invokes the action, returns none if successful; otherwise, returns some <see cref="Exception"/>.</summary>
    public static Option<Exception> TryInvoke(this Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            return e;
        }

        return default;
    }

    /// <summary>Awaits invoking the action, returns none if successful; otherwise, returns some <see cref="Exception"/>.</summary>
    public static async Task<Option<Exception>> TryInvoke(this Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            return e;
        }

        return default;
    }

    /// <summary>Awaits invoking the action, returns none if successful; otherwise, returns some exception <typeparamref name="E"/>.</summary>
    /// <typeparam name="E"></typeparam>
    public static async Task<Option<E>> TryInvoke<E>(this Func<Task> action)
        where E : Exception
    {
        try
        {
            await action();
        }
        catch (E e)
        {
            return e;
        }

        return default;
    }

    #endregion TAP

    public static IEnumerable<U> FilterMap<T, U>(this IEnumerable<T> sequence, Func<T, Option<U>> filterMap)
    {
        foreach (var element in sequence)
        {
            if (filterMap(element).TrySome(out U value))
            {
                yield return value;
            }
        }
    }
}

/// <summary>Represents an optional value. Every <see cref="Option{T}"/> is either <see cref="Some{T}"/> or <see cref="None{T}"/>.</summary>
/// <typeparam name="T">The type of the value.</typeparam>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Option<T> : IEquatable<Option<T>>
{
    private readonly byte _flags;
    private readonly T _value;

    internal Option(T value)
    {
        _flags = 1;
        _value = value;
    }

    /// <summary>Returns <c>true</c> if the option contains a value.</summary>
    public bool IsNone => _flags == 0;

    /// <summary>Returns <c>true</c> if the option does not contain a value.</summary>
    public bool IsSome => _flags != 0;

    /// <summary>If <see cref="IsSome"/>, unwraps the value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySome(out T value)
    {
        value = _value;
        return IsSome;
    }

    /// <summary>Returns the value without checking if the option contains a value.</summary>
    /// <remarks>Undefined behaviour if there is no value.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeUnchecked() => _value;

    /// <summary>If <see cref="IsSome"/> returns the value; otherwise, the <c>default</c> of the type.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: MaybeNull] public T SomeOrDefault() => IsSome ? _value : default!;

    /// <summary>If <see cref="IsSome"/> returns the value; otherwise, throws a <see cref="InvalidOperationException"/> with the <paramref name="message"/> specified.</summary>
    public T Expect(string? message = null)
    {
        if (!IsNone)
        {
            return _value;
        }

        ThrowHelper.ThrowInvalidOperationException(message);
        return default!; // unreachable
    }

    /// <summary>Coalesces the option with the fallback value specified.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOr(in T other) => IsNone ? other : _value;
    /// <summary>Coalesces the option with the fallback value specified.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOr(Func<T> other) => IsNone ? other() : _value;
    /// <summary>Coalesces the option with the fallback value specified.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe T SomeOr(delegate*<T> other) => IsNone ? other() : _value;

    /// <summary>If <see cref="IsSome"/> maps the value.</summary>
    /// <typeparam name="U">The type to map to.</typeparam>
    public Option<U> Map<U>(Func<T, U> map) => IsSome ? Some(map(_value)) : default;
    /// <summary>If <see cref="IsSome"/> maps the value.</summary>
    /// <typeparam name="U"></typeparam>
    [CLSCompliant(false)]
    public unsafe Option<U> Map<U>(delegate*<in T, U> map) => IsSome ? Some(map(_value)) : default;

    /// <summary>If <see cref="IsSome"/> maps the value; coalesces with the <paramref name="def"/>ault value.</summary>
    /// <typeparam name="U">The type to map to.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U MapOr<U>(Func<T, U> map, U def) => IsSome ? map(_value) : def;
    /// <summary>If <see cref="IsSome"/> maps the value; coalesces with the <paramref name="def"/>ault value.</summary>
    /// <typeparam name="U">The type to map to.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe U MapOr<U>(delegate*<T, U> map, U def) => IsSome ? map(_value) : def;
    /// <summary>If <see cref="IsSome"/> maps the value; coalesces with the <paramref name="def"/>ault value.</summary>
    /// <typeparam name="U">The type to map to.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U MapOr<U>(Func<T, U> map, Func<U> def) => IsSome ? map(_value) : def();
    /// <summary>If <see cref="IsSome"/> maps the value; coalesces with the <paramref name="def"/>ault value.</summary>
    /// <typeparam name="U">The type to map to.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe U MapOr<U>(delegate*<T, U> map, delegate*<U> def) => IsSome ? map(_value) : def();

    /// <summary>If <see cref="IsNone"/> returns None; otherwise, returns the <paramref name="other"/> option.</summary>
    /// <typeparam name="U">The of the other value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<U> And<U>(Option<U> other) => IsSome ? other : default;
    /// <summary>If <see cref="IsNone"/> returns None; otherwise, returns the <paramref name="other"/> option.</summary>
    /// <typeparam name="U">The of the other value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<U> And<U>(Func<Option<U>> other) => IsSome ? other() : default;
    /// <summary>If <see cref="IsNone"/> returns None; otherwise, returns the <paramref name="other"/> option.</summary>
    /// <typeparam name="U">The of the other value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe Option<U> And<U>(delegate*<Option<U>> other) => IsSome ? other() : default;

    /// <summary>If <see cref="IsSome"/> returns this; otherwise, returns the <paramref name="other"/> option.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<T> Or(Option<T> other) => IsSome ? this : other;
    /// <summary>If <see cref="IsSome"/> returns this; otherwise, returns the <paramref name="other"/> option.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<T> Or(Func<Option<T>> other) => IsSome ? this : other();
    /// <summary>If <see cref="IsSome"/> returns this; otherwise, returns the <paramref name="other"/> option.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe Option<T> Or(delegate*<Option<T>> other) => IsSome ? this : other();

    /// <summary>Returns Some if exactly one of this, and <paramref name="other"/> is Some, otherwise returns None.</summary>
    public Option<T> Xor(Option<T> other)
    {
        if (IsSome)
        {
            if (other.IsSome)
            {
                return default;
            }
            return this;
        }
        return other;
    }

    /// <summary>If <see cref="IsSome"/> and the filter applies to the value returns Some; otherwise returns None.</summary>
    public Option<T> Where(Predicate<T> filter) => IsSome && filter(_value) ? this : default;
    /// <summary>If <see cref="IsSome"/> and the filter applies to the value returns Some; otherwise returns None.</summary>
    [CLSCompliant(false)]
    public unsafe Option<T> Where(delegate*<T, bool> filter) => IsSome && filter(_value) ? this : default;

    /// <summary>Zips the option with another; otherwise, returns None.</summary>
    /// <typeparam name="U"></typeparam>
    public Option<(T, U)> Zip<U>(Option<U> other) => IsSome && other.IsSome ? Some((_value, other._value)) : default;
    /// <summary>Zips the option with another then maps the value; otherwise, returns None.</summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    public Option<R> Zip<U, R>(Option<U> other, Func<T, U, R> map) => Zip(other).TrySome(out var zipped) ? Some(map(zipped.Item1, zipped.Item2)) : default;
    /// <summary>Zips the option with another then maps the value; otherwise, returns None.</summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    [CLSCompliant(false)]
    public unsafe Option<R> Zip<U, R>(Option<U> other, delegate*<T, U, R> map) => Zip(other).TrySome(out var zipped) ? Some(map(zipped.Item1, zipped.Item2)) : default;

    /// <summary>Returns <c>true</c> if Some value is equal to the <paramref name="value"/>; otherwise, <c>false</c>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in T value) => IsSome && EqualityComparer<T>.Default.Equals(_value, value);

    /// <summary>Returns <c>true</c> if Some value is equal to the <paramref name="value"/>; otherwise, <c>false</c>. Used the <paramref name="comparer"/> to compare the values.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in T value, IEqualityComparer<T> comparer) => IsSome && comparer.Equals(_value, value);

    #region IEquatable members

    public bool Equals(Option<T> other)
    {
        return _flags == other._flags && EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public bool Equals(Option<T> other, IEqualityComparer<T> comparer)
    {
        return _flags == other._flags && comparer.Equals(_value, other._value);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Option<T> other => Equals(other),
            T some => Equals(some),
            _ => false
        };
    }

    public override int GetHashCode() => HashCode.Combine(_flags, _value);

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    #endregion IEquatable members

    /// <summary>Returns the nullable class or struct converted to <see cref="Some"/> when not null; otherwise, <see cref="None"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(in T? value) => value is null ? default : new(value!);
}
