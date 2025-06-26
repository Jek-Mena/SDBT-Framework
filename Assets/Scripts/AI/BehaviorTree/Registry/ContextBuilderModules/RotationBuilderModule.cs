using UnityEngine;

public class RotationBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(RotationBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
        
        // Try to find any IMovementNode component (generic, could be NavMeshMoveToTarget or another)
        var rotationNode = agent.GetComponent<IRotationNode>();
        if (rotationNode == null) 
        {
            Debug.LogError($"[{scriptName}] No IMovementNode found on '{agent.name}'. " + 
                           "Ensure your AI prefab has a movement component attached.");
            throw new System.Exception("[{scriptName}] Movement logic missing!");
        }
        
        blackboard.RotationLogic = rotationNode;
        Debug.Log($"[Inject: {scriptName}]");   
    }
}