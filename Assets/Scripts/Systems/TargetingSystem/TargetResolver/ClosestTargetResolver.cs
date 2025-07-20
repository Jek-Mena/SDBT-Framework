using AI.BehaviorTree.Runtime.Context;
using Systems.TargetingSystem;
using UnityEngine;

public class ClosestTargetResolver : ITargetResolver
{
    public Transform ResolveTarget(GameObject self, TargetingData data, BtContext context)
    {
        var candidates = GameObject.FindGameObjectsWithTag(data.TargetTag);
        Transform closest = null;
        var minDist = float.MaxValue;

        foreach (var candidate in candidates)
        {
            if (candidate == self) continue;
            var dist = Vector3.Distance(self.transform.position, candidate.transform.position);
            
            if (dist < minDist && dist <= data.MaxRange)
            {
                minDist = dist;
                closest = candidate.transform;
            }
        }
        return closest;
    }
}