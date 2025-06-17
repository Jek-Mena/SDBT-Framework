using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BtLoadTreePlugin : BasePlugin
{
    private IContextBuilder ContextBuilder => BtServices.ContextBuilder;

    public override void Apply(GameObject entity, JObject jObject)
    {
        var scriptName = nameof(BtLoadTreePlugin);
        var controller = entity.RequireComponent<BtController>();
        var context = controller.Context;
        var blackboard = context.Blackboard;
        
        if (blackboard == null)
        {
            Debug.LogError("[BtLoadTreePlugin] blackboard == null;");
            return;
        }

        // Inject BtConfig if present
        if (jObject.TryGetValue(CoreKeys.Config, out var configToken))
        {
            var configData = new ConfigData { RawJson = (JObject)configToken };
            blackboard.Set(PluginMetaKeys.Core.BtConfig.Plugin, configData);
        }

        // Load behavior tree from the inline or "params" block
        var treeToken = jObject[CoreKeys.Tree] ?? jObject[CoreKeys.Params]?[CoreKeys.Tree];
        if (treeToken == null)
            throw new Exception($"[{scriptName}] Missing 'tree' field (inline [{CoreKeys.Params}] or treeId: [{CoreKeys.Tree}])");
        
        var root = BtTreeBuilder.LoadFromToken(treeToken, context);
        controller.SetTree(root);

        Debug.Log($"[{scriptName}] Behavior tree built and assigned.");
    }
}