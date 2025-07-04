using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// The data structure for each registered entity, which represents the configuration details and associated data for a specific entity within a system.
/// Combines prefab and config for each registered entity.
/// 
/// Extend with extra fields if needed (BT path, icon, default stats, etc).
/// </summary>
public class EntityDefinition
{
    /// <summary> Unique identifier for this entity config. Example: "enemy_standard_chaser" </summary>
    public string EntityId;
    /// <summary> The prefab asset this entity should instantiate. </summary>
    public GameObject Prefab;
    /// <summary> Configuration data for the entity, typically defined as a JSON object. </summary>
    public JObject Config;
    
    /// <summary>
    /// Agent-global profiles: Fear, Health, etc.
    /// </summary>
    public JObject AgentProfilesBlock => Config[CoreKeys.AgentProfiles] as JObject;
    /// <summary>
    /// Behavior/BT node profiles: Movement, Targeting, Timing, etc.
    /// </summary>
    public JObject BehaviorProfilesBlock => Config[CoreKeys.BehaviorProfiles] as JObject;
    public string BehaviorTreePath => Config[CoreKeys.BehaviorTree]?.ToString();
}