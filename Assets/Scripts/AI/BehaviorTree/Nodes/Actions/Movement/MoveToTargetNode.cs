using UnityEngine;
// MoveNode (Behavior Tree)
// • What it does: Says “Move to that position.”
// • What it should not do: Decide how often to update destination, or whether it's “close enough.”
public class MoveToTargetNode : IBehaviorNode
{
    private const string ScriptName = nameof(MoveToTargetNode);
    
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
            return BtStatus.Failure;
        }
        
        // Resolve data from blackboard profile dictionaries
        // For now the movementData and targetingData are setup to fail-fast so, no need to check for null values.
        var movementData = context.Blackboard.GetMovementProfile(_movementProfileKey);
        // if (movementData == null)
        // {
        //     Debug.LogError($"[{ScriptName}] No 'MovementProfileKey' {_movementProfileKey} found in blackboard.");
        //     return BtStatus.Failure;
        // }
        
        var targetingData = context.Blackboard.GetTargetingProfile(_targetProfileKey);
        // if (targetingData == null)
        // {
        //     Debug.LogError($"[{ScriptName}] No 'TargetProfileKey' {_targetProfileKey} found in blackboard.'");
        //     return BtStatus.Failure;
        // }
        
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
        
        context.Movement.ApplySettings(movementData);
        var canMove = context.Movement.TryMoveTo(target.position);

        return canMove
            ? context.Movement.IsAtDestination() ? BtStatus.Success : BtStatus.Running
            : BtStatus.Failure;
    }
}