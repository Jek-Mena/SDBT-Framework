using UnityEngine;

public class MoveBehavior : IBehaviorNode
{
    private readonly IMovementBehavior _movement;
    private readonly Transform _target;

    public MoveBehavior(Blackboard bb)
    {
        _movement = bb.Movement;
        _target = bb.Target;
    }

    public BtStatus Tick(BtController controller)
    {
        if (_movement == null || _target == null)
            return BtStatus.Failure;

        if (!_movement.TryMoveTo(_target.position))
            return BtStatus.Failure;

        return _movement.IsAtDestination()
            ? BtStatus.Success
            : BtStatus.Running;
    }
}