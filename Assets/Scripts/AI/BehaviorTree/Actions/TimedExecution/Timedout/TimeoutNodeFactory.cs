using System;
using Newtonsoft.Json.Linq;

[BtNode(BtNodeName.Decorator.Timeout)]
public class TimeoutNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        // Extract the child node
        var childToken = jObject[JsonFields.Children];
        if (childToken == null)
            throw new Exception("[TimeoutNodeFactory] Missing 'children' block in Timeout node.");

        var childNode = build(childToken);

        // Extract config block
        var config = BtJsonUtils.GetConfig(jObject);
        if (config == null)
            throw new Exception("[TimeoutNodeFactory] Missing or invalid 'config' block in Timeout node.");

        // Get timer config
        var duration = config.Value<float?>("duration") ?? 3f;
        var key = config.Value<string>("key") ?? $"Timeout:{blackboard.GetHashCode()}:{Guid.NewGuid()}";

        // Optional flags if you want to extend TimeoutNode later
        // var interruptible = config.Value<bool?>("interruptible") ?? true;
        // var failOnInterrupt = config.Value<bool?>("failOnInterrupt") ?? false;

        // Construct the timeout decorator node
        return new TimeoutNode(childNode, duration, blackboard.TimedExecutionLogic, key);
    }
}