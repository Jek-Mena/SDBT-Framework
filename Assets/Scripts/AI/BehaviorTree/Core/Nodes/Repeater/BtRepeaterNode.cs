using UnityEngine;

public class BtRepeaterNode : IBehaviorNode
{
    private readonly IBehaviorNode _child;
    private readonly int _maxRepeats;
    private int _repeatCount;

    public BtRepeaterNode(IBehaviorNode child, int maxRepeats = -1)
    {
        _child = child;
        _maxRepeats = maxRepeats;
        _repeatCount = 0;
    }

    public BtStatus Tick(BtController controller)
    {
        var status = _child.Tick(controller);

        if (status is BtStatus.Success or BtStatus.Failure)
        {
            _repeatCount++;
            if (_maxRepeats > 0 && _repeatCount >= _maxRepeats)
                return BtStatus.Success;
        }

        return BtStatus.Running;
    }
}