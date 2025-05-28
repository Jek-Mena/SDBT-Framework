using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class BtNodeschemaParser
{
    public static Dictionary<string, BtNodeSchemaField> ParseFields(JObject schemaJson)
    {
        var fields = new Dictionary<string, BtNodeSchemaField>();

        foreach (var prop in schemaJson.Properties())
        {
            var field = new BtNodeSchemaField
            {
                Key = prop.Name,
                JsonType = ParseType(prop.Value[SchemaFieldKeys.Type]?.ToString()),
                DefaultValue = prop.Value[SchemaFieldKeys.Default],
                Description = prop.Value[SchemaFieldKeys.Description]?.ToString(),
                Min = prop.Value[SchemaFieldKeys.Min]?.Value<float>(),
                Max = prop.Value[SchemaFieldKeys.Max]?.Value<float>(),
                UiTypeHint = prop.Value[SchemaFieldKeys.UiType]?.ToString(),
                IsRequired = prop.Value[SchemaFieldKeys.Required]?.Value<bool>() ?? false
            };

            if (prop.Value[SchemaFieldKeys.Enum] is JArray enumArray)
            {
                field.EnumValues = enumArray.ToObject<List<string>>();
            }

            fields[prop.Name] = field;
        }

        return fields;
    }
    
    private static JTokenType ParseType(string jsonType)
    {
        return jsonType switch
        {
            JTokenTypeKeys.LiteralString => JTokenType.String,
            JTokenTypeKeys.LiteralNumber => JTokenType.Float,
            JTokenTypeKeys.LiteralInteger => JTokenType.Integer,
            JTokenTypeKeys.LiteralBoolean => JTokenType.Boolean,
            JTokenTypeKeys.LiteralObject => JTokenType.Object,
            _ => JTokenType.String
        };
    }
}