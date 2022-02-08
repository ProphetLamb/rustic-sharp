using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using static Rustic.Option;
using static Rustic.Result;

namespace Rustic;

public static class Result
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(in T ok) => new(1, ok, default!);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Ok<T, E>(in T ok) => new(1, ok, default!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Err<T>(Exception err) => new(0, default!, err);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Err<T, E>(in E err) => new(0, default!, err);

    public static Option<Result<T, E>> Transpose<T, E>(in this Result<Option<T>, E> self)
    {
        if (self.TryOk(out var option))
        {
            if (option.TrySome(out var some))
            {
                return Ok<T, E>(some);
            }

            return default;
        }

        return Some(Err<T, E>(self.ErrUnchecked()));
    }

    public static Result<T, E> Flatten<T, E>(in this Result<Result<T, E>, E> self) => self.TryOk(out var result) ? result : Err<T, E>(self.ErrUnchecked());
    public static Result<T, E> Flatten<T, E>(in this Result<T, Result<T, E>> self) => self.TryErr(out var result) ? result : Ok<T, E>(self.OkUnchecked());

    public static T Either<T>(in this Result<T, T> self) => self.IsOk ? self.OkUnchecked() : self.ErrUnchecked();

    public static Option<Result<T, E>> Fold<T, E>(this IEnumerable<Result<T, E>> self, Func<T, T, T> foldOk, Func<E, E, E> foldErr)
    {
        bool hasOk = false;
        T resOk = default!;
        bool hasErr = false;
        E resErr = default!;

        foreach (var r in self)
        {
            if (!hasErr && r.TryOk(out var ok))
            {
                resOk = hasOk ? foldOk(resOk, ok) : ok;
                hasOk = true;
            }
            else if (r.TryErr(out var err))
            {
                resErr = hasErr ? foldErr(resErr, err) : err;
                hasErr = true;
            }
        }

        if (hasErr)
        {
            return Some(Err<T, E>(resErr));
        }

        if (hasOk)
        {
            return Some(Ok<T, E>(resOk));
        }

        return default;
    }

    #region TAP

    public static Result<T> TryInvoke<T>(this Func<T> action)
    {
        T res;
        try
        {
            res = action();
        }
        catch (Exception e)
        {
            return Err<T>(e);
        }

        return Ok(res);
    }

    public static Result<T, E> TryInvoke<T, E>(this Func<T> action)
        where E : Exception
    {
        T res;
        try
        {
            res = action();
        }
        catch (E e)
        {
            return Err<T, E>(e);
        }

        return Ok<T, E>(res);
    }

    public static async Task<Result<T>> TryInvoke<T>(this Func<Task<T>> action)
    {
        T res;
        try
        {
            res = await action();
        }
        catch (Exception e)
        {
            return Err<T>(e);
        }

        return Ok(res);
    }

    public static async Task<Result<T, E>> TryInvoke<T, E>(this Func<Task<T>> action)
        where E : Exception
    {
        T res;
        try
        {
            res = await action();
        }
        catch (E e)
        {
            return Err<T, E>(e);
        }

        return Ok<T, E>(res);
    }

    #endregion TAP
}

/// <summary>Result is a type that represents either success (Ok) or failure (Err).</summary>
/// <typeparam name="T">The type of the Ok value.</typeparam>
[StructLayout(LayoutKind.Explicit)]
public readonly struct Result<T> : IEquatable<Result<T>>
{
    [FieldOffset(0)] private readonly byte _flags;
    [FieldOffset(1)] private readonly T _ok;
    [FieldOffset(1)] private readonly Exception _err;

    internal Result(byte flags, T ok, Exception err)
    {
        _flags = flags;
        if (_flags == 1)
        {
            _err = err;
            _ok = ok;
        }
        else
        {
            _ok = ok;
            _err = err;
        }
    }

    public bool IsErr => _flags == 0;
    public bool IsOk => _flags == 1;

    public bool TryOk(out T ok)
    {
        if (IsOk)
        {
            ok = _ok;
            return true;
        }

        ok = default!;
        return false;
    }

    public bool TryErr(out Exception err)
    {
        if (IsErr)
        {
            err = _err;
            return true;
        }

        err = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkUnchecked() => _ok;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception ErrUnchecked() => _err;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOrDefault() => IsOk ? _ok : default!;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception ErrOrDefault() => IsErr ? _err : default!;

    public T ExpectOk(string? message = null)
    {
        if (!IsOk)
        {
            ThrowHelper.ThrowInvalidOperationException(message);
        }
        return _ok;
    }

    public Exception ExpectErr(string? message = null)
    {
        if (!IsErr)
        {
            ThrowHelper.ThrowInvalidOperationException(message);
        }
        return _err;
    }

    #region IEquatable members

    public bool Equals(Result<T> other)
    {
        if (_flags != other._flags)
        {
            return false;
        }

        return IsOk ? EqualityComparer<T>.Default.Equals(_ok, other._ok) : EqualityComparer<Exception>.Default.Equals(_err, other._err);
    }

    public bool Equals(in Result<T> other, IEqualityComparer<T> okComparer, IEqualityComparer<Exception> errComparer)
    {
        if (_flags != other._flags)
        {
            return false;
        }

        return IsOk ? okComparer.Equals(_ok, other._ok) : errComparer.Equals(_err, other._err);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<T> other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(_flags, _ok, _err);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    #endregion IEquatable members

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T, Exception>(Result<T> result) => Unsafe.As<Result<T>, Result<T, Exception>>(ref result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(Exception ex) => Err<T>(ex);
}

/// <summary>Result is a type that represents either success (Ok) or failure (Err).</summary>
/// <typeparam name="T">The type of the Ok value.</typeparam>
/// <typeparam name="E">The type of the Err value.</typeparam>
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public readonly struct Result<T, E> : IEquatable<Result<T, E>>, ISerializable
{
    [FieldOffset(0)] private readonly byte _flags;
    [FieldOffset(1)] private readonly T _ok;
    [FieldOffset(1)] private readonly E _err;

    internal Result(byte flags, T ok, E err)
    {
        _flags = flags;
        if (_flags == 1)
        {
            _err = err;
            _ok = ok;
        }
        else
        {
            _ok = ok;
            _err = err;
        }
    }

    private Result(SerializationInfo info, StreamingContext context)
    {
        _flags = 0;
        _ok = default!;
        _err = default!;

        foreach (var entry in info)
        {
            if (entry.Name.Equals("Ok", StringComparison.OrdinalIgnoreCase))
            {
                _flags = 1;
                _err = default!;
                var v = entry.Value;
                _ok = v is null ? default! : (T)v;
                break;
            }

            if (entry.Name.Equals("Err", StringComparison.OrdinalIgnoreCase))
            {
                _flags = 0;
                _ok = default!;
                var v = entry.Value;
                _err = v is null ? default! : (E)v;
                break;
            }
        }
    }

    public bool IsErr => _flags == 0;
    public bool IsOk => _flags == 1;

    public bool TryOk(out T ok)
    {
        if (IsOk)
        {
            ok = _ok;
            return true;
        }

        ok = default!;
        return false;
    }

    public bool TryErr(out E err)
    {
        if (IsErr)
        {
            err = _err;
            return true;
        }

        err = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkUnchecked() => _ok;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrUnchecked() => _err;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOrDefault() => IsOk ? _ok : default!;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrOrDefault() => IsErr ? _err : default!;

    public T ExpectOk(string? message = null)
    {
        if (!IsOk)
        {
            ThrowHelper.ThrowInvalidOperationException(message);
        }
        return _ok;
    }

    public E ExpectErr(string? message = null)
    {
        if (!IsErr)
        {
            ThrowHelper.ThrowInvalidOperationException(message);
        }
        return _err;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<T> Ok() => IsOk ? Some(_ok) : default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<E> Err() => IsErr ? Some(_err) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOr(T def) => IsOk ? _ok : def;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOr(Func<T> def) => IsOk ? _ok : def();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe T OkOr(delegate*<T> def) => IsOk ? _ok : def();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrOr(E def) => IsErr ? _err : def;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrOr(Func<E> def) => IsErr ? _err : def();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe E ErrOr(delegate*<E> def) => IsErr ? _err : def();

    public Result<U, E> Map<U>(Func<T, U> map) => IsOk ? Ok<U, E>(map(_ok)) : Err<U, E>(_err);
    [CLSCompliant(false)]
    public unsafe Result<U, E> Map<U>(delegate*<T, U> map) => IsOk ? Ok<U, E>(map(_ok)) : Err<U, E>(_err);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U MapOr<U>(Func<T, U> map, U def) => IsOk ? map(_ok) : def;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe U MapOr<U>(delegate*<T, U> map, U def) => IsOk ? map(_ok) : def;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U MapOr<U>(Func<T, U> map, Func<U> def) => IsOk ? map(_ok) : def();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public unsafe U MapOr<U>(delegate*<T, U> map, delegate*<U> def) => IsOk ? map(_ok) : def();

    public Result<T, F> MapErr<F>(Func<E, F> map) => IsOk ? Ok<T, F>(_ok) : Err<T, F>(map(_err));
    [CLSCompliant(false)]
    public unsafe Result<T, F> MapErr<F>(delegate*<E, F> map) => IsOk ? Ok<T, F>(_ok) : Err<T, F>(map(_err));

    public Result<U, E> And<U>(Result<U, E> res) => IsOk ? res : Err<U, E>(_err);
    public Result<U, E> And<U>(Func<Result<U, E>> res) => IsOk ? res() : Err<U, E>(_err);
    [CLSCompliant(false)]
    public unsafe Result<U, E> And<U>(delegate*<Result<U, E>> res) => IsOk ? res() : Err<U, E>(_err);

    public Result<T, F> Or<F>(Result<T, F> res) => IsOk ? Ok<T, F>(_ok) : res;
    public Result<T, F> Or<F>(Func<E, Result<T, F>> res) => IsOk ? Ok<T, F>(_ok) : res(_err);
    [CLSCompliant(false)]
    public unsafe Result<T, F> Or<F>(delegate*<E, Result<T, F>> res) => IsOk ? Ok<T, F>(_ok) : res(_err);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsOk(in T value) => IsOk && EqualityComparer<T>.Default.Equals(_ok, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsOk(in T value, IEqualityComparer<T> comparer) => IsOk && comparer.Equals(_ok, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsErr(in E value) => IsOk && EqualityComparer<E>.Default.Equals(_err, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsErr(in E value, IEqualityComparer<E> comparer) => IsOk && comparer.Equals(_err, value);

    #region IEquatable members
    public bool Equals(Result<T, E> other)
    {
        if (_flags != other._flags)
        {
            return false;
        }

        return IsOk ? EqualityComparer<T>.Default.Equals(_ok, other._ok) : EqualityComparer<E>.Default.Equals(_err, other._err);
    }

    public bool Equals(in Result<T, E> other, IEqualityComparer<T> okComparer, IEqualityComparer<E> errComparer)
    {
        if (_flags != other._flags)
        {
            return false;
        }

        return IsOk ? okComparer.Equals(_ok, other._ok) : errComparer.Equals(_err, other._err);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<T, E> other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(_flags, _ok, _err);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);

    #endregion IEquatable members

    #region ISerializable members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (IsOk)
        {
            info.AddValue("Ok", _ok, typeof(T));
        }
        else
        {
            info.AddValue("Err", _err, typeof(E));
        }
    }

    #endregion ISerializable members
}

/// <summary>Converts the <see cref="Result{T,E}"/> to and from json.</summary>
/// <typeparam name="T">The type of the Ok value.</typeparam>
/// <typeparam name="E">The type of the Err value.</typeparam>
public sealed class ResultJsonConverter<T, E> : JsonConverter<Result<T, E>>
{
    public override Result<T, E> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType.IsNullOrNone())
        {
            reader.Read();
            return default;
        }

        reader.ReadOf(JsonTokenType.StartObject);

        // Empty object is "Err".
        if (reader.TokenType == JsonTokenType.EndObject)
        {
            reader.Read();
            return default;
        }

        // Read the result content
        var v = ReadResult(ref reader, options);

        reader.ReadOf(JsonTokenType.EndObject);
        return v;
    }

    private Result<T, E> ReadResult(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        // Object with no property name implies "Ok"
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return Ok<T,E>(ReadOkValue(ref reader, options));
        }

        // Read exact property name "Ok" or "Err"
        reader.TokenType.ThrowIfNot(JsonTokenType.PropertyName);
        string key = reader.GetKeyString();
        reader.ReadOrThrow();

        if (key.Equals("Ok", StringComparison.OrdinalIgnoreCase))
        {
            return Ok<T, E>(ReadOkValue(ref reader, options));
        }

        if (key.Equals("Err", StringComparison.OrdinalIgnoreCase))
        {
            return Err<T, E>(ReadErrValue(ref reader, options));
        }

        ThrowHelper.ThrowJsonException($"Unexpected property key '{key}', expected 'Ok' or 'Err'.");
        return default;
    }

    private T ReadOkValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<T>();
        return converter.Read(ref reader, typeof(T), options)!;
    }

    private E ReadErrValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<E>();
        return converter.Read(ref reader, typeof(E), options)!;
    }

    public override void Write(Utf8JsonWriter writer, Result<T, E> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.TryOk(out var ok))
        {
            writer.WritePropertyName("Ok");
            var converter = options.GetConverter<T>();
            converter.Write(writer, ok, options);
        }

        if (value.TryErr(out var err))
        {
            writer.WritePropertyName("Err");
            var converter = options.GetConverter<E>();
            converter.Write(writer, err, options);
        }

        writer.WriteEndObject();
    }
}

/// <summary>Converts the <see cref="Result{T}"/> to and from json.</summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType.IsNullOrNone())
        {
            reader.Read();
            return default;
        }

        reader.ReadOf(JsonTokenType.StartObject);

        // Empty object is "Err".
        if (reader.TokenType == JsonTokenType.EndObject)
        {
            reader.Read();
            return default;
        }

        // Read the result content
        var v = ReadResult(ref reader, options);

        reader.ReadOf(JsonTokenType.EndObject);
        return v;
    }

    private Result<T> ReadResult(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        // Object with no property name implies "Ok"
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return Ok(ReadOkValue(ref reader, options));
        }

        // Read exact property name "Ok" or "Err"
        reader.TokenType.ThrowIfNot(JsonTokenType.PropertyName);
        string key = reader.GetKeyString();
        reader.ReadOrThrow();

        if (key.Equals("Ok", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(ReadOkValue(ref reader, options));
        }

        if (key.Equals("Err", StringComparison.OrdinalIgnoreCase))
        {
            return Err<T>(ReadErrValue(ref reader, options));
        }

        ThrowHelper.ThrowJsonException($"Unexpected property key '{key}', expected 'Ok' or 'Err'.");
        return default;
    }

    private T ReadOkValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<T>();
        return converter.Read(ref reader, typeof(T), options)!;
    }

    private Exception ReadErrValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<Exception>();
        return converter.Read(ref reader, typeof(Exception), options)!;
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.TryOk(out var ok))
        {
            writer.WritePropertyName("Ok");
            var converter = options.GetConverter<T>();
            converter.Write(writer, ok, options);
        }

        if (value.TryErr(out var err))
        {
            writer.WritePropertyName("Err");
            var converter = options.GetConverter<Exception>();
            converter.Write(writer, err, options);
        }

        writer.WriteEndObject();
    }
}
