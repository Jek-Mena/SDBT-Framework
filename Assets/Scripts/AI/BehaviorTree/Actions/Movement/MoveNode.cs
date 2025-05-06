// MoveNode (Behavior Tree)
// • What it does: Says “Move to that position.”
// • What it should not do: Decide how often to update destination, or whether it's “close enough.”

using UnityEngine;

public class MoveNode : IBehaviorNode
{
    public BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;
        var movementLogic = blackboard.MovementLogic;
        var target = blackboard.Target;

        if (movementLogic == null || target == null)
            return BtStatus.Failure;

        var canMove = blackboard.MovementLogic.TryMoveTo(target.position);

        if (!canMove)
            return BtStatus.Failure;

        return blackboard.MovementLogic.IsAtDestination()
            ? BtStatus.Success
            : BtStatus.Running;
    }
}