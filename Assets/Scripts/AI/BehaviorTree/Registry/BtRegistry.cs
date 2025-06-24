using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class BtRegistry
{
    private static readonly Dictionary<string, JObject> BtTemplates = new();

    /// <summary>
    /// Registers a behavior tree template in the registry.
    /// </summary>
    /// <param name="key">The unique key used to identify the behavior tree template.</param>
    /// <param name="btJson">The JSON object representing the behavior tree template.</param>
    public static void RegisterTemplate(string key, JObject btJson)
    {
        BtTemplates[key] = btJson;
    }

    /// <summary>
    /// Retrieves a behavior tree template from the registry.
    /// </summary>
    /// <param name="key">The unique key used to identify the behavior tree template.</param>
    /// <returns>The JSON object representing the behavior tree template, or null if the key is not found.</returns>
    public static JObject GetTemplate(string key)
    {
        return BtTemplates.GetValueOrDefault(key);
    }
    
    /// <summary>
    /// Utility for tests/debug: clear the registry (for test teardown, not for game usage).
    /// </summary>
    public static void Clear()
    {
        BtTemplates.Clear();
    }
}