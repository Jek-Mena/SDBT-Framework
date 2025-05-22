using UnityEngine;

public class TimeoutDecoratorNode : TimedExecutionNode
{
    private readonly IBehaviorNode _child;
    
    public TimeoutDecoratorNode(IBehaviorNode child, TimedExecutionData data) : base(data)
    {
        _child = child;
    }

    public override BtStatus Tick(BtContext context)
    {
        EnsureTimerStarted();
        
        if (Timer == null)
        {
            Debug.LogError("[TimeoutNode] Timer system is null.");
            return BtStatus.Failure;
        }

        var timerStatus = CheckTimerStatus();

        // If no child, behave purely as a timer node.
        if (_child == null)
            return timerStatus;

        var childStatus = _child.Tick(context);

        return childStatus == BtStatus.Success || timerStatus == BtStatus.Success
            ? BtStatus.Success
            : BtStatus.Running;
    }
}