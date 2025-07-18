using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    public class TimerNode : IBehaviorNode
    {
        private readonly TimedExecutionComponent _timed;
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();
        public string DisplayName => string.IsNullOrEmpty(_timed.Data.Label) ? $"{BtNodeDisplayName.TimedExecution.Timer}" : $"{BtNodeDisplayName.TimedExecution.Timer} ({_timed.Data.Label})";
        public TimerNode(TimedExecutionData data)
        {
            _timed = new TimedExecutionComponent(data);
        }

        public void Initialize(BtContext context)
        {
            _timed.Initialize(context);
            LastStatus = BtStatus.Initialized;
        }
        
        public BtStatus Tick(BtContext context)
        {
            _timed.StartTimerIfNeeded();
            LastStatus = _timed.GetTimerStatus();
            return _timed.GetTimerStatus();
        }

        public void Reset(BtContext context)
        {
            _timed.InterruptTimer();
            LastStatus = BtStatus.Reset;
        }

        public void OnExitNode(BtContext context)
        {
            _timed.InterruptTimer();
            LastStatus = BtStatus.Exit;
        }
    }
}