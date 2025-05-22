using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class TimeoutDecoratorNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(TimeoutDecoratorNodeFactory);

        // === Extract config ===
        var config = nodeData.Config;
        if (config == null)
            throw new Exception($"[{context}] Missing 'config' for TimeoutDecorator node.");

        if (config.TryGetValue(CoreKeys.Ref, out _))
            config = BtConfigResolver.Resolve(nodeData.Raw, blackboard, context);

        // === Extract data ===
        var timedData = TimedExecutionDataBuilder.FromConfig(config, context);

        // === Validate children (0 or 1 allowed) ===
        var children = nodeData.Children?
            .Select(childToken => buildChildNode(new TreeNodeData((JObject)childToken)))
            .ToList() ?? new();

        if (children.Count > 1)
            throw new Exception($"[{context}] TimeoutDecorator must have 0 or 1 child. Found: {children.Count}");

        var childNode = children.FirstOrDefault();

        // === Get timer system from blackboard ===
        var timer = blackboard.TimeExecutionManager;
        if (!timer)
            throw new Exception($"[{context}] TimeExecutionManager not found in blackboard. Did you forget the plugin/context builder?");

        var node = new TimeoutDecoratorNode(childNode, timedData);
        node.Initialize(blackboard);
        
        return node;
    }
}