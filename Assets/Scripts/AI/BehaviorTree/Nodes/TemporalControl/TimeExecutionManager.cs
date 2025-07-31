using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using Systems.Abstractions;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    /// <summary>
    /// [2025-06-13] Refactored for TimerExecutionMono to TimerExecutionManager
    /// </summary>
    public class TimeExecutionManager : ITimedExecutionNode, ISystemCleanable
    {
        private class TimerData
        {
            private readonly float _endTime;
            public bool IsRunning => Time.time < _endTime;
            public bool IsComplete => Time.time >= _endTime;
            public TimerData(float duration) => _endTime = Time.time + duration;
        }

        private const string ScriptName = nameof(TimeExecutionManager);
        private readonly Dictionary<string, TimerData> _timers = new();

        public void StartTime(string key, float duration)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception($"[{ScriptName}] Tried to start timer with empty/null key.");
        
            if (_timers.ContainsKey(key))
                throw new Exception($"[{ScriptName}] Timer key '{key}' already exists. Overwriting existing timer.");
        
            _timers[key] = new TimerData(duration);
        }

        public void Interrupt(string key)
        {
            if (_timers.Remove(key))
            {
                return;
                Debug.Log($"[Timer] Interrupted '{key}'");
            }
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
        public void LateTick()
        {
            if (_timers.Count == 0) return;
            var expiredTimers = new List<string>();
            foreach (var pair in _timers)
            {
                if (pair.Value.IsComplete)
                    expiredTimers.Add(pair.Key);
            }

            foreach (var key in expiredTimers)
                _timers.Remove(key);
        }

        public void ReleaseSystem(BtContext context)
        {
            _timers.Clear();
            //Debug.Log($"[{ScriptName}] CleanupSystem called, all timers cleared.");
        }
    }
}