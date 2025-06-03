public class ImpulseMoverNode : IBehaviorNode
{
    public BtStatus Tick(BtContext context)
    {
        var blackBoard = context.Blackboard;

        if (blackBoard.ImpulseLogic == null)
            return BtStatus.Failure;

        if (blackBoard.ImpulseLogic.IsImpulsing())
            return blackBoard.ImpulseLogic.IsImpulseComplete() ? BtStatus.Success : BtStatus.Running;

        return blackBoard.ImpulseLogic.TryImpulse() ? BtStatus.Running : BtStatus.Failure;
    }
}