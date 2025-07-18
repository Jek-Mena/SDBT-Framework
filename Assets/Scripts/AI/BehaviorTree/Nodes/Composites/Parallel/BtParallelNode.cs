using System;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Composites.Parallel
{
    /// <summary>
    /// Parallel node that ticks all children every frame.
    /// Returns:
    /// - Failure: if ANY child returns Failure
    /// - Running: if ANY child is Running (and none failed)
    /// - Success: if ALL children return Success
    /// 
    /// - Implementation supports all standard exit conditions.
    /// </summary>
    public class BtParallelNode : IBehaviorNode
    {
        private const string ScriptName = nameof(BtParallelNode);
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        public string DisplayName => BtNodeDisplayName.Composite.Parallel;

        public IEnumerable<IBehaviorNode> GetChildren => _children;

        private readonly List<IBehaviorNode> _children;
        private readonly ParallelExitCondition _exitCondition;

        public BtParallelNode(List<IBehaviorNode> children, ParallelExitCondition exitCondition)
        {
            _children = children;
            _exitCondition = exitCondition;
        }

        public void Initialize(BtContext context)
        {
            foreach (var child in _children)
                child.Initialize(context);
            LastStatus = BtStatus.Initialized;
        }   

        public BtStatus Tick(BtContext context)
        {
            if (_children == null || _children.Count == 0)
            {
                //Debug.LogError($"[{ScriptName}] No children found.");
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            if (!BtValidator.Require(context)
                    .Children(_children)
                    .Check(out var error)
               )
            {
                Debug.LogError(error);
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            var anyRunning = false;
            var anySuccess = false;
            var anyFailure = false;
            var allSuccess = true;
            var allFailure = true;

            for (var i = 0; i < _children.Count; i++)
            {
                //Debug.Log($"[{ScriptName}] Ticking child {i}");
                var status = _children[i].Tick(context);

                switch (status)
                {
                    case BtStatus.Running:
                        anyRunning = true;
                        allSuccess = false;
                        allFailure = false;
                        break;
                    case BtStatus.Success:
                        anySuccess = true;
                        allFailure = false;
                        break;
                    case BtStatus.Failure:
                        anyFailure = true;
                        allSuccess = false;
                        break;
                }
            }

            switch (_exitCondition)
            {
                case ParallelExitCondition.FirstSuccess:
                    LastStatus = anySuccess ? BtStatus.Success
                        : anyRunning ? BtStatus.Running
                        : BtStatus.Failure;
                    break;

                case ParallelExitCondition.FirstFailure:
                    LastStatus = anyFailure ? BtStatus.Failure
                        : anyRunning ? BtStatus.Running
                        : BtStatus.Success;
                    break;

                case ParallelExitCondition.AllSuccess:
                    LastStatus = allSuccess ? BtStatus.Success
                        : anyRunning ? BtStatus.Running
                        : BtStatus.Failure;
                    break;

                case ParallelExitCondition.AllFailure:
                    LastStatus = allFailure ? BtStatus.Failure
                        : anyRunning ? BtStatus.Running
                        : BtStatus.Success;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_exitCondition),
                        $"[{ScriptName}] Unknown or unsupported exit condition.");
            }

            return LastStatus;
        }
        
        public void Reset(BtContext context)
        {
            foreach (var child in _children)
                child.Reset(context);
            LastStatus = BtStatus.Reset;
            // If you have any additional local state, reset here.
        }

        public void OnExitNode(BtContext context)
        {
            foreach (var child in _children)
                child.OnExitNode(context);
            LastStatus = BtStatus.Exit;
        }
    }
}
