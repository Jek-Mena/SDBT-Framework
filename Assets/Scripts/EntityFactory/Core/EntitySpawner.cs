using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    private const string ScriptName = nameof(EntitySpawner); 
    
    [SerializeField] private Transform spawnPoint;
    
    private string _entityIdToSpawn = "enemy_standard_laghound";
    private int _spawnCount = 1;
    
    private readonly JsonEntityLoader _entityLoader = new JsonEntityLoader();

    // Call this from Start(), Update(), or custom trigger/event
    public void SpawnEntity()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Spawn(_entityIdToSpawn, spawnPoint.position);
        }
    }
    
    public static GameObject Spawn(string entityId, Vector3 spawnPosition)
    {
        var config = GameAssets.GetEntityConfig(entityId);
        var prefab = GameAssets.GetPrefab(entityId);
        
        if (!prefab || config == null)
        {
            Debug.LogError($"[{ScriptName}] Failed to spawn {entityId}: missing prefab/config.");
            return null;
        }
        
        var entity = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Debug.Log($"[{ScriptName}] '{entity.name}' successfully spawned.");

        var controller = entity.RequireComponent<BtController>();
        var blackboard = new Blackboard();
        var builder = new BtBlackboardBuilder(controller, blackboard, entityId);
        
        return entity;
    }
    
    private void Start()
    {
        // 1) Load the entity definition
        var entityData = _entityLoader.Load("Data/Units/Enemies/Standard/enemy_standard_laghound");
        var prefab = Resources.Load<GameObject>(entityData.Prefab);

        if (!prefab)
        {
            Debug.LogError($"Prefab not found for '{entityData.Prefab}'");
            return;
        }
        var entity = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"'{entity.name}' successfully spawned.");

        // 2) Build component array and root JSON for plugins
        var compsArray = new JArray(
            entityData.Components.Select(comp =>
                new JObject
                {
                    [CoreKeys.Plugin] = comp.PluginKey,
                    [CoreKeys.Params] = comp.@params ?? new JObject()
                }
            )
        );
        var rootJson = new JObject { [CoreKeys.Components] = compsArray };

        // 3) Extract BtConfig from pluginlist
        var configComponent = compsArray
            .OfType<JObject>()
            .FirstOrDefault(c => c[CoreKeys.Plugin]?.ToString() == PluginMetaKeys.Core.BtConfig.Plugin);

        if (configComponent == null)
        {
            Debug.LogError("[EntitySpawner] BtConfig plugin not found in components. You must include a Plugin/BtConfig entry.");
            return;
        }

        var configParams = configComponent[CoreKeys.Params] as JObject;
        if (configParams == null)
        {
            Debug.LogError("[EntitySpawner] BtConfig plugin found, but Params block is missing or invalid.");
            return;
        }

        var configData = new ConfigData { RawJson = configParams };

        // 5) Create context builder with injected BtConfig
        var contextBuilder = ContextBuilderFactory.CreateWithBtConfig(configData);

        // 6) Build blackboard + wire runtime context (BtController will receive it)
        contextBuilder.Build(entity);
        
        // 7.) Apply all plugins in the correct order
        BtLoader.ApplyAll(entity, rootJson);
    }
}