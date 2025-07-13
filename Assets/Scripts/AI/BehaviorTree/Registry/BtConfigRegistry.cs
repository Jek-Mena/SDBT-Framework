using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AI.BehaviorTree.Registry
{
    public static class BtConfigRegistry
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

        public static IEnumerable<string> GetAllKeys() => BtTemplates.Keys;
    
        /// <summary>
        /// Retrieves a behavior tree template from the registry.
        /// Throws if the template is not found.
        /// </summary>
        /// <param name="key">The unique key for the behavior tree template.</param>
        /// <returns>The JSON object representing the behavior tree template.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the template is not found.</exception>
        public static JObject RequireTemplate(string key)
        {
            if (!BtTemplates.TryGetValue(key, out var template))
                throw new KeyNotFoundException($"Behavior tree template '{key}' not found in registry.");
            return template;
        }
        
        /// <summary>
        /// Retrieves the 'root' node object from a registered behavior tree template.
        /// Throws if the template is missing, or if the 'root' or its 'type' property is missing.
        /// </summary>
        /// <param name="key">The unique key for the behavior tree template.</param>
        /// <returns>The JSON object representing the root node.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the template is not found.</exception>
        /// <exception cref="Exception">
        /// Thrown if the 'root' property or the root node's 'type' property is missing in the template.
        /// </exception>
        public static JObject RequireRootNode(string key)
        {
            var template = RequireTemplate(key);
            var root = template["root"];
            if (root == null)
                throw new Exception($"Behavior tree template '{key}' is missing its 'root' property! Check your asset file format.");
            if (root["type"] == null)
                throw new Exception($"Behavior tree root node for '{key}' has no 'type' property! JSON: {root}");
            return (JObject)root;
        }
        
        /// <summary>
        /// Utility for tests/debug: clear the registry (for test teardown, not for game usage).
        /// </summary>
        public static void Clear()
        {
            BtTemplates.Clear();
        }
    }
}