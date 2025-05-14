// TODO [Refactor: Consolidate JSON Utilities]
// Consolidate JTokenExtensions.cs and JsonUtils.cs into a single, coherent JSON utility system.
// Retain only the most robust and context-aware methods; remove redundant or duplicate logic.

// === Rename Target ===
// Target class name: JsonUtils (keep consistent with current usage)
// Move any generic extension methods (like 'In') to a new static class, e.g., TypeExtensions or GeneralUtils.

// === Keep (Move to JsonUtils.cs) ===
// - RequireKey(JObject config, string key, string context)
// - RequireFloat(JObject config, string key, string context)
// - RequireBool(JObject config, string key, string context)
// - ValidateKeysExist(JObject config, params string[] keys)
// - WarnIfMissing(JObject config, string context, params string[] keys)
// - GetConfig(JObject json, string context)
// - TryGetBool(this JObject obj, string key, bool defaultValue = false) [add to JsonUtils.cs as extension if fallback is needed]

// === Keep (but move elsewhere) ===
// - In(this JTokenType type, params JTokenType[] types) ? Move to a general-purpose extension class (e.g., TypeExtensions)

// === Drop (Redundant or Weak Variants) ===
// - RequireFloat(this JObject obj, string key) ? Replaced by JsonUtils.RequireFloat with context
// - RequireKey(this JObject obj, string key) ? Replaced by JsonUtils.RequireKey with context
// - HasKey(this JObject obj, string key) ? Redundant, use obj.ContainsKey directly
// - ValidateRequiredKeys(Type type, JObject config, string pluginName)
//      ? Drop unless you're relying on static string fields as schemas. If so, document clearly.

// === Optional Enhancements ===
// - Unify all "Require" methods to follow the same signature: (JObject, string key, string context)
// - Consider adding TryGetXXX variants (like TryGetFloat) if you need lenient parsing paths
// - Ensure all thrown exceptions include context-aware error messages for traceability

using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

/// <summary>
/// Centralized helpers for safely extracting and validating JSON keys from JObject configs.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Retrieves a required string value from a JObject.
    /// Throws if the key is missing or value is null/empty.
    /// </summary>
    public static string RequireKey(JObject config, string key, string context)
    {
        var value = config.Value<string>(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception($"[JsonUtils] Required key '{key}' missing or empty in context: {context}");

        return value;
    }

    /// <summary>
    /// Retrieves a required float value from a JObject.
    /// Throws if the key is missing or not parseable as a float.
    /// </summary>
    public static float RequireFloat(JObject config, string key, string context)
    {
        if (!config.TryGetValue(key, out var token))
            throw new Exception($"[JsonUtils] Required float key '{key}' not found in context: {context}");

        var result = token.Value<float?>();

        if (result == null)
            throw new Exception($"[JsonUtils] Key '{key}' could not be parsed as float in context: {context}");

        return result.Value;
    }

    /// <summary>
    /// Retrieves a required bool value from a JObject.
    /// Throws if the key is missing or not parseable as a bool.
    /// </summary>
    public static bool RequireBool(JObject config, string key, string context)
    {
        if (!config.TryGetValue(key, out var token))
            throw new Exception($"[JsonUtils] Required bool key '{key}' not found in context: {context}");

        var result = token.Value<bool?>();

        if (result == null)
            throw new Exception($"[JsonUtils] Key '{key}' could not be parsed as bool in context: {context}");

        return result.Value;
    }

    /// <summary>
    /// Throws if any of the listed keys are missing from the JObject.
    /// Used for schema completeness checks.
    /// </summary>
    public static void ValidateKeysExist(JObject config, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (!config.ContainsKey(key))
                throw new Exception($"[JsonUtils] Required key '{key}' not found in config block.");
        }
    }

    /// <summary>
    /// Logs a warning if any keys are missing — for soft validation (dev/debug mode).
    /// </summary>
    public static void WarnIfMissing(JObject config, string context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (!config.ContainsKey(key))
            {
                Debug.LogWarning($"[JsonUtils] Optional key '{key}' missing in context: {context}");
            }
        }
    }

    /// <summary>
    /// Retrieves the 'config' block from the JSON object or throws if missing — for strict validation during node construction.
    /// </summary>
    public static JObject GetConfig(JObject json, string context)
    {
        var config = json[CoreKeys.Config] as JObject ?? json;
        if (config == null)
            throw new Exception($"{context} Missing or invalid 'config' block.");
        return config;
    }
}