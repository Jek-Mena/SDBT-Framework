using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    
    private string entityId = "enemy_standard_dasher";

    private void Awake() => PluginRegistry.RegisterAll();

    private void Start()
    {
        var loader = new JsonEntityLoader();
        var data = loader.Load(entityId);

        var prefab = Resources.Load<GameObject>(data.prefab);
        if (!prefab)
        {
            Debug.LogError($"Prefab not found: {data.prefab}");
            return;
        }

        var entity = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        foreach (var entry in data.components)
        {
            var key = entry.type;

            if (!PluginRegistry.TryGet(key, out var plugin))
            {
                Debug.LogError($"Plugin not found for: {key}");
                continue;
            }

            plugin.Apply(entity, entry.@params);
        }

        Debug.Log($"Spawned: {entityId}");
    }
}