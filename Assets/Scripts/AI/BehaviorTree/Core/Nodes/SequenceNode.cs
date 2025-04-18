using System.Collections.Generic;
using Assets.Scripts.Shared.AI;

public class SequenceNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public SequenceNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtController controller)
    {
        foreach (var child in _children)
        {
            var status = child.Tick(controller);
            if (status != BtStatus.Success)
                return status;
        }
        return BtStatus.Success;
    }
}