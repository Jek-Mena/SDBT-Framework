using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Plugin(PluginKey.BtNode_Timeout)]
public class TimeoutNodePlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        blackboard.TimedExecutionLogic = entity.GetComponent<ITimedExecutionNode>();
        if (blackboard.TimedExecutionLogic == null)
        {
            Debug.LogError("[TimeoutNodePlugin] Timer system not found! TimeoutNode will not work.");
            return;
        }

        var id = controller.gameObject.GetInstanceID();
        blackboard.TimerData = new TimedExecutionData
        {
            duration = config.RequireFloat(JsonKeys.TimedExecution.Duration),
            key = $"Timer:{id}:{BtNodeName.Tasks.Timeout}",
            interruptible = config.TryGetBool(JsonKeys.TimedExecution.Interruptible, false),
            failOnInterrupt = config.TryGetBool(JsonKeys.TimedExecution.FailOnInterrupt, false)
        };
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.TimedExecution), entry.@params, nameof(TimeoutNodePlugin));
    }
}

/*public override void Apply(GameObject entity, JObject config)
{
    var controller = entity.RequireComponent<BtController>();
    var blackboard = controller.Blackboard;

    blackboard.TimedExecutionLogic = entity.GetComponent<ITimedExecutionNode>();
    if (blackboard.TimedExecutionLogic == null)
    {
        Debug.LogError("Timer system not found! TimeoutNode will not work.");
    }

    blackboard.TimerData = new TimedExecutionData()
    {
        duration = config.RequireFloat(JsonKeys.TimedExecution.Duration),
        key = $"Timer:{controller.gameObject.GetInstanceID()}:{BtNodeName.Decorator.Timeout}",
        failOnInterrupt = config.TryGetBool(JsonKeys.TimedExecution.FailOnInterrupt, false),
        interruptible = config.TryGetBool(JsonKeys.TimedExecution.Interruptible, false)
    };

    Debug.Log($"[TimeoutNodePlugin] Injected TimerData: duration={blackboard.TimerData.duration}, key={blackboard.TimerData.key}");
}*/