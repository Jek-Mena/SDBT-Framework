using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ImpulseMoverNode : IBehaviorNode
{
    public BtStatus Tick(BtController controller)
    {
        var blackBoard = controller.Blackboard;

        if (blackBoard.ImpulseLogic == null)
            return BtStatus.Failure;

        if (blackBoard.ImpulseLogic.IsImpulsing())
            return blackBoard.ImpulseLogic.IsImpulseComplete() ? BtStatus.Success : BtStatus.Running;

        return blackBoard.ImpulseLogic.TryImpulse() ? BtStatus.Running : BtStatus.Failure;
    }
}