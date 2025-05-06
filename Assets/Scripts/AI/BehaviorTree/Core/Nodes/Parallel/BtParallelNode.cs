// This is Success-on-ALL. Write Success-on-ANY later on.

using System.Collections.Generic;

public class BtParallelNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public BtParallelNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtController controller)
    {
        var allSucceeded = true;
        var anyRunning = false;

        foreach (var child in _children)
        {
            var status = child.Tick(controller);

            if (status == BtStatus.Failure)
                return BtStatus.Failure;

            if (status == BtStatus.Running)
                anyRunning = true;

            if(status != BtStatus.Success)
                allSucceeded = false;
        }

        if (anyRunning) return BtStatus.Running;
        return allSucceeded ? BtStatus.Success : BtStatus.Running;
    }
}