using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JTokenExtensions
{
    // Fail fast and loud or waste hours on silent misbehavior.
    public static bool In(this JTokenType type, params JTokenType[] types)
        => System.Array.Exists(types, t => t == type);
    public static bool TryGetBool(this JObject obj, string key, bool defaultValue = false)
    {
        if (!obj.TryGetValue(key, out var token))
            return defaultValue;

        if (token.Type == JTokenType.Boolean)
            return token.Value<bool>();

        if (token.Type == JTokenType.String)
        {
            // Handle "true"/"false" strings just in case
            if (bool.TryParse(token.Value<string>(), out var result))
                return result;
        }

        Debug.LogWarning($"[JTokenExtensions] Expected boolean for key '{key}', got '{token}' ({token.Type}). Defaulting to {defaultValue}");
        return defaultValue;
    }

    public static float RequireFloat(this JObject obj, string key)
    {
        if (!obj.TryGetValue(key, out var token) || !token.Type.In(JTokenType.Float, JTokenType.Integer))
            throw new System.Exception($"Missing or invalid float value for key '{key}' in: {obj}");
        return token.Value<float>();
    }

    public static void RequireKey(this JObject obj, string key)
    {
        if (!obj.ContainsKey(key))
            throw new System.Exception($"Missing required key '{key}' in: {obj}");
    }

    public static bool HasKey(this JObject obj, string key)
    {
        return obj.ContainsKey(key);
    }

    public static void ValidateRequiredKeys(Type type, JObject config, string pluginName)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in fields)
        {
            if (field.FieldType != typeof(string)) continue;
            
            var key = field.GetValue(null) as string;

            if (config.HasKey(key)) continue;

            Debug.LogWarning($"[{pluginName}] Missing required key: '{key}'");
        }
    }
}