using System.Collections.Generic;
using Assets.Scripts.Shared.AI;

public class ParallelNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public ParallelNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtController controller)
    {
        var allSucceeded = true;
        var anyRunning = false;

        foreach (var child in _children)
        {
            var result = child.Tick(controller);

            if (result == BtStatus.Failure)
                return BtStatus.Failure;

            if (result == BtStatus.Running)
                anyRunning = true;

            if(result != BtStatus.Success)
                allSucceeded = false;
        }

        if (anyRunning) return BtStatus.Running;
        return allSucceeded ? BtStatus.Success : BtStatus.Running;
    }
}