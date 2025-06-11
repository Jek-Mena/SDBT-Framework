using UnityEngine;
// MoveNode (Behavior Tree)
// • What it does: Says “Move to that position.”
// • What it should not do: Decide how often to update destination, or whether it's “close enough.”
public class MoveToTargetNode : IBehaviorNode
{
    private readonly MovementData _movementData;
    
    public MoveToTargetNode(MovementData movementData)
    {
        _movementData = movementData;
    }
    
    public BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .Movement()
                .Targeting()
                .Check(out var error))
        {
            Debug.Log(error);
            return BtStatus.Failure;
        }

        var target = context.TargetResolver.ResolveTarget(context.Agent, context.TargetingData);
        context.Movement.ApplySettings(_movementData);
        var canMove = context.Movement.TryMoveTo(target.position);

        return canMove
            ? context.Movement.IsAtDestination() ? BtStatus.Success : BtStatus.Running
            : BtStatus.Failure;
    }
}