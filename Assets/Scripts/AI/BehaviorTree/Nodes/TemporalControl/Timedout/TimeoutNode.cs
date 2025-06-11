using UnityEngine;

public class TimeoutNode : TimedExecutionNode
{
    private readonly IBehaviorNode _child;
    
    public TimeoutNode(IBehaviorNode child, TimedExecutionData data) : base(data)
    {
        _child = child;
    }

    public override BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .Timers()
                .Check(out var error)
           )
        {
            Debug.Log(error);
            return BtStatus.Failure;           
        }
        
        EnsureTimerStarted();

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