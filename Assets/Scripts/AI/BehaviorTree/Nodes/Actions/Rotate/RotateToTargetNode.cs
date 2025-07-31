using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Systems.TargetingSystem;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class RotateToTargetNode : IBehaviorNode
    {
        private const string ScriptName = nameof(RotateToTargetNode);
        public string DisplayName => BtNodeDisplayName.Rotation.RotateToTarget;
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;

        public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

        private readonly string _rotationProfileKey;
        private readonly string _targetProfileKey;
    
        public RotateToTargetNode(string rotationProfileKey, string targetProfileKey)
        {
            _rotationProfileKey = rotationProfileKey;
            _targetProfileKey = targetProfileKey;
        }
    
        public void Initialize(BtContext context)
        {
            LastStatus = BtStatus.Initialized;      
        }
        
        public void Reset(BtContext context)
        {
            context.Blackboard.RotationIntentRouter.CancelRotation();
            LastStatus = BtStatus.Exit;       
        }
    
        public void OnExitNode(BtContext context)
        {
            context.Blackboard.RotationIntentRouter.CancelRotation();
            LastStatus = BtStatus.Exit;
        }

        public BtStatus Tick(BtContext context)
        {
            if (!BtValidator.Require(context)
                    .Targeting(_targetProfileKey)
                    .Rotation()
                    .Check(out var error)
               )
            {
                Debug.Log(error);
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            // Resolve data from blackboard profile dictionaries
            var rotationData = context.AgentProfiles.GetRotationProfile(_rotationProfileKey);
            var targetObj = context.Blackboard.Get<object>(BlackboardKeys.Target.CurrentTarget);
            Transform target = null;

            if (targetObj is Transform t && t)
                target = t;
            else if (targetObj is GameObject go && go.transform)
                target = go.transform;
            // (Optionally) if targetObj is Vector3, create a dummy GameObject or handle accordingly

            if (!target)
            {
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            var canRotate = context.Blackboard.RotationIntentRouter.TryIssueRotateIntent(target, rotationData, context.Blackboard.BtSessionId);

            //Debug.Log($"[{ScriptName}]🔁Can rotate: {canRotate}" );
            
            if(canRotate)
                if (context.Blackboard.RotationIntentRouter.IsFacingTarget())
                    LastStatus = rotationData.SuccessToRunning ? BtStatus.Running : BtStatus.Success;
                else 
                    LastStatus = BtStatus.Running;
            else
                LastStatus = BtStatus.Failure;
            
            // if (!canRotate)
            //     LastStatus = BtStatus.Failure;
            // else if (context.Blackboard.RotationIntentRouter.IsFacingTarget())
            //     LastStatus = BtStatus.Success;
            // else
            //     LastStatus = BtStatus.Running;
        
            // Debug.Log($"[RotateToTargetNode]💯🚀🎯Rotating to: {target} | " +
            //           $"DomainBlocked: {context.Blackboard.StatusEffectManager.IsBlocked(DomainKeys.Rotation)} | " +
            //           $"CurrentSettings: {context.Blackboard.RotationIntentRouter.GetCurrentSettings()}");
            //
            context.Blackboard.RotationIntentRouter.Tick(context.DeltaTime);
            
            return LastStatus;
        }
    }
}
