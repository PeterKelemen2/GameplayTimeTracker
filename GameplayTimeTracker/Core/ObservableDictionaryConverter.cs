using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ObservableDictionaryConverter<TKey, TValue> : JsonConverter<ObservableDictionary<TKey, TValue>>
{
    public override ObservableDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dictionary = new ObservableDictionary<TKey, TValue>();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var key = reader.GetString();
            reader.Read();
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            dictionary.Add((TKey)Convert.ChangeType(key, typeof(TKey)), value);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ObservableDictionary<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key.ToString());
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }
}
