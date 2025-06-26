using UnityEngine;

public class TimerNode : TimedExecutionNode
{
    public TimerNode(TimedExecutionData data) : base(data) { }

    public override BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context).Timers().Check(out var error))
        {
            Debug.LogError(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        EnsureTimerStarted();

        var timerStatus = CheckTimerStatus();
        _lastStatus = timerStatus;
        return _lastStatus; // Success when timer completes, Running otherwise
    }
}