using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNode_Pause)]
public class PauseNodePlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        blackboard.TimedExecutionLogic = entity.GetComponent<ITimedExecutionNode>();
        if(blackboard.TimedExecutionLogic == null)
            Debug.LogError("Timer system not found! PauseNode will not work.");

        blackboard.TimerData = new TimedExecutionData
        {
            duration = config.RequireFloat(JsonKeys.TimedExecution.Duration),
            key = $"Timer:{controller.gameObject.GetInstanceID()}:{BtNodeName.Tasks.Pause}",
            failOnInterrupt = false,
            interruptible = false
        };
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.TimedExecution), entry.@params, nameof(PauseNodePlugin));
    }
}

//TODO Add a validation or check if there is no Plugin.
//PauseNodePlugin was not created and there was no any message that it was missing 