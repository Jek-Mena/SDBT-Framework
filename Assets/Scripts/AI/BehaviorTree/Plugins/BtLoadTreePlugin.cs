using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtLoadTree)]
public class BtLoadTreePlugin : BasePlugin
{
    private readonly IContextBuilder _contextBuilder = new ContextBuilder();

    public override void Apply(GameObject entity, JObject config)
    {
        var controller = entity.RequireComponent<BtController>();
        
        var treeId = config[JsonKeys.BehaviorTree.TreeId]?.Value<string>();
        if (string.IsNullOrEmpty(treeId))
        {
            Debug.LogError("Missing behaviour tree id.");
            return;
        }

        // You must map treeId -> IBehaviorNode tree here
        var context = _contextBuilder.Build(entity);
        controller.InitContext(context);

        var root = BtFactory.BuildTree(treeId, context);
        controller.LoadBtFromRunTime(root);
    }
}