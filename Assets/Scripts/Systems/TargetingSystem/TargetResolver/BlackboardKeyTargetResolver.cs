using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace Systems.TargetingSystem.TargetResolver
{
    public class BlackboardKeyTargetResolver : ITargetResolver
    {
        public Transform ResolveTarget(GameObject self, TargetingData data, BtContext context)
        {
            if (string.IsNullOrEmpty(data.BlackboardKey))
            {
                Debug.LogError("[BlackboardKeyTargetResolver] No blackboard key specified.");
                return null;
            }
            
            var value = context.Blackboard.Get<object>(data.BlackboardKey);
            switch (value)
            {
                case Transform transform:
                    return transform;
                case GameObject go:
                    return go.transform;
                default:
                    Debug.LogError($"[BlackboardKeyTargetResolver] Blackboard key {data.BlackboardKey} is not a Transform or GameObject.");
                    return null;
            }
        }
    }
}