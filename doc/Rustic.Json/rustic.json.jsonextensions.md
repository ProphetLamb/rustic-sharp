# JsonExtensions

Namespace: Rustic.Json

Extensions methods for ,  and similar types.

```csharp
public static class JsonExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [JsonExtensions](./rustic.json.jsonextensions.md)

## Methods

### **TryEnumerateObject(JsonElement&, ObjectEnumerator&)**

Attempts to enumerate the objects in the .

```csharp
public static bool TryEnumerateObject(JsonElement& self, ObjectEnumerator& enumerator)
```

#### Parameters

`self` JsonElement&<br>

`enumerator` ObjectEnumerator&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TryGetString(JsonElement&, String&)**

Attempts to get the  as a string.

```csharp
public static bool TryGetString(JsonElement& self, String& value)
```

#### Parameters

`self` JsonElement&<br>

`value` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ReadOrThrow(Utf8JsonReader&)**

Attempts to read from the reader; otherwise, throws a .

```csharp
public static void ReadOrThrow(Utf8JsonReader& self)
```

#### Parameters

`self` Utf8JsonReader&<br>

### **ReadOf(Utf8JsonReader&, JsonTokenType)**

Throws if the  is not . Reads from the .

```csharp
public static bool ReadOf(Utf8JsonReader& self, JsonTokenType expected)
```

#### Parameters

`self` Utf8JsonReader&<br>

`expected` JsonTokenType<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetConverter&lt;T&gt;(JsonSerializerOptions)**

Returns the converter for the type.

```csharp
public static JsonConverter<T> GetConverter<T>(JsonSerializerOptions self)
```

#### Type Parameters

`T`<br>
The type of the Option value.

#### Parameters

`self` JsonSerializerOptions<br>

#### Returns

JsonConverter&lt;T&gt;<br>

### **GetKeyString(Utf8JsonReader&)**

Returns the string from the reader, throws if the string is null or empty.

```csharp
public static string GetKeyString(Utf8JsonReader& reader)
```

#### Parameters

`reader` Utf8JsonReader&<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TryRead&lt;T&gt;(Utf8JsonReader&, JsonConverter&lt;T&gt;, JsonSerializerOptions, T&)**

Attempts to read from the reader using the converter.

```csharp
public static bool TryRead<T>(Utf8JsonReader& self, JsonConverter<T> converter, JsonSerializerOptions options, T& value)
```

#### Type Parameters

`T`<br>
The type of the Option value.

#### Parameters

`self` Utf8JsonReader&<br>

`converter` JsonConverter&lt;T&gt;<br>

`options` JsonSerializerOptions<br>

`value` T&<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TryWrite&lt;T&gt;(Utf8JsonWriter, T, JsonConverter&lt;T&gt;, JsonSerializerOptions)**

Attempts to write to the value to the writer using the converter.

```csharp
public static bool TryWrite<T>(Utf8JsonWriter self, T value, JsonConverter<T> converter, JsonSerializerOptions options)
```

#### Type Parameters

`T`<br>
The type of the Option value.

#### Parameters

`self` Utf8JsonWriter<br>

`value` T<br>

`converter` JsonConverter&lt;T&gt;<br>

`options` JsonSerializerOptions<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ThrowIfNot(JsonTokenType, JsonTokenType)**

Throws if the  is not the .

```csharp
public static void ThrowIfNot(JsonTokenType self, JsonTokenType expected)
```

#### Parameters

`self` JsonTokenType<br>

`expected` JsonTokenType<br>

### **ThrowIfNot(JsonTokenType, JsonTokenType, JsonTokenType)**

Throws if the  is not the  or .

```csharp
public static void ThrowIfNot(JsonTokenType self, JsonTokenType expected, JsonTokenType expected2)
```

#### Parameters

`self` JsonTokenType<br>

`expected` JsonTokenType<br>

`expected2` JsonTokenType<br>

### **ThrowIfNotLeaf(JsonTokenType)**

Throws if the  is not a leaf ([JsonExtensions.IsLeaf(JsonTokenType)](./rustic.json.jsonextensions.md#isleafjsontokentype)).

```csharp
public static void ThrowIfNotLeaf(JsonTokenType self)
```

#### Parameters

`self` JsonTokenType<br>

### **IsLeaf(JsonTokenType)**

Returns `false` if the  is StartObject, EndObject, StartArray, EndArray or PropertyName; otherwise, return `true`.

```csharp
public static bool IsLeaf(JsonTokenType self)
```

#### Parameters

`self` JsonTokenType<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsNullOrNone(JsonTokenType)**

Returns whether the  is  or .

```csharp
public static bool IsNullOrNone(JsonTokenType self)
```

#### Parameters

`self` JsonTokenType<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
