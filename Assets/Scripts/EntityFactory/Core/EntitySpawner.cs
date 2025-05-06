using Newtonsoft.Json.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    private readonly JsonEntityLoader _entityLoader = new JsonEntityLoader();

    private void Start()
    {
        // 1) Load the entity definition
        var entityData = _entityLoader.Load("Data/Units/Enemies/Standard/enemy_standard_chaser");
        var prefab = Resources.Load<GameObject>(entityData.prefab);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found for '{entityData.prefab}'");
            return;
        }

        // 2) Instantiate at your designated spawnPoint
        var go = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        if (go == null)
        {
            Debug.LogError($"Failed to spawn '{prefab.name}'.");
            return;
        }
        Debug.Log($"'{go.name}' successfully spawned.");

        // 3) Build a root JObject mapping each component key to its params
        var configRoot = new JObject();
        foreach (var comp in entityData.components)
        {
            var key = comp.Key. ToString();
            var parameters = comp.@params ?? new JObject();
            configRoot.Add(key, parameters);
        }

        // 4) Apply all plugins in the correct order
        BtLoader.ApplyAll(go, configRoot);
    }
}