using UnityEngine;

// TODO: Future expansion: Let the system support the usage of multiple possible movement components by iterating or support some selection logic, but for now, keep it simple and DRY.
public class MovementBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(MovementBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
        
        // Try to find any IMovementNode component (generic, could be NavMeshMoveToTarget or another)
        var movementNode = agent.GetComponent<IMovementNode>();
        if (movementNode == null) 
        {
            Debug.LogError($"[{scriptName}] No IMovementNode found on '{agent.name}'. " + 
                           "Ensure your AI prefab has a movement component attached.");
            throw new System.Exception($"[{scriptName}] Movement logic missing!");
        }

        // Initialize with dummy data just to guarantee pipeline safety (real profile will come from the BT node)
        movementNode.Initialize(new MovementData());
        
        // Inject StatusEffectManager only if supported
        if (movementNode is IUsesStatusEffectManager effectUser)
        {
            if (context.Blackboard.StatusEffectManager) 
                effectUser.SetStatusEffectManager(context.Blackboard.StatusEffectManager);
        }

        blackboard.MovementLogic = movementNode;
        Debug.Log($"[Inject: {scriptName}]");
    }
}