using AI.BehaviorTree.Nodes.Actions.Rotate;
using AI.BehaviorTree.Runtime.Context;
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
            Debug.LogError($"[{scriptName}] No IRotationNode found on '{agent.name}'. " + 
                           "Ensure your AI prefab has a rotation component attached.");
            throw new System.Exception($"[{scriptName}] Rotation logic missing!");
        }
        
        rotationNode.Initialize(new RotationData());
        Debug.Log($"[{scriptName}] {rotationNode} initialize for {agent.name}");   
        
        // Inject StatusEffectManager only if supported
        if (rotationNode is IUsesStatusEffectManager effectUser)
        {
            if (context.Blackboard.StatusEffectManager)
            {
                effectUser.SetStatusEffectManager(context.Blackboard.StatusEffectManager);
                Debug.Log($"[{scriptName}] {nameof(StatusEffectManager)} initialize for {agent.name}");   
            }
        }
        
        blackboard.RotationLogic = rotationNode;
    }
}