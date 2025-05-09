using System;

public class ConditionNode : IBehaviorNode
{
    private readonly Func<BtController, bool> _condition;
    private readonly IBehaviorNode _child;

    public ConditionNode(Func<BtController, bool> condition, IBehaviorNode child)
    {
        _condition = condition;
        _child = child;
    }

    public BtStatus Tick(BtController controller)
    {
        return _condition.Invoke(controller) ? _child.Tick(controller) : BtStatus.Failure;
    }
}