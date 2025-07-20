using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
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
            var targetingData = context.AgentProfiles.GetTargetingProfile(_targetProfileKey);

            //Debug.Log($"[{ScriptName}]🔁Loaded rotationProfile: {_rotationProfileKey}, data: {JsonUtility.ToJson(rotationData)}");
            //Debug.Log($"[{ScriptName}]🎯Loaded targetingProfile: {_targetProfileKey}, data: {JsonUtility.ToJson(targetingData)}");
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
                Debug.LogError($"[{ScriptName}] No target found using {resolver}: {targetingData.Style}'");
                LastStatus = BtStatus.Failure;
                return LastStatus;
            }

            //var targetObj = context.Blackboard.Get<object>(_targetProfileKey);
            // Debug.Log($"[RotateToTargetNode] Retrieved target for rotation: Type={targetObj?.GetType().Name}, Value={targetObj}");
            // if (targetObj is Transform tf) Debug.Log($"[RotateToTargetNode] Target Transform Position: {tf.position}");
            // if (targetObj is Vector3 v3) Debug.Log($"[RotateToTargetNode] Target Vector3: {v3}");
            // if (targetObj is GameObject go)
            // {
            //     Debug.Log($"[RotateToTargetNode] Target GameObject: {go.name} Position: {go.transform.position}");
            //     Debug.Log($"[{ScriptName}] Issuing rotation intent to: {go.name} at {go.transform.position}");
            // }

            var canRotate = context.Blackboard.RotationIntentRouter.TryIssueRotateIntent(target, rotationData, context.Blackboard.BtSessionId);
            //Debug.Log($"[{ScriptName}]🔁Can rotate: {canRotate}" );
        
            if (!canRotate)
                LastStatus = BtStatus.Failure;
            else if (context.Blackboard.RotationIntentRouter.IsFacingTarget(target))
                LastStatus = BtStatus.Success;
            else
                LastStatus = BtStatus.Running;
        
            // Debug.Log($"[RotateToTargetNode]💯🚀🎯Rotating to: {target} | " +
            //           $"DomainBlocked: {context.Blackboard.StatusEffectManager.IsBlocked(DomainKeys.Rotation)} | " +
            //           $"CurrentSettings: {context.Blackboard.RotationIntentRouter.GetCurrentSettings()}");
            //
            return LastStatus;
        }
    }
}
