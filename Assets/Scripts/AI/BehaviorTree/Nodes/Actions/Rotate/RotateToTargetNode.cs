using UnityEngine;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;

public class RotateToTargetNode : IBehaviorNode
{
    private const string ScriptName = nameof(RotateToTargetNode);

    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Rotation.RotateToTarget;
    public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

    private readonly string _rotationProfileKey;
    private readonly string _targetProfileKey;

    public RotateToTargetNode(string rotationProfileKey, string targetProfileKey)
    {
        _rotationProfileKey = rotationProfileKey;
        _targetProfileKey = targetProfileKey;
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
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        // Resolve data from blackboard profile dictionaries
        var rotationData = context.AgentProfiles.GetRotationProfile(_rotationProfileKey);
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

        context.Rotation.ApplySettings(rotationData);
        var canRotate = context.Rotation.TryRotateTo(target.position);

        if (canRotate)
        {
            _lastStatus = context.Rotation.IsFacingTarget(target.position) ? BtStatus.Success : BtStatus.Running;
        }
        else
        {
            _lastStatus = BtStatus.Failure;
        }
        return _lastStatus;
    }
}
