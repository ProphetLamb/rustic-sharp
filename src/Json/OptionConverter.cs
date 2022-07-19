using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Rustic;

namespace Rustic.Json;

/// <summary>Converts the <see cref="Option{T}"/> to and from json.</summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class OptionConverter<T> : JsonConverter<Option<T>>
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType.IsNullOrNone())
        {
            reader.Read();
            return default;
        }

        reader.ReadOf(JsonTokenType.StartObject);

        // Empty object is "None".
        if (reader.TokenType == JsonTokenType.EndObject)
        {
            reader.Read();
            return default;
        }

        // Read the option content
        var v = ReadOption(ref reader, options);

        reader.ReadOf(JsonTokenType.EndObject);
        return v;
    }

    private Option<T> ReadOption(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        // Object with no property name implies "Some"
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return ReadInnerValue(ref reader, options);
        }

        // Read exact property name "Some" or "None"
        reader.TokenType.ThrowIfNot(JsonTokenType.PropertyName);
        string key = reader.GetKeyString();
        reader.ReadOrThrow();

        if (key.Equals("Some", StringComparison.OrdinalIgnoreCase))
        {
            return ReadInnerValue(ref reader, options);
        }

        if (key.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            // Graciously allow any leaf to be the "None" value, because its ignored anyways.
            reader.TokenType.ThrowIfNotLeaf();
            reader.Read();
            return default;
        }

        ThrowHelper.ThrowJsonException($"Unexpected property key '{key}', expected 'Some' or 'None'.");
        return default;
    }

    private Option<T> ReadInnerValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = options.GetConverter<T>();
        var v = converter.Read(ref reader, typeof(T), options);
        if (v is null)
        {
            return default;
        }

        return v;
    }

    public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.TrySome(out var inner))
        {
            writer.WritePropertyName("Some");

            var converter = options.GetConverter<T>();
            converter.Write(writer, inner, options);
        }
        else
        {
            writer.WriteNull("None");
        }
        writer.WriteEndObject();
    }
}
