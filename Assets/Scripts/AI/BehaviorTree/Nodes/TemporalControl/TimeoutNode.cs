using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutNode : TimedExecutionNode
{
    private readonly IBehaviorNode _child;
    private readonly string[] _domains;
    public override string DisplayName => string.IsNullOrEmpty(Label) ? $"{BtNodeDisplayName.TemporalCondition.Timeout}" : $"{BtNodeDisplayName.TemporalCondition.Timeout} ({Label})";

    public TimeoutNode(IBehaviorNode child, TimedExecutionData data, string[] domains = null)
        : base(data)
    {
        _child = child;
        _domains = domains ?? Array.Empty<string>();
    }

    public override IEnumerable<IBehaviorNode> GetChildren => _child != null ? new[] { _child } : Array.Empty<IBehaviorNode>();

    public override BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .Timers()
                .Check(out var error)
           )
        {
            Debug.Log(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;           
        }

        EnsureTimerStarted();

        var timerStatus = CheckTimerStatus();

        // If no child, behave purely as a timer node.
        if (_child == null)
        {
            _lastStatus = timerStatus;
            return _lastStatus;
        }

        var childStatus = _child.Tick(context);

        _lastStatus = childStatus == BtStatus.Success || timerStatus == BtStatus.Success
            ? BtStatus.Success
            : BtStatus.Running;
        return _lastStatus;
    }
}