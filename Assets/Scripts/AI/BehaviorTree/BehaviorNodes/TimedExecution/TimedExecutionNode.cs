using UnityEngine;

public abstract class TimedExecutionNode : IBehaviorNode
{
    public BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;
        var timer = blackboard.TimedExecutionLogic;
        var data = blackboard.CurrentNodeData as TimedExecutionData;

        if (timer == null || data == null)
            return BtStatus.Failure;

        string key = string.IsNullOrEmpty(data.key)
            ? $"TimerNode:{controller.gameObject.GetInstanceID()}"
            : data.key;

        if (data.interruptible && blackboard.IsStunned)
        {
            timer.Interrupt(key);
            return data.failOnInterrupt ? BtStatus.Failure : BtStatus.Success;
        }

        if (timer.IsRunning(key))
            return timer.IsComplete(key) ? BtStatus.Success : BtStatus.Running;

        timer.StartTime(key, data.duration);
        return BtStatus.Running;
    }
}