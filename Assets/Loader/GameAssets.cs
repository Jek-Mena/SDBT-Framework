using System;
using System.Collections.Generic;
using System.Text;
using AI.BehaviorTree.Core;
using AI.BehaviorTree.Keys;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Loader
{
    /// <summary>
    /// GameAssets is a static registry for all entity definitions, loaded at game boot.
    /// Provides instant lookup of prefab+config by entityId, and ensures config/prefab are always paired.
    /// </summary>
    public static class GameAssets
    {
        private const string ScriptName = nameof(GameAssets); 
        
        // Main registry: entityId -> EntityDefinition (config + prefab)
        private static readonly Dictionary<string, AgentDefinition> EntityDefs = new();

        /// <summary>
        /// Loads all entity configs and prefabs from Resources folders at boot.
        /// Call ONCE before any entity spawns (e.g., from BtBootstrapper or GameManager).
        /// </summary>
        /// <param name="configFolder">Folder under Assets/Resources/ containing entity JSONs.</param>
        /// <param name="prefabFolder">Folder under Resources/ containing prefabs.</param>
        public static void Bootstrap(string configFolder, string prefabFolder)
        {
            Debug.Log($"Bootstrap called at {ScriptName}");
            
            // --- Load configs (must be in Resources as TextAssets) ---
            var textAssets = Resources.LoadAll<TextAsset>(configFolder);

            var configs = new Dictionary<string, JObject>();
            if (configs == null)
                throw new ArgumentNullException(nameof(configs));

            foreach (var textAsset in textAssets)
            {
                try
                {
                    var jObj = JObject.Parse(textAsset.text);
                    var entityId = jObj[BtAgentJsonFields.EntityId]?.ToString();
                    if (!string.IsNullOrWhiteSpace(entityId))
                        configs[entityId] = jObj;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{ScriptName}] Failed to parse config '{textAsset.name}': {ex}");
                }
            }
        
            // --- Load prefabs ---
            var prefabObjs = Resources.LoadAll<GameObject>(prefabFolder);
            var prefabs = new Dictionary<string, GameObject>();
            foreach (var prefab in prefabObjs)
            {
                // Use the prefab's name as the key (should match entityId or be referenced in config)
                prefabs[prefab.name] = prefab;
            }
        
            // --- Build EntityDefinition registry ---
            EntityDefs.Clear();
            foreach (var entityId in configs.Keys)
            {
                // Find prefab by path or by entityId
                var prefabPath = configs[entityId][BtAgentJsonFields.Prefab]?.ToString();
                GameObject prefab = null;

                if (!string.IsNullOrEmpty(prefabPath))
                {
                    // Try Resources.Load by path (excluding 'Assets/Resources/')
                    prefab = Resources.Load<GameObject>(prefabPath);
                }
                if (!prefab && prefabs.ContainsKey(entityId))
                {
                    prefab = prefabs[entityId];
                }

                if (!prefab)
                {
                    Debug.LogWarning($"[{ScriptName}] Prefab not found for entityId '{entityId}'.");
                    continue;
                }

                var def = new AgentDefinition
                {
                    EntityId = entityId,
                    Prefab = prefab,
                    Config = configs[entityId]
                };
                EntityDefs[entityId] = def;
            }

            // --- Debug summary ---
            var logBuilder = new StringBuilder();
            foreach (var kvp in EntityDefs)
            {
                var entityId = kvp.Key;
                var def = kvp.Value;
                var prefabName = def.Prefab ? def.Prefab.name : "null";
                var configJson = def.Config != null ?
                    def.Config.ToString(Newtonsoft.Json.Formatting.Indented) 
                    : "null";
            
                logBuilder.AppendLine("ID:     " + entityId);
                logBuilder.AppendLine("Prefab: " + prefabName);
                logBuilder.AppendLine("Config:");
                logBuilder.AppendLine(configJson);
                logBuilder.AppendLine("------------------------------------");
            }
            Debug.Log($"[{ScriptName}] Bootstrap complete. Total Registered Entities: {EntityDefs.Count}\n {logBuilder}");
        }
    
        /// <summary>
        /// Get the EntityDefinition (config + prefab) by entityId.
        /// </summary>
        public static AgentDefinition GetEntity(string entityId)
        {
            if (EntityDefs.TryGetValue(entityId, out var def))
                return def;

            Debug.LogError($"[{ScriptName}] Entity not found: {entityId}");
            return null;
        }
    
        /// <summary>
        /// Returns all registered entity IDs (useful for editors/debug/AI Director).
        /// </summary>
        public static IEnumerable<string> AllEntityIds => EntityDefs.Keys;
    }
}
