using UnityEngine;

public class RotateToTargetNode : IBehaviorNode
{
    private const string ScriptName = nameof(RotateToTargetNode);
    
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
            return BtStatus.Failure;
        }
        
        // Resolve data from blackboard profile dictionaries
        var rotationData = context.Blackboard.GetRotationProfile(_rotationProfileKey);
        var targetingData = context.Blackboard.GetTargetingProfile(_targetProfileKey);

        var resolver = TargetResolverRegistry.ResolveOrClosest(targetingData.Style);
        if (resolver == null)
        {
            Debug.LogError($"[{ScriptName}] No resolver for style '{targetingData.Style}'");
            return BtStatus.Failure;
        }
        
        var target = resolver.ResolveTarget(context.Agent, targetingData);
        if (!target)
        {
            Debug.LogError($"[{ScriptName}] No target found using targetTag: {targetingData.TargetTag}'");
            return BtStatus.Failure;
        }
        
        context.Rotation.ApplySettings(rotationData);
        var canRotate = context.Rotation.TryRotateTo(target.position);

        return canRotate
            ? context.Rotation.IsFacingTarget(target.position) ? BtStatus.Success : BtStatus.Running
            : BtStatus.Failure;
    }
}