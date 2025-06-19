using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// GameAssets is a static registry for all entity definitions, loaded at game boot.
/// Provides instant lookup of prefab+config by entityId, and ensures config/prefab are always paired.
/// </summary>
public static class GameAssets
{
    private const string ScriptName = nameof(GameAssets); 
        
    // Main registry: entityId -> EntityDefinition (config + prefab)
    private static readonly Dictionary<string, EntityDefinition> _entityDefs = new();

    /// <summary>
    /// Loads all entity configs and prefabs from Resources folders at boot.
    /// Call ONCE before any entity spawns (e.g., from BtBootstrapper or GameManager).
    /// </summary>
    /// <param name="configFolder">Folder under Assets/Resources/ containing entity JSONs.</param>
    /// <param name="prefabFolder">Folder under Resources/ containing prefabs.</param>
    public static void Bootstrap(string configFolder, string prefabFolder)
    {
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
                var entityId = jObj[CoreKeys.EntityId]?.ToString();
                if (!string.IsNullOrWhiteSpace(entityId))
                    configs[entityId] = jObj;
            }
            catch (System.Exception ex)
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
        _entityDefs.Clear();
        foreach (var entityId in configs.Keys)
        {
            // Find prefab by path or by entityId
            var prefabPath = configs[entityId][CoreKeys.Prefab]?.ToString();
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

            var def = new EntityDefinition
            {
                EntityId = entityId,
                Prefab = prefab,
                Config = configs[entityId]
            };
            _entityDefs[entityId] = def;
        }
        Debug.Log($"[{ScriptName}] Bootstrap complete. Registered {_entityDefs.Count} entities.");
    }
    
    /// <summary>
    /// Get the EntityDefinition (config + prefab) by entityId.
    /// </summary>
    public static EntityDefinition GetEntity(string entityId)
    {
        if (_entityDefs.TryGetValue(entityId, out var def))
            return def;

        Debug.LogError($"[{ScriptName}] Entity not found: {entityId}");
        return null;
    }
    
    /// <summary>
    /// Returns all registered entity IDs (useful for editors/debug/AI Director).
    /// </summary>
    public static IEnumerable<string> AllEntityIds => _entityDefs.Keys;
}
