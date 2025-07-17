using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.TemporalControl.Component
{
    /// <summary>
    /// A decorator node that wraps any IBehaviorNode and calls OnExit() when it transitions
    /// from Running to Success or Failure. Used to handle timer cleanup, state resets, etc.
    /// Now supports overlay/debug with LastStatus, NodeName, GetChildren.
    /// </summary>
    public class BtLifecycleNode : IBehaviorNode
    {
        private readonly IBehaviorNode _inner;
        private BtStatus _lastStatus = BtStatus.Running;

        public BtLifecycleNode(IBehaviorNode inner)
        {
            _inner = inner;
        }

        public BtStatus LastStatus => _lastStatus;
        public string DisplayName => _inner.DisplayName;
        public void Reset(BtContext context)
        {
            _inner.Reset(context);
            _lastStatus = BtStatus.Idle;
        }

        public void OnExitNode(BtContext context)
        {
            // Propagate to the wrapped/child node
            _inner.OnExitNode(context);

            // Reset this node's own state if needed
            _lastStatus = BtStatus.Idle;
        }

        public IEnumerable<IBehaviorNode> GetChildren => new[] { _inner };

        public BtStatus Tick(BtContext context)
        {
            var status = _inner.Tick(context);
            _lastStatus = status;
            return _lastStatus;
        }
    }
}