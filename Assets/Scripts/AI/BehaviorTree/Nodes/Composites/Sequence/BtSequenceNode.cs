using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Composites.Sequence
{
    public class BtSequenceNode : IBehaviorNode
    {
        private readonly List<IBehaviorNode> _children;
        private int _currentIndex;

        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        public string DisplayName => BtNodeDisplayName.Composite.Sequence;
        public IEnumerable<IBehaviorNode> GetChildren => _children;
        
        public BtSequenceNode(List<IBehaviorNode> children)
        {
            _children = children;
            _currentIndex = 0;
        }

        public void Initialize(BtContext context)
        {
            foreach (var child in _children)
                child.Initialize(context);
            _currentIndex = 0;
            LastStatus = BtStatus.Initialized;       
        }

        public void Reset(BtContext context)
        {
            foreach (var child in _children)
                child.Reset(context);
            _currentIndex = 0;
            LastStatus = BtStatus.Reset;
        }
    
        public void OnExitNode(BtContext context)
        {
            foreach (var child in _children)
                child.OnExitNode(context);
            _currentIndex = 0;
            LastStatus = BtStatus.Idle;
        }
    
        public BtStatus Tick(BtContext context)
        {
            if(!BtValidator.Require(context)
                   .Children(_children)
                   .Check(out var error)
              )
            {
                Debug.Log(error);
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }
        
            while (_currentIndex < _children.Count)
            {
                //Debug.Log($"[{ScriptName}] Ticking child {_currentIndex}");
                var status = _children[_currentIndex].Tick(context);

                if (status == BtStatus.Running)
                {
                    LastStatus = BtStatus.Running;
                    return LastStatus;
                }

                if (status == BtStatus.Failure)
                {
                    LastStatus = BtStatus.Failure;
                    _currentIndex = 0;
                    return LastStatus;
                }

                _currentIndex++;
            }

            _currentIndex = 0;
            LastStatus = BtStatus.Success;
            return LastStatus;
        }
    }
}