using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerExecutionMono : MonoBehaviour, ITimedExecutionNode
{
    private class TimerData
    {
        public float EndTime;
        public bool IsRunning => Time.time < EndTime;
        public bool IsComplete => Time.time >= EndTime;

        public TimerData(float duration) => EndTime = Time.time + duration;
    }

    private readonly Dictionary<string, TimerData> _timers = new();

    public void StartTime(string key, float duration)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new Exception("[TimerExecutionMono] Tried to start timer with empty/null key.");
        
        if (_timers.ContainsKey(key))
            throw new Exception($"[TimerExecutionMono] Timer key '{key}' already exists. Overwriting existing timer.");
        
        _timers[key] = new TimerData(duration);
    }

    public void Interrupt(string key)
    {
        if (_timers.Remove(key))
            Debug.Log($"[Timer] Interrupted '{key}'");
    }

    public bool IsRunning(string key)
    {
        if (!_timers.TryGetValue(key, out var timer)) 
            return false;
        return timer.IsRunning;
    }

    public bool IsComplete(string key)
    {
        if (!_timers.TryGetValue(key, out var timer)) 
            return false;
        return timer.IsComplete;
    }

    //Clean up completed timers (helpful for long sessions)
    private void LateUpdate()
    {
        var expiredTimers = new List<string>();
        foreach (var pair in _timers)
        {
            if (pair.Value.IsComplete)
                expiredTimers.Add(pair.Key);
        }

        foreach (var key in expiredTimers)
            _timers.Remove(key);
    }
}

/// <summary>
/// Marker for behavior nodes that require lifecycle hooks (e.g. OnExit).
/// Currently supports OnExit(), but may expand to full lifecycle methods in the future:
/// - OnEnter()
/// - Reset()
/// - Cleanup()
/// 
/// Consider renaming this to ILifecycleBehavior or refactoring to modular lifecycle interface:
/// public interface ILifecycleBehavior
/// {
///     void OnEnter();
///     void OnExit();
///     void Reset();
/// }
/// </summary>
public interface IExitableBehavior
{
    void OnExit();
}