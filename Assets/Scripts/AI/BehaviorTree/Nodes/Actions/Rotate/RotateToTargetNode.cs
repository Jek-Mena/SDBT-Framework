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
        if (!BtValidator.Require(context)
                .Rotation()
                .Targeting()
                .Check(out var error)
           )
        {
            Debug.Log(error);
            return BtStatus.Failure;
        }
        
        var targetPos = context.TargetResolver.ResolveTarget(context.Agent, context.TargetingData).position;
        context.Rotation.ApplySettings(_rotationData);
        var canRotate = context.Rotation.TryRotateTo(targetPos);

        return canRotate
            ? context.Rotation.IsFacingTarget(targetPos) ? BtStatus.Success : BtStatus.Running
            : BtStatus.Failure;
    }
}