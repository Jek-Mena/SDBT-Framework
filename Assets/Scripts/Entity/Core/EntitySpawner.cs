using AI.BehaviorTree.Core;
using AI.BehaviorTree.Registry.List;
using AI.BehaviorTree.Runtime.Context;
using Loader;
using UnityEngine;
using Utils.Component;

namespace Entity.Core
{
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
            var runtimeData = agent.RequireComponent<AgentRuntimeData>();
            runtimeData.Definition = def;
        
            // Build context and assign Behavior Tree
            var blackboardBuilder = new BtContextBuilder();
            ContextModuleRegistrationList.RegisterAll(blackboardBuilder);
        
            // Build context (pure, no wiring up)
            var context = blackboardBuilder.BuildContext(agent);
        
            var controller = context.Controller;
            if (!controller)
            {
                Debug.LogError($"[{ScriptName}] BtController missing from '{agent.name}'");
                return agent;       
            }
        
            // Explicitly wire context to controller
            controller.Initialize(context);
        
            return agent;
        }
   
    }
}