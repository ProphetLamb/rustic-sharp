using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Rustic;

using static Rustic.Result;

namespace Rustic.Json;

/// <summary>Converts the <see cref="Result{T,E}"/> to and from json.</summary>
/// <typeparam name="T">The type of the Ok value.</typeparam>
/// <typeparam name="E">The type of the Err value.</typeparam>
public sealed class ResultConverter<T, E> : JsonConverter<Result<T, E>>
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

    private static Result<T, E> ReadResult(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        // Object with no property name implies "Ok"
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return Ok<T, E>(ReadOkValue(ref reader, options));
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

    private static T ReadOkValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<T>();
        return converter.Read(ref reader, typeof(T), options)!;
    }

    private static E ReadErrValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
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
public sealed class ResultConverter<T> : JsonConverter<Result<T>>
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

    private static T ReadOkValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<T>();
        return converter.Read(ref reader, typeof(T), options)!;
    }

    private static Exception ReadErrValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
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
