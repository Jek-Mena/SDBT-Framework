using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Decorators.Repeater
{
    public class BtRepeaterNode : IBehaviorNode
    {
        private readonly IBehaviorNode _child;
        private readonly int _maxRepeats;
        private int _repeatCount;
        
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        public string DisplayName => BtNodeDisplayName.Decorators.Repeater;
        public IEnumerable<IBehaviorNode> GetChildren => new[] { _child };

        public BtRepeaterNode(IBehaviorNode child, int maxRepeats = -1)
        {
            _child = child;
            _maxRepeats = maxRepeats;
            _repeatCount = 0;
        }
        
        public void Initialize(BtContext context)
        {
            _child.Initialize(context);
            _repeatCount = 0;
            LastStatus = BtStatus.Initialized;
        }
    
        public void Reset(BtContext context)
        {
            _child.Reset(context);
            _repeatCount = 0;
            LastStatus = BtStatus.Reset;
        }

        public void OnExitNode(BtContext context)
        {
            _child.OnExitNode(context);
            _repeatCount = 0;
            LastStatus = BtStatus.Exit;
        }

        public BtStatus Tick(BtContext context)
        {
            if (!BtValidator.Require(context)
                    .RequireChild(_child)
                    .Check(out var error))
            {
                Debug.LogError(error);
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            // Infinite repeat if _maxRepeats < 0 (default)
            while (_maxRepeats < 0 || _repeatCount < _maxRepeats)
            {
                var status = _child.Tick(context);

                if (status == BtStatus.Running)
                {
                    LastStatus = BtStatus.Running;
                    return LastStatus;
                }

                // Only increment if child actually finished (success or failure)
                _repeatCount++;

                // If we've hit the repeat limit, finish
                if (_maxRepeats > 0 && _repeatCount >= _maxRepeats)
                {
                    LastStatus = BtStatus.Success;
                    return LastStatus;
                }

                // Reset child for next repeat
                _child.Reset(context);
            }

            LastStatus = BtStatus.Running;
            return LastStatus;
        }
    }
}