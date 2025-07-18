using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    /// <summary>
    /// A decorator node that wraps any IBehaviorNode and calls OnExit() when it transitions
    /// from Running to Success or Failure. Used to handle timer cleanup, state resets, etc.
    /// Now supports overlay/debug with LastStatus, NodeName, GetChildren.
    /// </summary>
    public class BtLifecycleNode : IBehaviorNode
    {
        private readonly IBehaviorNode _inner;
        public string DisplayName => _inner.DisplayName;
        public BtStatus LastStatus { get; private set; } = BtStatus.Running;

        public BtLifecycleNode(IBehaviorNode inner)
        {
            _inner = inner;
        }

        public void Reset(BtContext context)
        {
            _inner.Reset(context);
            LastStatus = BtStatus.Reset;
        }

        public void OnExitNode(BtContext context)
        {
            _inner.OnExitNode(context);
            LastStatus = BtStatus.Exit;
        }

        public IEnumerable<IBehaviorNode> GetChildren => new[] { _inner };

        public void Initialize(BtContext context)
        {
            _inner.Initialize(context);
            LastStatus = BtStatus.Initialized;
        }

        public BtStatus Tick(BtContext context)
        {
            var status = _inner.Tick(context);
            LastStatus = status;
            return LastStatus;
        }
    }
}