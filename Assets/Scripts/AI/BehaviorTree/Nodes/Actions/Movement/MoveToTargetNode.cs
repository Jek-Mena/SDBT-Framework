using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public class MoveToTargetNode : IBehaviorNode
    {
        private const string ScriptName = nameof(MoveToTargetNode);

        private BtStatus _lastStatus = BtStatus.Idle;
        public BtStatus LastStatus => _lastStatus;
        public string DisplayName => BtNodeDisplayName.Movement.MoveToTarget;
        public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

        private readonly string _movementProfileKey;
        private readonly string _targetProfileKey;

        private MovementData _lastMovementData;
        private TargetingData _lastTargetingData;

        public MoveToTargetNode(string movementProfileKey, string targetProfileKey)
        {
            _movementProfileKey = movementProfileKey;
            _targetProfileKey = targetProfileKey;
        }

        public BtStatus Tick(BtContext context)
        {
            if (!BtValidator.Require(context)
                    .Targeting(_targetProfileKey)
                    .Movement()
                    .Check(out var error))
            {
                Debug.Log(error);
                _lastStatus = BtStatus.Failure;
                return _lastStatus;
            }

            // Resolve data from blackboard profile dictionaries
            var movementData = context.AgentProfiles.GetMovementProfile(_movementProfileKey);
            var targetingData = context.AgentProfiles.GetTargetingProfile(_targetProfileKey);

            var resolver = TargetResolverRegistry.ResolveOrClosest(targetingData.Style);
            if (resolver == null)
            {
                Debug.LogError($"[{ScriptName}] No resolver for style '{targetingData.Style}'");
                _lastStatus = BtStatus.Failure;
                return _lastStatus;
            }

            var target = resolver.ResolveTarget(context.Agent, targetingData);
            if (!target)
            {
                Debug.LogError($"[{ScriptName}] No target found using targetTag: {targetingData.TargetTag}'");
                _lastStatus = BtStatus.Failure;
                return _lastStatus;
            }
            
            var canMove = context.MovementOrchestrator.TryMoveTo(target.position, movementData);

            _lastStatus = canMove
                ? context.MovementOrchestrator.IsAtDestination() ? BtStatus.Success : BtStatus.Running
                : BtStatus.Failure;

            return _lastStatus;
        }
    }
}
