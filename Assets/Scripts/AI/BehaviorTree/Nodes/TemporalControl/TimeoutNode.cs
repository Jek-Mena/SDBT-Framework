using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    public class TimeoutNode : IBehaviorNode
    {
        private readonly IBehaviorNode _child;
        private readonly TimedExecutionComponent _timed;
        public BtStatus LastStatus { get; private set; }
        public string DisplayName { get; private set; }
        public IEnumerable<IBehaviorNode> GetChildren => _child != null ? new[] { _child } : System.Linq.Enumerable.Empty<IBehaviorNode>();
        
        public TimeoutNode(IBehaviorNode child, TimedExecutionData data)
        {
            _child = child;
            _timed = new TimedExecutionComponent(data);
        }
        
        public void Initialize(BtContext context)
        {
            _timed.Initialize(context);
            _child?.Initialize(context);
            LastStatus = BtStatus.Idle;
        }
        
        public BtStatus Tick(BtContext context)
        {
            _timed.StartTimerIfNeeded();
            var timerStatus = _timed.GetTimerStatus();
            if (_child == null)
            {
                LastStatus = timerStatus;
                return timerStatus;
            }

            if (timerStatus == BtStatus.Running)
            {
                var childStatus = _child.Tick(context);
                LastStatus = childStatus == BtStatus.Success ? BtStatus.Success : BtStatus.Running;
                return LastStatus;
            }

            LastStatus = timerStatus;
            return timerStatus;
        }

        public void Reset(BtContext context)
        {
            _timed.InterruptTimer();
            _child?.Reset(context);
            LastStatus = BtStatus.Idle;
        }

        public void OnExitNode(BtContext context)
        {
            Reset(context); // If you need special logic, add it here instead
        }
    }
}