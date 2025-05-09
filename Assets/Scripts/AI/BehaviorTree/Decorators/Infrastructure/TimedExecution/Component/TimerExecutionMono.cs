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
            Debug.Log($"Creating TimerData with a of {duration}");
            EndTime = Time.time + duration;
        }
    }

    private readonly Dictionary<string, TimerData> _timers = new();

    public void StartTime(string key, float duration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning("[TimerExecutionMono] Tried to start timer with empty/null key.");
            return;
        }

        if (_timers.ContainsKey(key))
        {
            Debug.LogWarning($"[TimerExecutionMono] Timer key '{key}' already exists. Overwriting existing timer.");
        }

        _timers[key] = new TimerData(duration);
        Debug.Log($"[Timer] Started '{key}' for {duration:F2}s (ends at {Time.time + duration:F2})");
    }

    public void Interrupt(string key)
    {
        if (_timers.Remove(key))
            Debug.Log($"[Timer] Interrupted '{key}'");
    }

    public bool IsRunning(string key)
    {
        if (!_timers.TryGetValue(key, out var timer)) return false;
        return timer.IsRunning;
    }

    public bool IsComplete(string key)
    {
        if (!_timers.TryGetValue(key, out var timer)) return false;
        return timer.IsComplete;
    }

    //Clean up completed timers (helpful for long sessions)
    private void LateUpdate()
    {
        var keysToRemove = new List<string>();
        foreach (var pair in _timers)
        {
            if (pair.Value.IsComplete)
                keysToRemove.Add(pair.Key);
        }

        foreach (var key in keysToRemove)
        {
            _timers.Remove(key);
            Debug.Log($"[Timer] Auto-cleared completed timer '{key}'");
        }
    }
}

