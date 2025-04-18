using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtLoadTree)]
public class BtLoadTreePlugin : BasePlugin
{
    private readonly IContextBuilder _contextBuilder = new DefaultContextBuilder();

    public override void Apply(GameObject entity, JObject config)
    {
        var controller = entity.RequireComponent<BtController>();
        
        var treeId = config[JsonKeys.BehaviorTree.ConfigId]?.ToString();
        if (string.IsNullOrEmpty(treeId))
        {
            Debug.LogError("Missing behaviour tree id.");
            return;
        }

        // You must map treeId -> IBehaviorNode tree here
        var context = _contextBuilder.Build(entity);
        controller.InitContext(context);

        var root = BehaviorTreeFactory.BuildTree(treeId, context);
        controller.LoadBtFromRunTime(root);
    }
}

//TODO: Change the switch statement
public static class BehaviorTreeFactory
{
    public static IBehaviorNode BuildTree(string id, Blackboard bb)
    {
        return id switch
        {
            // Basic: Move straight toward target
            JsonKeys.BehaviorTree.Behavior.BasicChase => new BehaviorLeafNode(new MoveBehavior(bb)),

            // Dash-based enemy logic (just a dash test for now)
            JsonKeys.BehaviorTree.Behavior.DashOnly => new BehaviorLeafNode(new DashBehavior(bb)),

            // TODO: Add more behaviors as you build them
            // "flankAndDash" => new SequenceNode(...),
            // "trapReposition" => new ParallelNode(...),

            _ => null
        };
    }
}
