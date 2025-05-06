using UnityEngine;

public abstract class TimedExecutionNode : IBehaviorNode
{
    public virtual BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;
        var timer = blackboard.TimedExecutionLogic;
        var data = blackboard.TimerData;

        if (timer == null || data == null)
        {
            Debug.LogError($"[PauseNode] Missing timer or timer data on '{controller.gameObject.name}'");
            return BtStatus.Failure;
        }

        var key = string.IsNullOrEmpty(data.key)
            ? $"TimerNode:{controller.gameObject.GetInstanceID()}"
            : data.key;

        if (data.interruptible && blackboard.IsStunned)
        {
            timer.Interrupt(key);
            return data.failOnInterrupt ? BtStatus.Failure : BtStatus.Success;
        }

        if (!timer.IsRunning(key) && !timer.IsComplete(key))
            timer.StartTime(key, data.duration);

        return timer.IsComplete(key) ? BtStatus.Success : BtStatus.Running;
    }
}