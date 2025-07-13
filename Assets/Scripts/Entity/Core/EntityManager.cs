using System.Collections.Generic;
using UnityEngine;

namespace Entity.Core
{
    /// <summary>
    /// Controls all global spawning and despawning. Holds references to all spawners in the scene.
    /// </summary>
    public class EntityManager : MonoBehaviour
    {
        private static EntityManager Instance { get; set; }
        private readonly List<EntitySpawner> _spawners = new();
        private readonly List<GameObject> _activeEntities = new();
    
        private void Awake()
        {
            if (Instance) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        
            // Find all spawners in the scene
            _spawners.AddRange(FindObjectsByType<EntitySpawner>(FindObjectsSortMode.None));
        }
    
        private void Start()
        {
            // Test spawn
            foreach (var spawner in _spawners)
            {
                spawner.SpawnEntity(null, Vector3.zero, Quaternion.identity);
            }
        }
    
        public GameObject Spawn(string entityId, Vector3 position, Quaternion rotation, int spawnerIndex = 0)
        {
            if (_spawners.Count == 0)
            {
                Debug.LogError("No spawners found in scene!");
                return null;
            }

            var spawner = _spawners[spawnerIndex % _spawners.Count];
            var go = spawner.SpawnEntity(entityId, position, rotation);
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
}