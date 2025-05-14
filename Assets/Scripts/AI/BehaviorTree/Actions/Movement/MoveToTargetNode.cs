// MoveNode (Behavior Tree)
// • What it does: Says “Move to that position.”
// • What it should not do: Decide how often to update destination, or whether it's “close enough.”
public class MoveToTargetNode : IBehaviorNode
{
    // Intent in -> Tick -> Status out.
    public BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;
        var movementLogic = blackboard.MovementLogic;
        var target = blackboard.Target;

        if (movementLogic == null)
        {
            UnityEngine.Debug.Log($"[MoveToTargetNode] movementLogic: {movementLogic}");
            return BtStatus.Failure;
        }

        if (target == null)
        {
            UnityEngine.Debug.Log($"[MoveToTargetNode] target: {target}");
            return BtStatus.Failure;
        }

        UnityEngine.Debug.Log($"[MoveToTargetNode] Tick — MovementLogic: {movementLogic}, Target: {target}");
        var canMove = blackboard.MovementLogic.TryMoveTo(target.position);

        if (!canMove)
            return BtStatus.Failure;

        return blackboard.MovementLogic.IsAtDestination()
            ? BtStatus.Success
            : BtStatus.Running;
    }
}