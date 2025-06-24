using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls all global spawning and despawning. Holds references to all spawners in the scene.
/// </summary>
public class EntityManager : MonoBehaviour
{
    private static EntityManager Instance { get; set; }
    private List<EntitySpawner> Spawners = new();

    private List<GameObject> _activeEntities = new();
    
    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Find all spawners in the scene
        Spawners.AddRange(FindObjectsByType<EntitySpawner>(FindObjectsSortMode.None));
    }
    
    private void Start()
    {
        // Test spawn
        foreach (var spawner in Spawners)
        {
            spawner.SpawnEntity(null, Vector3.zero, Quaternion.identity);
        }
    }
    
    public GameObject Spawn(string entityId, Vector3 position, Quaternion rotation, int spawnerIndex = 0)
    {
        if (Spawners.Count == 0)
        {
            Debug.LogError("No spawners found in scene!");
            return null;
        }

        var sprawner = Spawners[spawnerIndex % Spawners.Count];
        var go = sprawner.SpawnEntity(entityId, position, rotation);
        if (go != null)
            _activeEntities.Add(go);
        return go;
    }
    
    public void Despawn(GameObject entity)
    {
        if (_activeEntities.Contains(entity))
        {
            _activeEntities.Remove(entity);
            Destroy(entity);
        }
    }

    public void DespawnAll()
    {
        foreach (var e in _activeEntities)
            Destroy(e);
        _activeEntities.Clear();
    }
}