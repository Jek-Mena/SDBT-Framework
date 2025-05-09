using System;
using Newtonsoft.Json.Linq;

public class TimeoutDecoratorNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        // 1. Build child node
        var childToken = jObject[JsonFields.Children];
        if (childToken == null)
            throw new Exception("[TimeoutNodeFactory] Missing 'children' block in Timeout node.");

        var childNode = build(childToken);

        var context = nameof(TimeoutDecoratorNodeFactory);
        // 2. Parse config
        var config = JsonUtils.GetConfig(jObject, context);

        // 3. Key Logic: required or fallback 
        var key = TimerKeyBuilder.Build(config, JsonLiterals.Behavior.TimedExecution.Timeout, blackboard, context);

        // TODO: Optional flags if you want to extend TimeoutNode later.
        // These are not working and need to use the current implementation.
        // var interruptible = config.Value<bool?>("interruptible") ?? true;
        // var failOnInterrupt = config.Value<bool?>("failOnInterrupt") ?? false;

        // Duration (fail-fast)
        var duration = JsonUtils.RequireFloat(config, JsonKeys.TimedExecution.Duration, context);
        
        // Construct the timeout decorator node
        return new TimeoutDecoratorNode(childNode, duration, blackboard.TimedExecutionLogic, key);
    }
}