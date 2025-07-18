using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.TemporalControl.Base
{
    /// <summary>
    /// Represents an abstract behavior tree node that executes for a specified duration of time.
    /// This class is designed to manage timed execution logic, allowing derived nodes to perform
    /// actions within a defined time frame.
    /// It integrates with the behavior tree system and utilizes timed execution data to control its behavior.
    /// </summary>
    [System.Obsolete]
    public abstract class TimedExecutionNode : IBehaviorNode
    {
        protected readonly TimedExecutionData TimeData;
        protected readonly string Label;
        protected ITimedExecutionNode Timer;
        protected bool TimerStarted;

        // Overlay/debug support:
        protected BtStatus _lastStatus = BtStatus.Idle;
        public virtual BtStatus LastStatus => _lastStatus;
        public virtual string DisplayName => GetType().Name;
        
        public virtual IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

        protected TimedExecutionNode(TimedExecutionData timeData)
        {
            TimeData = timeData;
            Label = timeData.Label;
        }

        public virtual void Initialize(BtContext context)
        {
            Timer = context.Blackboard.TimeExecutionManager;
            Debug.Log($"[TimedExecutionNode] Initializing {Label} Duration: {TimeData.Duration}");
        }

        public abstract BtStatus Tick(BtContext context);

        public virtual void Reset(BtContext context)
        {
            _lastStatus = BtStatus.Idle;
            TimerStarted = false;
            // If you have any timers or custom state, reset/cancel them here
            // Optionally: Timer?.Interrupt(Label);
        }
        
        public virtual void OnExitNode(BtContext context)
        {
            Timer?.Interrupt(Label);
            TimerStarted = false;
            _lastStatus = BtStatus.Idle;
        }
        
        protected BtStatus CheckTimerStatus()
        {
            return Timer.IsComplete(Label) ? BtStatus.Success : BtStatus.Running;
        }
        
        protected void EnsureTimerStarted()
        {
            if (TimerStarted) return;

            Timer.StartTime(Label, TimeData.Duration);
            TimerStarted = true;
            Debug.Log($"[TimedExecutionNode] Timer started for {Label} with duration {TimeData.Duration}");
        }
    
    }
}