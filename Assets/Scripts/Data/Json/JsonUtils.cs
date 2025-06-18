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
using System.Linq;
using UnityEngine;

/// <summary>
/// Centralized helpers for safely extracting and validating JSON keys from JObject configs.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Retrieves a required string value from a JObject.
    /// Throws if the key string is missing or value is null/empty.
    /// </summary>
    public static string RequireString(JObject config, string key, string context)
    {
        var value = config.Value<string>(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception($"[JsonUtils] Required key '{key}' missing or empty in context: {context}");

        return value;
    }

    /// <summary>
    /// Retrieves a required int value from a JObject.
    /// Throws if the key is missing or not parseable as an int.
    /// </summary>
    public static int RequireInt(JObject config, string key, string context)
    {
        if (!config.TryGetValue(key, out var token))
            throw new Exception($"[JsonUtils] Required int key '{key}' not found in context: {context}");

        var result = token.Value<int?>();

        if (result == null)
            throw new Exception($"[JsonUtils] Key '{key}' could not be parsed as int in context: {context}");

        return result.Value;
    }

    /// <summary>
    /// Retrieves an integer value from a <see cref="JObject"/> by the specified key.
    /// If the key is not present or the value is not a valid integer, the provided fallback value is returned.
    /// Throws an exception if the key exists but the value is not a valid integer or integer string.
    /// </summary>
    /// <param name="config">The <see cref="JObject"/> to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="fallback">The fallback value to return if the key is not present or the value is invalid.</param>
    /// <param name="context">The context in which this operation is performed, used for error reporting.</param>
    /// <returns>The integer value associated with the specified key, or the fallback value if the key is not present or invalid.</returns>
    /// <exception cref="Exception">
    /// Thrown if the key exists but the value is not a valid integer or integer string.
    /// </exception>
    public static int GetIntOrDefault(JObject config, string key, int fallback, string context)
    {
        if (config == null || !config.TryGetValue(key, out var token)) return fallback;
        
        if (token.Type == JTokenType.Integer)
            return token.Value<int>();
        if (token.Type == JTokenType.String)
        {
            if (int.TryParse(token.ToString(), out var result))
                return result;
            throw new Exception($"[JsonUtils] Key '{key}' present but not a valid integer string in context: {context}");
        }
        throw new Exception($"[JsonUtils] Key '{key}' present but not an integer or integer string in context: {context}");
    }

    public static float RequireFloat(JObject config, string key, string context)
    {
        var token = config[key];
        if (token == null)
            throw new Exception($"[{context}] Required float key '{key}' not found in config.");

        // Defensive: Support int, float, or parseable string
        if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            return token.Value<float>();
        if (token.Type == JTokenType.String && float.TryParse(token.ToString(), out var val))
            return val;

        throw new Exception($"[{context}] Key '{key}' is not a float/int/string. Got {token.Type}: {token}");
    }

    // Like above, but with default
    public static float GetFloatOrDefault(JObject config, string key, float fallback, string context)
    {
        var token = config[key];
        if (token == null)
            return fallback;
        if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            return token.Value<float>();
        if (token.Type == JTokenType.String && float.TryParse(token.ToString(), out var val))
            return val;
        return fallback;
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
    [System.Obsolete] // To use Resolve and rename resolve to GetConfig
    public static JObject GetConfig(JObject jObject, string context)
    {
        var config = jObject[CoreKeys.Config] as JObject ?? jObject;
        if (config == null)
            throw new Exception($"{context} Missing or invalid 'config' block.");
        return config;
    }
    
    /// <summary>
    /// Retrieves a boolean value from the specified JSON object using the provided key. 
    /// If the key is not present, the fallback value is returned. 
    /// If the value is present but not a valid boolean or boolean string, an exception is thrown.
    /// </summary>
    /// <param name="config">The JSON object to retrieve the value from.</param>
    /// <param name="key">The key associated with the boolean value.</param>
    /// <param name="fallback">The default value to return if the key is not found.</param>
    /// <param name="context">The context in which this operation is performed, used for error reporting.</param>
    /// <returns>The boolean value associated with the key, or the fallback value if the key is not found.</returns>
    /// <exception cref="Exception">
    /// Thrown if the key is present but the value is not a valid boolean or boolean string.
    /// </exception>
    public static bool GetBoolOrDefault(JObject config, string key, bool fallback, string context)
    {
        if (!config.TryGetValue(key, out var token)) return fallback;
        
        if (token.Type == JTokenType.Boolean)
            return token.Value<bool>();
        
        if (token.Type == JTokenType.String)
        {
            if (bool.TryParse(token.ToString(), out var result))
                return result;
            throw new Exception($"[JsonUtils] Key '{key}' present but not a valid boolean string in context: {context}");
        }
        throw new Exception($"[JsonUtils] Key '{key}' present but not a boolean or boolean string in context: {context}");
    }

    /*
    • Responsibility: Given a root JSON object and a dot-path string, it navigates the object tree and returns the value at that path.
    • It is a pure helper function.
    • It should NOT know about the blackboard, plugins, or any of your game’s logic.
    • It does NOT recurse a whole tree; it just finds a value at a path.
    • Keep this utility. It’s your core “pointer” mechanic.
     */
    /// <summary>Resolves a dot-path (e.g., "timing.moveDuration") within a JObject tree.</summary>
    public static JToken ResolveDotPath(JObject root, string path, string context)
    {
        var parts = path.Split('.');
        JToken current = root;

        foreach (var part in parts)
        {
            if (current is JObject obj && obj.TryGetValue(part, out var next))
                current = next;
            else
                throw new Exception($"[{context}] Cannot resolve path '{path}' — failed at '{part}'");
        }

        return current;
    }

    /*
    • Responsibility: Recursively traverse a JSON tree (such as a behavior tree node config).
        • Wherever it finds an object of the form { "$ref": "some.path" }, it uses ResolveDotPath on your config to replace that node with the value it points to.
        • It is a tree transformer.
    • It is the “ref expander” that makes all your JSON references work.
    • This is what you call on your config or tree before you hand it off to anything else.
    • Keep this, but make it always use ResolveDotPath.
    • Never use “flat key” blackboard lookups anymore.
     */
    /// <summary>
    /// Recursively walks the given JSON node, replacing all objects of the form { "$ref": "some.path" }
    /// with the resolved value from the BtConfig in the blackboard, using dot-paths.
    ///
    /// [ARCHITECTURE NOTE] Only use RawJson for config-to-blackboard mapping in pipeline/build phase.
    /// All runtime config access should be via blackboard and strongly-typed models.
    /// </summary>
    public static void ResolveRefs(JToken node, BtContext context)
    {
        var blackboard = context.Blackboard;
        // Get config root ONCE for performance; throw if missing.
        var configData = blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin);
        if (configData?.RawJson == null)
            throw new Exception("[ResolveRefs] BtConfig missing or invalid in blackboard. Ensure Plugin/BtConfig runs first!");

        ResolveRefsRecursive(node, configData.RawJson);
    }

    /// <summary>
    /// Determines whether a JToken contains any unresolved references.
    /// Considers a reference unresolved if it contains a key matching CoreKeys.Ref
    /// or has nested tokens that do.
    /// </summary>
    /// <param name="token">The JToken to search for unresolved references.</param>
    /// <returns>True if any unresolved references are found; otherwise, false.</returns>
    public static bool HasUnresolvedRefs(JToken token)
    {
        if (token.Type == JTokenType.Object)
        {
            var obj = (JObject)token;
            if (obj.ContainsKey(CoreKeys.Ref)) return true;

            return obj.Properties().Any(prop => HasUnresolvedRefs(prop.Value));
        }
        else if (token.Type == JTokenType.Array)
        {
            return token.Children().Any(HasUnresolvedRefs);
        }

        return false;
    }
    
    // Internal: recursive helper. DO NOT call this directly outside this file.
    private static void ResolveRefsRecursive(JToken node, JObject configRoot)
    {
        if (node.Type == JTokenType.Object)
        {
            var obj = (JObject)node;
            foreach (var prop in obj.Properties().ToList()) // .ToList() because you may mutate the collection
            {
                if (prop.Value is JObject child && child.TryGetValue(CoreKeys.Ref, out var refKeyToken))
                {
                    var path = refKeyToken.Value<string>();
                    // Use dot-path resolution, always.
                    var value = ResolveDotPath(configRoot, path, "ResolveRefs");
                    // Replace the entire { "$ref": ... } with the resolved value.
                    obj[prop.Name] = value is JToken jt ? jt : JToken.FromObject(value);
                }
                else
                {
                    // Continue recursing for all properties
                    ResolveRefsRecursive(prop.Value, configRoot);
                }
            }
        }
        else if (node.Type == JTokenType.Array)
        {
            foreach (var item in node.Children())
                ResolveRefsRecursive(item, configRoot);
        }
    }
}