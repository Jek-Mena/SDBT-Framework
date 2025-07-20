using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace Systems.TargetingSystem
{
    public interface ITargetResolver
    {
        Transform ResolveTarget(GameObject self, TargetingData data, BtContext context);
    }
}