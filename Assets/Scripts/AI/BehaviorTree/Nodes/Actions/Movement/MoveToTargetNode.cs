using UnityEngine;
// MoveNode (Behavior Tree)
// • What it does: Says “Move to that position.”
// • What it should not do: Decide how often to update destination, or whether it's “close enough.”
public class MoveToTargetNode : IBehaviorNode
{
    private readonly MovementData _movementData;
    private readonly ITargetResolver _resolver;
    
    public MoveToTargetNode(MovementData movementData)
    {
        _movementData = movementData;
    }

    // Intent in -> Tick -> Status out.
    public BtStatus Tick(BtContext context)
    {
        var controller = context.Controller;
        if (!controller)
        {
            Debug.LogError("[MoveToTargetNode] Controller is missing from BtController");
            return BtStatus.Failure;
        }
        
        var blackboard = controller.Blackboard;
        if (blackboard  == null)
        {
            Debug.LogError("[MoveToTargetNode] Blackboard is missing from BtController");
            return BtStatus.Failure;
        }

        // Pull TargetingData at runtime; supports BT runtime swap, etc.
        var targetingData = blackboard.TargetingData;
        if (targetingData == null)
        {
            Debug.LogError("[MoveToTargetNode] No TargetingData. Will not move.");
            return BtStatus.Failure;
        }

        var movementLogic = blackboard.MovementLogic;
        if (movementLogic == null)
        {
            Debug.LogError("[MoveToTargetNode] movementLogic: null");
            return BtStatus.Failure;
        }
        
        var resolver = blackboard.TargetResolver ?? TargetResolverRegistry.TryGetValue(targetingData.Style);
        if (resolver == null)
        {
            Debug.LogError("[MoveToTargetNode] TargetResolver missing. Skipping.");
            return BtStatus.Failure;
        }
        
        var target = resolver.ResolveTarget(controller.gameObject, targetingData);

        // Optionally: Configure movement logic with this node’s settings
        movementLogic.ApplySettings(_movementData);
        var canMove = blackboard.MovementLogic.TryMoveTo(target.position);

        if (!canMove)
            return BtStatus.Failure;

        return blackboard.MovementLogic.IsAtDestination()
            ? BtStatus.Success
            : BtStatus.Running;
    }
}