using Newtonsoft.Json.Linq;
using System;

public class TimedExecutionNodeFactory<T> : IBtNodeFactory where T : TimedExecutionNode, new()
{
    private readonly string _nodeType;

    public TimedExecutionNodeFactory(string nodeType)
    {
        _nodeType = nodeType;
    }

    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var config = JsonUtils.GetConfig(jObject, nameof(TimedExecutionNodeFactory<T>));

        var data = new TimedExecutionData()
        {
            Label = $"{_nodeType}:{blackboard.GetHashCode()}",
            Duration = config.Value<float?>(TimedExecutionKeys.Json.Duration) ?? 1f,
            StartDelay = config.Value<float?>(TimedExecutionKeys.Json.StartDelay) ?? 0f,
            Interruptible = config.Value<bool?>(TimedExecutionKeys.Json.Interruptible) ?? true,
            FailOnInterrupt = config.Value<bool?>(TimedExecutionKeys.Json.FailOnInterrupt) ?? true,
            ResetOnExit = config.Value<bool?>(TimedExecutionKeys.Json.ResetOnExit) ?? true,
            mode = config.Value<string>(TimedExecutionKeys.Json.Mode) switch
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