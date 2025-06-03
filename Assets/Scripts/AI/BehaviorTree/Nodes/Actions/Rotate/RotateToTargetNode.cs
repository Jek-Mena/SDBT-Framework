using UnityEngine;

public class RotateToTargetNode : IBehaviorNode
{
    private readonly RotationData _rotationData;

    public RotateToTargetNode(RotationData rotationData)
    {
        _rotationData = rotationData;
    }
    
    public BtStatus Tick(BtContext context)
    {
        var controller = context.Controller;
        if (!controller)
        {
            Debug.LogError("[RotateToTargetNode] Controller is missing from BtController. There is an in upstream.");
            return BtStatus.Failure;
        }

        var blackboard = controller.Blackboard;
        if (blackboard  == null)
        {
            Debug.LogError("[MoveToTargetNode] Blackboard is missing from BtController");
            return BtStatus.Failure;
        }
        
        var targetingData = blackboard.TargetingData;
        if (targetingData == null)
        {
            Debug.LogError("[MoveToTargetNode] TargetingData is missing from blackboard.");
            return BtStatus.Failure;
        }
        
        var resolver = blackboard.TargetResolver ?? TargetResolverRegistry.TryGetValue(targetingData.Style);
        var target = resolver.ResolveTarget(controller.gameObject, targetingData);

        if (target == null)
        {
            Debug.LogWarning("[RotateToTargetNode] No valid target in range.");
            return BtStatus.Idle;  // No valid target in range—nothing to look at
        }
        
        var rotationLogic = blackboard.RotationLogic;
        if (rotationLogic == null)
        {
            Debug.LogError("[RotateToTargetNode] rotationLogic: null");
            return BtStatus.Failure;
        }
        
        rotationLogic.ApplySettings(_rotationData);
        var canRotate = rotationLogic.TryRotateTo(target.position);
        
        if (!canRotate)
            return BtStatus.Failure;
        
        return rotationLogic.IsFacingTarget(target.position)
            ? BtStatus.Success
            : BtStatus.Running;
    }
}