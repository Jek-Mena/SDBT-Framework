using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Composites.Selector
{
    public class BtSelectorNode : IBehaviorNode
    {
        private readonly List<IBehaviorNode> _children;
        private readonly IChildSelectorStrategy _selectorStrategy;
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        public string DisplayName { get; set; }
        public void Reset(BtContext context)
        {
            // Reset all children so their internal state is fresh
            foreach (var child in _children)
                child.Reset(context);

            // Reset own status or any internal state
            LastStatus = BtStatus.Idle;
            // If your selector strategy holds internal state, reset it here as well
            // (Not needed for stateless strategies, but add a Reset() call if they support it)
        }

        public void OnExitNode(BtContext context)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IBehaviorNode> GetChildren => _children; // Expose children for debug tools, visualization, etc.

        public BtSelectorNode(
            List<IBehaviorNode> children, 
            IChildSelectorStrategy selectorStrategy,
            string displayName = nameof(BtSelectorNode))
        {
            _children = children;
            _selectorStrategy = selectorStrategy;
        }

        public BtStatus Tick(BtContext context)
        {
            var index = _selectorStrategy.SelectChildIndex(_children, context);
            if (index < 0 || index >= _children.Count)
            {
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }
            
            LastStatus = _children[index].Tick(context);
            return LastStatus;
        }
    }
}