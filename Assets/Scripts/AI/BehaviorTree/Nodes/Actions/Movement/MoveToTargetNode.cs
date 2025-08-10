using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
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
            context.Services.Movement.CancelMovement();
            LastStatus = BtStatus.Reset;
        }
        
        public void OnExitNode(BtContext context)
        {
            context.Services.Movement.CancelMovement();
            LastStatus = BtStatus.Exit;
        }

        public BtStatus Tick(BtContext context)
        {
            // Resolve data from blackboard profile dictionaries
            var movementData = context.AgentProfiles.GetMovementProfile(_movementProfileKey);
            var targetObj = context.Blackboard.Get<object>(BlackboardKeys.Target.CurrentTarget);
            Vector3? targetPos = null;

            switch (targetObj)
            {
                case Transform t when t:
                    targetPos = t.position;
                    break;
                case Vector3 v:
                    targetPos = v;
                    break;
            }

            if (targetPos == null)
            {
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            var canMove = context.Services.Movement.TryIssueMoveIntent(targetPos.Value, movementData, context.Blackboard.Data.BtSessionId);

            //Debug.Log($"[{ScriptName}]🏃‍♂️Can move: {canMove}" );

            if (canMove)
                if (context.Services.Movement.IsAtDestination())
                    LastStatus = BtStatus.Success;
                else
                    LastStatus = BtStatus.Running;
            else
                LastStatus = BtStatus.Failure;

            context.Services.Movement.Tick(context.DeltaTime);
            
            return LastStatus;
        }
    }
}
