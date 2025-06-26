// NOTE: ImpulseMoverNode is NOT currently in use and properly implemented.
// Upgraded for debug overlay/traversal consistency.

using System.Collections.Generic;

public class ImpulseMoverNode : IBehaviorNode
{
    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Movement.ImpulseMover;
    public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

    public BtStatus Tick(BtContext context)
    {
        var blackBoard = context.Blackboard;

        if (blackBoard.ImpulseLogic == null)
        {
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        if (blackBoard.ImpulseLogic.IsImpulsing())
        {
            _lastStatus = blackBoard.ImpulseLogic.IsImpulseComplete() ? BtStatus.Success : BtStatus.Running;
            return _lastStatus;
        }

        _lastStatus = blackBoard.ImpulseLogic.TryImpulse() ? BtStatus.Running : BtStatus.Failure;
        return _lastStatus;
    }
}