using System.Collections.Generic;
using UnityEngine;

public class TimerExecutionMono : MonoBehaviour, ITimedExecutionNode
{
    private class TimerData
    {
        public float EndTime;
        public bool IsRunning => Time.time < EndTime;
        public bool IsComplete => Time.time >= EndTime;

        public TimerData(float duration)
        {
            EndTime = Time.time + duration;
        }
    }

    private readonly Dictionary<string, TimerData> _timers = new();

    public void StartTime(string key, float duration)
    {
        _timers[key] = new TimerData(duration);
    }

    public void Interrupt(string key)
    {
        _timers.Remove(key);
    }

    public bool IsRunning(string key)
    {
        return _timers.TryGetValue(key, out var timer) && !timer.IsComplete;
    }

    public bool IsComplete(string key)
    {
        return _timers.TryGetValue(key, out var timer) && timer.IsComplete;
    }
}

