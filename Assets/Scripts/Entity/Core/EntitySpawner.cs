using Newtonsoft.Json.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [Tooltip("If set, this spawner always spawns this entity type. Otherwise, call SpawnEntity manually.")]
    [SerializeField] private string defaultEntityId = "enemy_test_agent";
    [SerializeField] private Transform spawnPoint;
    
    private const string ScriptName = nameof(EntitySpawner);
    
    public GameObject SpawnEntity(string entityId, Vector3 position, Quaternion rotation)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            entityId = defaultEntityId;

        if (position == Vector3.zero)
            position = spawnPoint.position;
        
        // Get definition 
        var def = GameAssets.GetEntity(entityId);
        if (def == null)
        {
            Debug.LogError($"[{ScriptName}] No definition for '{entityId}'"); 
            return null;
        }

        Debug.Log($"[EntitySpawner] Spawning '{entityId}'. Config: {def.Config}");
        
        // Instantiate entity
        var agent = Instantiate(def.Prefab, position, rotation);
        agent.name = $"{entityId}-{agent.GetInstanceID()}";
        
        // Assign the definition to the agent
        var runtimeData = agent.RequireComponent<EntityRuntimeData>();
        runtimeData.Definition = def;
        
        // Build context and assign Behavior Tree
        var blackboardBuilder = new BtBlackboardBuilder();
        ContextModuleRegistration.RegisterAll(blackboardBuilder);
        
        // Build context (pure, no wiring up)
        var context = blackboardBuilder.BuildContext(agent);
        
        // Assign BT tree from registry
        var treeToken = def.Config[CoreKeys.Tree]?.ToString();
        if (string.IsNullOrWhiteSpace(treeToken))
        {
            Debug.LogError($"[{ScriptName}] No tree key specified for '{entityId}'.");
            return agent;
        }

        var controller = context.Controller;
        if (!controller)
        {
            Debug.LogError($"[{ScriptName}] BtController missing from '{agent.name}'");
            return agent;       
        }
        
        // Explicitly wire context to controller
        controller.InitContext(context);
        
        // Uses SwitchToTree (will deep-clone and resolve refs using context)
        controller.SwitchToTree(treeToken, "Spawned");
        
        return agent;
    }
   
}