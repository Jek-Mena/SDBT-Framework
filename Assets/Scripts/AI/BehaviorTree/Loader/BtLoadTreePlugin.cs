using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BtLoadTreePlugin : BasePlugin
{
    private IContextBuilder ContextBuilder => BtServices.ContextBuilder;

    public override void Apply(GameObject entity, JObject jObject)
    {
        var context = nameof(BtLoadTreePlugin);
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

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

        // 🔨 Load behavior tree from inline or "params" block
        var treeToken = jObject[CoreKeys.Tree]
                        ?? jObject[CoreKeys.Params]?[CoreKeys.Tree];

        if (treeToken == null)
            throw new Exception($"[{context}] Missing 'tree' field (inline [{CoreKeys.Params}] or treeId: [{CoreKeys.Tree}])");

        IBehaviorNode root;

        if (treeToken.Type == JTokenType.Object)
        {
            // Inline tree definition
            root = BtTreeBuilder.Build(treeToken, blackboard);
        }
        else if (treeToken.Type == JTokenType.String)
        {
            var treeId = treeToken.ToString();
            root = BtTreeBuilder.Load(treeId, blackboard);
        }
        else
        {
            throw new Exception(
                $"[{context}] Invalid 'tree' format: expected JObject or string, got: {treeToken.Type}");
        }

        controller.SetTree(root);

        Debug.Log($"[{context}] Behavior tree built and assigned.");
    }
}