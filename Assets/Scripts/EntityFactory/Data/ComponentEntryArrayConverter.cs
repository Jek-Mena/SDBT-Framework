using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ComponentEntryArrayConverter : JsonConverter<List<ComponentEntry>>
{
    public override List<ComponentEntry> ReadJson(JsonReader reader, Type objectType, List<ComponentEntry> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        var list = new List<ComponentEntry>();

        foreach (var token in array)
        {
            if (token is not JArray entry || entry.Count != 2)
                throw new JsonSerializationException("Each component entry must be an array of [type, params].");

            var typeString = entry[0]?.ToString();
            var @params = entry[1] as JObject;

            if (!Enum.TryParse(typeString, out PluginKey key))
                throw new JsonSerializationException($"Invalid plugin key: {typeString}");

            list.Add(new ComponentEntry(key, @params));
        }

        return list;
    }

    public override void WriteJson(JsonWriter writer, List<ComponentEntry> value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        foreach (var entry in value)
        {
            writer.WriteStartArray();
            writer.WriteValue(entry.type.ToString());
            serializer.Serialize(writer, entry.@params);
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}