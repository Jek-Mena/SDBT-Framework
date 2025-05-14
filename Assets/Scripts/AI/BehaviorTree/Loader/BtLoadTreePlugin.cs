using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class BtLoadTreePlugin : BasePlugin
{
    private IContextBuilder ContextBuilder => BtServices.ContextBuilder;

    public override void Apply(GameObject entity, JObject jObject)
    {
        if (jObject == null)
        {
            Debug.LogError("[BtLoadTreePlugin] config == null; ensure your JSON includes a component: { \"plugin\": \"BtLoadTree\", \"params\": { \"treeId\": \"…\" } }");
            return;
        }

        var controller = entity.RequireComponent<BtController>();
        
        var treeId = jObject[CoreKeys.TreeId]?.Value<string>();
        if (string.IsNullOrEmpty(treeId))
        {
            Debug.LogError($"Missing JSON key {CoreKeys.TreeId}.");
            return;
        }

        var context = controller.Blackboard;
        controller.InitContext(context);

        var treeJson = BtLoader.LoadJsonTreeBtId(treeId);

        var rootToken = treeJson[CoreKeys.Root] ?? treeJson;

        if (rootToken[CoreKeys.Type] == null)
        {
            Debug.LogError($"[BT Loader] Missing 'btKey' in root node:\n{rootToken.ToString(Formatting.Indented)}");
            return;
        }

        var root = BtTreeBuilder.Build(rootToken, context);
        controller.LoadBtFromRunTime(root);
    }
}