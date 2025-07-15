using UnityEngine;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;

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
    
    public void Reset(BtContext context)
    {
        context.Blackboard.RotationIntentRouter.CancelRotation();
        LastStatus = BtStatus.Idle;       
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

        Debug.Log($"[{ScriptName}]🔁Loaded rotationProfile: {_rotationProfileKey}, data: {JsonUtility.ToJson(rotationData)}");
        Debug.Log($"[{ScriptName}]🎯Loaded targetingProfile: {_targetProfileKey}, data: {JsonUtility.ToJson(targetingData)}");
        
        var resolver = TargetResolverRegistry.ResolveOrClosest(targetingData.Style);
        if (resolver == null)
        {
            Debug.LogError($"[{ScriptName}] No resolver for style '{targetingData.Style}'");
            LastStatus = BtStatus.Failure;
            return LastStatus;
        }

        var target = resolver.ResolveTarget(context.Agent, targetingData);
        if (!target)
        {
            Debug.LogError($"[{ScriptName}] No target found using targetTag: {targetingData.TargetTag}'");
            LastStatus = BtStatus.Failure;
            return LastStatus;
        }
        
        var canRotate = context.Blackboard.RotationIntentRouter.TryIssueRotateIntent(target.position, rotationData, context.Blackboard.BtSessionId);
        Debug.Log($"[{ScriptName}]🔁Can rotate: {canRotate}" );

        LastStatus = canRotate
            ? context.Blackboard.RotationIntentRouter.IsFacingTarget(target.position) ? BtStatus.Success : BtStatus.Running
            : BtStatus.Failure;
        
        return LastStatus;
    }
}
