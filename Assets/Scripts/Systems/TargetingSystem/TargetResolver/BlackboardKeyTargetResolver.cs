using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace Systems.TargetingSystem.TargetResolver
{
    
    public class BlackboardKeyTargetResolver : ITargetResolver
    {
        private const string ScriptName = nameof(BlackboardKeyTargetResolver);
        
        public Transform ResolveTarget(GameObject self, TargetingData data, BtContext context)
        {
            if (string.IsNullOrEmpty(data.BlackboardKey))
            {
                Debug.LogError($"[{ScriptName}] No blackboard key specified.");
                return null;
            }
            
            var value = context.Blackboard.Get<object>(data.BlackboardKey);
            
            // Defensive: always null guard first
            if (value == null)
                return null;
            
            if (value is Transform transform) return transform;
            if (value is GameObject go) return go.transform;

            Debug.LogError($"[{ScriptName}] Blackboard key {data.BlackboardKey} is not a Transform or GameObject. " +
                           $"Agent: {context.Agent.name}. Value ({value}) is a type of {value.GetType()}");
            
            return null;
        }
    }
}