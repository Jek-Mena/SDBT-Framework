using UnityEngine;

namespace AI.BehaviorTree.Core
{
    /// <summary>
    /// A MonoBehaviour class that serves as a container for runtime data associated with an entity.
    /// It bridges runtime behavior and the static configuration provided by an EntityDefinition.
    /// </summary>
    public class EntityRuntimeData : MonoBehaviour
    {
        public EntityDefinition Definition;    
    }
}