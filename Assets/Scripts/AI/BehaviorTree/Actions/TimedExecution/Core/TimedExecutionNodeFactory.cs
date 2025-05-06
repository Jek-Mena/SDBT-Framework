using Newtonsoft.Json.Linq;
using System;

public class TimedExecutionNodeFactory<T> : IBtNodeFactory where T : TimedExecutionNode, new()
{
    private readonly string _nodeType;

    public TimedExecutionNodeFactory(string nodeType)
    {
        _nodeType = nodeType;
    }

    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> recurse)
    {
        var config = BtJsonUtils.GetConfig(jObject);

        var data = new TimedExecutionData()
        {
            key = $"{_nodeType}:{blackboard.GetHashCode()}",
            duration = config.Value<float?>(JsonKeys.TimedExecution.Duration) ?? 1f,
            startDelay = config.Value<float?>(JsonKeys.TimedExecution.StartDelay) ?? 0f,
            interruptible = config.Value<bool?>(JsonKeys.TimedExecution.Interruptible) ?? true,
            failOnInterrupt = config.Value<bool?>(JsonKeys.TimedExecution.FailOnInterrupt) ?? true,
            resetOnExit = config.Value<bool?>(JsonKeys.TimedExecution.ResetOnExit) ?? true,
            mode = config.Value<string>(JsonKeys.TimedExecution.Mode) switch
            {
                "Loop" => TimerExecutionMode.Loop,
                "UntilSuccess" => TimerExecutionMode.UntilSuccess,
                "UntilFailure" => TimerExecutionMode.UntilFailure,
                _ => TimerExecutionMode.Normal
            }
        };

        blackboard.TimerData = data;
        return new T();
    }
}