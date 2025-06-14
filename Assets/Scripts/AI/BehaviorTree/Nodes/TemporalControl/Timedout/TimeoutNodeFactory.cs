using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class TimeoutNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var blackboard = context.Blackboard;
        var scriptName = nameof(TimeoutNodeFactory);

        // === Extract config ===
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing 'config' for TimeoutDecorator node.");

        // === Extract data ===
        var timedData = TimedExecutionDataBuilder.FromConfig(config, scriptName);

        // === Validate children (0 or 1 allowed) ===
        var children = nodeData.Children?
            .Select(childToken => buildChildNode(new TreeNodeData((JObject)childToken)))
            .ToList() ?? new();

        if (children.Count > 1)
            throw new Exception($"[{scriptName}] TimeoutDecorator must have 0 or 1 child. Found: {children.Count}");

        var childNode = children.FirstOrDefault();

        // === Get timer system from blackboard ===
        var timer = blackboard.TimeExecutionManager;
        if (!timer)
            throw new Exception($"[{scriptName}] TimeExecutionManager not found in blackboard. Did you forget the plugin/context builder?");

        var node = new TimeoutNode(childNode, timedData);
        node.Initialize(blackboard);
        
        return node;
    }
}