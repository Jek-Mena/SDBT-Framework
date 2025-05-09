using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using UnityEngine;

[Plugin(PluginKey.BtLoadTree)]
public class BtLoadTreePlugin : BasePlugin
{
    private readonly IContextBuilder _contextBuilder = new ContextBuilder();

    public override void Apply(GameObject entity, JObject jObject)
    {
        if (jObject == null)
        {
            Debug.LogError("[BtLoadTreePlugin] config == null; ensure your JSON includes a component: { \"plugin\": \"BtLoadTree\", \"params\": { \"treeId\": \"…\" } }");
            return;
        }

        var controller = entity.RequireComponent<BtController>();
        
        var treeId = jObject[JsonFields.TreeId]?.Value<string>();
        if (string.IsNullOrEmpty(treeId))
        {
            Debug.LogError($"Missing JSON key {JsonFields.TreeId}.");
            return;
        }

        // You must map treeId -> IBehaviorNode tree here
        var context = _contextBuilder.Build(entity);
        controller.InitContext(context);

        var treeJson = BtLoader.LoadJsonTreeBtId(treeId);

        var rootToken = treeJson[JsonFields.Root] ?? treeJson;

        if (rootToken[JsonFields.BtKey] == null)
        {
            Debug.LogError($"[BT Loader] Missing 'btKey' in root node:\n{rootToken.ToString(Formatting.Indented)}");
            return;
        }

        var root = BtTreeBuilder.Build(rootToken, context);
        
        controller.LoadBtFromRunTime(root);
    }
}