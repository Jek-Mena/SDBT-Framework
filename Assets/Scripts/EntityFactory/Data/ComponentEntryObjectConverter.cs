using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ComponentEntryObjectConverter : JsonConverter<List<ComponentEntry>>
{
    public override List<ComponentEntry> ReadJson(JsonReader reader, Type objectType, List<ComponentEntry> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        var list = new List<ComponentEntry>();

        foreach (var token in array)
        {
            if (token.Type != JTokenType.Object)
                throw new JsonSerializationException("Each component must be an object with 'plugin' and 'params'.");

            var obj = (JObject)token;
            var pluginString = obj[CoreKeys.Plugin]?.ToString();
            var @params = obj[CoreKeys.Params] as JObject;

            if (string.IsNullOrWhiteSpace(pluginString))
                throw new JsonSerializationException("Missing or empty plugin key.");

            list.Add(new ComponentEntry(pluginString, @params));
        }

        return list;
    }

    public override void WriteJson(JsonWriter writer, List<ComponentEntry> value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        foreach (var entry in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(CoreKeys.Plugin);
            writer.WriteValue(entry.PluginKey);
            writer.WritePropertyName(CoreKeys.Params);
            serializer.Serialize(writer, entry.@params);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
