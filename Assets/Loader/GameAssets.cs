using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameAssets
{
    private static Dictionary<string, JObject> _entityConfigs = new();
    private static Dictionary<string, GameObject> _entityPrefabs = new();

    // Should be called ONCE on startup (Bootstrapper or before any spawns)
    public static void Bootstrap()
    {
        // Example for Resources-based loading; adapt as needed
        // Here, let's say all configs are in Resources/Data/Units/...
        var configFiles = Resources.LoadAll<TextAsset>("Data/Units");
        foreach (var file in configFiles)
        {
            var jo = JObject.Parse(file.text);
            var id = jo[CoreKeys.EntityId]?.ToString();
            if (id != null) 
                _entityConfigs[id] = jo;
        }
        
        // For each config, load the prefab path it references (prefabs under Resources)
        foreach (var (id, config) in _entityConfigs)
        {
            var prefabPath = config[CoreKeys.Prefab]?.ToString();
            if (!string.IsNullOrWhiteSpace(prefabPath))
            {
                var prefab = Resources.Load<GameObject>(prefabPath);
                if (prefab)
                    _entityPrefabs[id] = prefab;
            }
        }
        Debug.Log($"[GameAssets] Loaded {_entityConfigs.Count} configs and {_entityPrefabs.Count} prefabs.");
        
        // Load all entity configs (JSON files)
        _entityConfigs = new();
        
        // Load all entity prefabs (prefabs)
        _entityPrefabs = new();
    }
    
    public static JObject GetEntityConfig(string entityId)
    {
        if (_entityConfigs.TryGetValue(entityId, out var config)) return config;
        throw new KeyNotFoundException($"Entity config not found: {entityId}");
    }

    public static GameObject GetPrefab(string entityId)
    {
        if (_entityPrefabs.TryGetValue(entityId, out var prefab)) return prefab;
        throw new KeyNotFoundException($"Prefab not found for entity: {entityId}");
    }
}
