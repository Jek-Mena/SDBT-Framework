using UnityEngine;

public class TimerNode : TimedExecutionNode
{
    public TimerNode(TimedExecutionData data) : base(data) { }

    public override BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context).Timers().Check(out var error))
        {
            Debug.LogError(error);
            return BtStatus.Failure;
        }

        EnsureTimerStarted();

        var timerStatus = CheckTimerStatus();
        return timerStatus; // Success when timer completes, Running otherwise
    }
}