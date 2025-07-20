using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using AI.BehaviorTree.Runtime.Context;
using Systems.TargetingSystem;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public class MoveToTargetNode : IBehaviorNode
    {
        private const string ScriptName = nameof(MoveToTargetNode);
        public string DisplayName => BtNodeDisplayName.Movement.MoveToTarget;
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;
        
        public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

        private readonly string _movementProfileKey;
        private readonly string _targetProfileKey;
        
        public MoveToTargetNode(string movementProfileKey, string targetProfileKey)
        {
            _movementProfileKey = movementProfileKey;
            _targetProfileKey = targetProfileKey;
        }

        public void Initialize(BtContext context)
        {
            LastStatus = BtStatus.Initialized;       
        }
        
        public void Reset(BtContext context)
        {
            // Only called when node is interrupted or parent resets (e.g. after pause)
            context.Blackboard.MovementIntentRouter.CancelMovement();
            LastStatus = BtStatus.Reset;
        }
        
        public void OnExitNode(BtContext context)
        {
            context.Blackboard.MovementIntentRouter.CancelMovement();
            LastStatus = BtStatus.Exit;
        }

        public BtStatus Tick(BtContext context)
        {
            if (!BtValidator.Require(context)
                    .Targeting(_targetProfileKey)
                    .MovementOrchestrator()
                    .Check(out var error))
            {
                Debug.Log(error);
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }
            
            // Resolve data from blackboard profile dictionaries
            var movementData = context.AgentProfiles.GetMovementProfile(_movementProfileKey);
            var targetingData = context.AgentProfiles.GetTargetingProfile(_targetProfileKey);

            var resolver = TargetResolverRegistry.ResolveOrClosest(targetingData.Style);
            if (resolver == null)
            {
                Debug.LogError($"[{ScriptName}] No resolver for style '{targetingData.Style}'");
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            var target = resolver.ResolveTarget(context.Agent, targetingData, context);
            if (!target)
            {
                Debug.LogError($"[{ScriptName}] No target found using targetTag: {targetingData.TargetTag}'");
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }
            
            var canMove = context.Blackboard.MovementIntentRouter.TryIssueMoveIntent(target.position, movementData, context.Blackboard.BtSessionId);
            //Debug.Log($"[{ScriptName}]🏃‍♂️Can move: {canMove}" );
            
            LastStatus = canMove
                ? context.Blackboard.MovementIntentRouter.IsAtDestination() ? BtStatus.Success : BtStatus.Running
                : BtStatus.Failure;

            return LastStatus;
        }
    }
}
