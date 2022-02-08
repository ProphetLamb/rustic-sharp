using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rustic.Json;

public static class JsonExtensions
{
    /// <summary>Configures the <see cref="JsonSerializerOptions"/> for <see cref="Option{T}"/> structures.</summary>
    /// <typeparam name="T">The type of the Option value.</typeparam>
    public static JsonSerializerOptions ConfigureOptions<T>(this JsonSerializerOptions self)
    {
        self.Converters.Add(new OptionConverter<T>());
        return self;
    }

    /// <summary>Configures the <see cref="JsonSerializerOptions"/> for <see cref="Result{T}"/> and <see cref="Result{T,E}"/> structures.</summary>
    /// <typeparam name="T">The type of the Result.Ok value.</typeparam>
    /// <typeparam name="E">The type of the Result.Err value.</typeparam>
    public static JsonSerializerOptions ConfigureResult<T, E>(this JsonSerializerOptions self)
    {
        self.Converters.Add(new ResultConverter<T, E>());
        self.Converters.Add(new ResultConverter<T>());
        return self;
    }

    /// <summary>Attempts to enumerate the objects in the <see cref="JsonElement"/>.</summary>
    public static bool TryEnumerateObject(in this JsonElement self, [NotNullWhen(true)] out JsonElement.ObjectEnumerator enumerator)
    {
        try
        {
            enumerator = self.EnumerateObject();
        }
        catch (InvalidOperationException)
        {
            enumerator = default;
            return false;
        }
        return true;
    }

    /// <summary>Attempts to get the <see cref="JsonElement"/> as a string.</summary>
    public static bool TryGetString(in this JsonElement self, [NotNullWhen(true)] out string? value)
    {
        try
        {
            value = self.GetString();
        }
        catch (InvalidOperationException)
        {
            value = null;
        }
        return value is not null;
    }

    #region JsonReader & Writer

    /// <summary>Attempts to read from the reader; otherwise, throws a <see cref="JsonException"/>.</summary>
    public static void ReadOrThrow(ref this Utf8JsonReader self)
    {
        if (!self.Read())
        {
            ThrowHelper.ThrowJsonException($"Unable to read from the {nameof(Utf8JsonReader)}");
        }
    }

    /// <summary>Throws if the <see cref="JsonTokenType"/> is not <paramref name="expected"/>. Reads from the <see cref="Utf8JsonReader"/>.</summary>
    public static bool ReadOf(ref this Utf8JsonReader self, JsonTokenType expected)
    {
        self.TokenType.ThrowIfNot(expected);
        return self.Read();
    }

    /// <summary>Returns the converter for the type.</summary>
    /// <typeparam name="T">The type of the Option value.</typeparam>
    public static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions self)
    {
        return (JsonConverter<T>)self.GetConverter(typeof(T));
    }

    /// <summary>Returns the string from the reader, throws if the string is null or empty.</summary>
    public static string GetKeyString(ref this Utf8JsonReader reader)
    {
        var value = reader.GetString();
        if (String.IsNullOrWhiteSpace(value))
        {
            ThrowHelper.ThrowJsonException($"Unable to obtain the string key value. The value '{value}' is null or whitespace.");
        }
        return value!;
    }

    /// <summary>Attempts to read from the reader using the converter.</summary>
    /// <typeparam name="T">The type of the Option value.</typeparam>
    public static bool TryRead<T>(ref this Utf8JsonReader self, JsonConverter<T> converter, JsonSerializerOptions options, [NotNullWhen(true)] out T value)
    {
        if (!converter.CanConvert(typeof(T)))
        {
            value = default!;
            return false;
        }
        try
        {
            var temp = converter.Read(ref self, typeof(T), options);
            if (temp is not null)
            {
                value = temp;
                return true;
            }
        }
        catch (JsonException)
        {
        }
        value = default!;
        return false;
    }

    /// <summary>Attempts to write to the value to the writer using the converter.</summary>
    /// <typeparam name="T">The type of the Option value.</typeparam>
    public static bool TryWrite<T>(this Utf8JsonWriter self, T value, JsonConverter<T> converter, JsonSerializerOptions options)
    {
        if (!converter.CanConvert(typeof(T)))
        {
            return false;
        }
        try
        {
            converter.Write(self, value, options);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }


    #endregion JsonReader & Writer

    #region JsonTokenType

    /// <summary>Throws if the <see cref="JsonTokenType"/> is not the <paramref name="expected"/>.</summary>
    public static void ThrowIfNot(this JsonTokenType self, JsonTokenType expected)
    {
        if (self != expected)
        {
            ThrowHelper.ThrowJsonUnexpectedTokenException(expected, self);
        }
    }

    /// <summary>Throws if the <see cref="JsonTokenType"/> is not the <paramref name="expected"/> or <paramref name="expected2"/>.</summary>
    public static void ThrowIfNot(this JsonTokenType self, JsonTokenType expected, JsonTokenType expected2)
    {
        if (self != expected && self != expected2)
        {
            ThrowHelper.ThrowJsonUnexpectedTokenException(expected, self);
        }
    }

    /// <summary>Throws if the <see cref="JsonTokenType"/> is not a leaf (<see cref="IsLeaf"/>).</summary>
    public static void ThrowIfNotLeaf(this JsonTokenType self)
    {
        if (!self.IsLeaf())
        {
            ThrowHelper.ThrowJsonException($"Expected JsonTokenType to be a leaf, but was {self}");
        }
    }

    /// <summary>Returns <c>false</c> if the <see cref="JsonTokenType"/> is StartObject, EndObject, StartArray, EndArray or PropertyName; otherwise, return <c>true</c>.</summary>
    public static bool IsLeaf(this JsonTokenType self)
    {
        return self switch
        {
            JsonTokenType.StartObject or JsonTokenType.EndObject or JsonTokenType.StartArray or JsonTokenType.EndArray or JsonTokenType.PropertyName => false,
            _ => true,
        };
    }

    /// <summary>Returns whether the <see cref="JsonTokenType"/> is <see cref="JsonTokenType.Null"/> or <see cref="JsonTokenType.None"/>.</summary>
    public static bool IsNullOrNone(this JsonTokenType self)
    {
        return self is JsonTokenType.Null or JsonTokenType.None;
    }

    #endregion JsonTokenType
}
