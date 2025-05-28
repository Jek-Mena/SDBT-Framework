using Newtonsoft.Json.Linq;
using System.Linq;

public static class BtSchemaUtils
{
    public static void ValidateField(JObject obj, string field, JTokenType expectedType, string path, BtJsonValidator.ValidationResult result)
    {
        if (!obj.TryGetValue(field, out var token))
        {
            result.Errors.Add($"[{path}] Missing field '{field}'");
        }
        else if (token.Type != expectedType)
        {
            result.Errors.Add($"[{path}] Field '{field}' should be {expectedType}, got {token.Type}");
        }
    }
    
    public static void ValidateField(JObject obj, string field, JTokenType[] expectedTypes, string path, BtJsonValidator.ValidationResult result)
    {
        if (!obj.TryGetValue(field, out var token))
        {
            result.Errors.Add($"[{path}] Missing field '{field}'");
        }
        else if (!expectedTypes.Contains(token.Type))
        {
            var expectedList = string.Join(", ", expectedTypes.Select(t => t.ToString()));
            result.Errors.Add($"[{path}] Field '{field}' should be one of [{expectedList}], got {token.Type}");
        }
    }

    // Helper method to fallback if schema default is null
    public static object GetDefaultForType(JTokenType type)
    {
        return type switch
        {
            JTokenType.String => "",
            JTokenType.Float => 0f,
            JTokenType.Integer => 0,
            JTokenType.Boolean => false,
            _ => null
        };
    }
}