using System;

public class BehaviorTreeAssignmentModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(BehaviorTreeAssignmentModule);
        
        // Get the entity config from blackboard (always via canonical key)
        var config = context.Blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin)?.RawJson;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing entity config in blackboard!");

        // Find the BT tree token in config (usually "tree" key or nested in "params")
        var treeToken = config[CoreKeys.Tree] ?? config[CoreKeys.Params]?[CoreKeys.Tree];
        if (treeToken == null)
            throw new Exception($"[{scriptName}] No 'tree' field found in entity config!");

        // Build the root behavior node
        var rootNode = BtTreeBuilder.LoadFromToken(treeToken, context);
        
        // Assign to controller
        context.Controller.SetTree(rootNode);
        
        UnityEngine.Debug.Log($"[{scriptName}] BT root node assigned to controller.");
    }
}