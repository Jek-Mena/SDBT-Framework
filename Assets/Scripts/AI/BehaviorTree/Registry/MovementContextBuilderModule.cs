using UnityEngine;

// TODO: Future expansion: Let the system support the usage of multiple possible movement components by iterating or support some selection logic, but for now, keep it simple and DRY.
public class MovementContextBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(MovementContextBuilderModule);
        var entity = context.Agent;
        var blackboard = context.Blackboard;
        
        // Try to find any IMovementNode component (generic, could be NavMeshMoveToTarget or another)
        var movementNode = entity.GetComponent<IMovementNode>();
        if (movementNode == null) 
        {
            Debug.LogError($"[{scriptName}] No IMovementNode found on '{entity.name}'. " + 
                           "Ensure your AI prefab has a movement component attached.");
            throw new System.Exception($"[{scriptName}] Movement logic missing!");
        }
        
        blackboard.MovementLogic = movementNode;
        Debug.Log($"[Inject: {scriptName}]");
    }
}