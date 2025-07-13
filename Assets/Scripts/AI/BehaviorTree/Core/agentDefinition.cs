using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AI.BehaviorTree.Core
{
    /// <summary>
    /// The data structure for each registered entity, which represents the configuration details and associated data for a specific entity within a system.
    /// Combines prefab and config for each registered entity.
    /// 
    /// Extend with extra fields if needed (BT path, icon, default stats, etc).
    /// </summary>
    public class AgentDefinition
    {
        /// <summary> Unique identifier for this entity config. Example: "enemy_standard_chaser" </summary>
        public string EntityId;
        /// <summary> The prefab asset this entity should instantiate. </summary>
        public GameObject Prefab;
        /// <summary> Configuration data for the entity, typically defined as a JSON object. </summary>
        public JObject Config;
    }
}