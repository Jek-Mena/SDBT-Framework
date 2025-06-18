using System;
using UnityEngine;

public class TimeoutNode : TimedExecutionNode
{
    private readonly IBehaviorNode _child;
    private readonly string[] _domains;
    
    public TimeoutNode(IBehaviorNode child, TimedExecutionData data, string[] domains = null) 
        : base(data)
    {
        _child = child;
        _domains = domains ?? Array.Empty<string>();
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