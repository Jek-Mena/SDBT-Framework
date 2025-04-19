using UnityEngine;

public class MoveNode : IBehaviorNode
{
    public BtStatus Tick(BtController controller)
    {
        var blackBoard = controller.Blackboard;

        if (blackBoard.MovementLogic == null || blackBoard.Target == null)
            return BtStatus.Failure;

        if (blackBoard.MovementLogic.TryMoveTo(blackBoard.Target.position))
            return BtStatus.Failure;

        return blackBoard.MovementLogic.IsAtDestination()
            ? BtStatus.Success
            : BtStatus.Running;
    }
}