﻿using System.Text.Json;
using Tenon.Serialization;
using SystemJsonSerializer = System.Text.Json.JsonSerializer;

namespace Tenon.JsonSerialization;

public sealed class JsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null) : ISerializer
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOption;
    private readonly JsonSerializerOptions _jsonSerializerOption = jsonSerializerOptions ?? DefaultJsonSerializerOption;

    static JsonSerializer()
    {
        DefaultJsonSerializerOption = new JsonSerializerOptions
        {
            Converters =
            {
                new DateTimeConverter(),
                new DateTimeNullableConverter()
            },
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public byte[] Serialize<T>(T value)
    {
        return SystemJsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOption);
    }

    public string SerializeObject(object value)
    {
        return SystemJsonSerializer.Serialize(value, _jsonSerializerOption);
    }

    public T Deserialize<T>(byte[] bytes)
    {
        return SystemJsonSerializer.Deserialize<T>(bytes, _jsonSerializerOption);
    }

    public T DeserializeObject<T>(string value)
    {
        return SystemJsonSerializer.Deserialize<T>(value, _jsonSerializerOption);
    }
}