using System;
using Newtonsoft.Json.Linq;

/// <summary>
/// [2025-06-25] Obsolete
/// This is the old Approach:
/// The assignment module looks for a single "behaviorTree" field in the entity config (config[CoreKeys.BehaviorTree]).
/// It loads that as the one-and-only tree for the agent.
///
/// The New (Modular) Approach:
/// You no longer use a single field—trees are now selected and switched via switch profiles in the blackboard, not by a top-level "behaviorTree" field.
/// All tree assignment is now dynamic, depending on stimulus and conditions.
/// </summary>
[System.Obsolete]
public class BehaviorTreeAssignmentModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(BehaviorTreeAssignmentModule);
        
        // Get the entity config from blackboard (always via canonical key)
        var config = context.Blackboard.Get<EntityRuntimeData>(BlackboardKeys.EntityConfig).Definition.Config;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing entity config in blackboard!");

        // Find the BT tree token in config
        var treeToken = config[CoreKeys.BehaviorTree] ?? config[CoreKeys.Params]?[CoreKeys.BehaviorTree];
        if (treeToken == null)
            throw new Exception($"[{scriptName}] No '{CoreKeys.BehaviorTree}' field found in entity config!");

        // Build the root behavior node
        var rootNode = BtTreeBuilder.LoadFromToken(treeToken, context);
        
        // Assign to controller
        //context.Controller.SetTree(rootNode);
        
        UnityEngine.Debug.Log($"[{scriptName}] BT root node assigned to controller.");
    }
}