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
        public IEnumerable<IBehaviorNode> GetChildren => _children;
        
        public BtSelectorNode(
            List<IBehaviorNode> children, 
            IChildSelectorStrategy selectorStrategy,
            string displayName = nameof(BtSelectorNode))
        {
            _children = children ?? throw new System.ArgumentNullException(nameof(children));
            _selectorStrategy = selectorStrategy ?? throw new System.ArgumentNullException(nameof(selectorStrategy));
            DisplayName = displayName;
        }

        public void Initialize(BtContext context)
        {
            foreach (var child in _children)
                child.Initialize(context);
            LastStatus = BtStatus.Initialized;
        }
        
        public void Reset(BtContext context)
        {
            foreach (var child in _children)
                child.Reset(context);
            LastStatus = BtStatus.Reset;
        }

        public void OnExitNode(BtContext context)
        {
            foreach (var child in _children)
                child.OnExitNode(context);
            LastStatus = BtStatus.Exit;
        }
        
        public BtStatus Tick(BtContext context)
        {
            if (_children == null || _children.Count == 0)
            {
                Debug.LogWarning($"[{DisplayName}] No children found.");
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }
            
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