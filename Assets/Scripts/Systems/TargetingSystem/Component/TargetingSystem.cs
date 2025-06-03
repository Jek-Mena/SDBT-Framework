using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public Blackboard Blackboard;

    public void SetTargetingData(TargetingData targetingData)
    {
        Blackboard.TargetingData = targetingData;
        Blackboard.TargetResolver = TargetResolverRegistry.TryGetValue(targetingData.Style);
        Blackboard.Target = Blackboard.TargetResolver.ResolveTarget(gameObject, targetingData);
    }

    // Call this whenever you want to refresh target (e.g., each BT tick or on-demand)
    public void RefreshTarget()
    {
        if (Blackboard.TargetingData == null || Blackboard.TargetResolver == null)
            return;
        
        Blackboard.Target = Blackboard.TargetResolver.ResolveTarget(gameObject, Blackboard.TargetingData);
    }
}