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
            Key = $"{_nodeType}:{blackboard.GetHashCode()}",
            Duration = config.Value<float?>(JsonKeys.TimedExecution.Duration) ?? 1f,
            StartDelay = config.Value<float?>(JsonKeys.TimedExecution.StartDelay) ?? 0f,
            Interruptible = config.Value<bool?>(JsonKeys.TimedExecution.Interruptible) ?? true,
            FailOnInterrupt = config.Value<bool?>(JsonKeys.TimedExecution.FailOnInterrupt) ?? true,
            ResetOnExit = config.Value<bool?>(JsonKeys.TimedExecution.ResetOnExit) ?? true,
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