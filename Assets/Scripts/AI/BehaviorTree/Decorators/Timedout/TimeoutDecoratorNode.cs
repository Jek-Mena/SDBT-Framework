using UnityEngine;

public class TimeoutDecoratorNode : IBehaviorNode
{
    private readonly IBehaviorNode _child;
    private readonly float _duration;
    private readonly ITimedExecutionNode _timer;
    private readonly string _key;

    public TimeoutDecoratorNode(IBehaviorNode child, float duration, ITimedExecutionNode timer, string key)
    {
        _child = child;
        _duration = duration;
        _timer = timer;
        _key = key;
    }

    public BtStatus Tick(BtController controller)
    {
        if (_timer == null)
        {
            Debug.LogError("[TimeoutNode] Timer system is null — did you forget to add TimeoutNodePlugin?");
            return BtStatus.Failure;
        }

        Debug.Log($"[TimeoutNode] Duration set to {_duration}");

        // Start Timer once
        if (!_timer.IsRunning(_key) && !_timer.IsComplete(_key))
            _timer.StartTime(_key, _duration);

        // 2) Let your child (e.g. MoveTo) run every frame for up to _duration seconds
        var childStatus = _child.Tick(controller);
        // 3) If either the move completes OR the timer expires, we’re done
        if (childStatus == BtStatus.Success || _timer.IsComplete(_key))
            return BtStatus.Success;

        return BtStatus.Running;
    }
}