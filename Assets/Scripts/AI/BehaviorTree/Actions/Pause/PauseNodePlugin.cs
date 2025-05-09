using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNode_Pause)]
public class PauseNodePlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        blackboard.TimedExecutionLogic = entity.GetComponent<ITimedExecutionNode>();
        if(blackboard.TimedExecutionLogic == null)
            Debug.LogError("Timer system not found! PauseNode will not work.");

        var context = nameof(PauseNodePlugin);

        var config = JsonUtils.GetConfig(jObject, context);

        blackboard.TimerData = new TimedExecutionData
        {
            Duration = JsonUtils.RequireFloat(config, JsonKeys.TimedExecution.Duration, context),
            Key = $"Timer:{controller.gameObject.GetInstanceID()}:{JsonLiterals.Behavior.TimedExecution.Pause}",
            FailOnInterrupt = false,
            Interruptible = false
        };
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.TimedExecution), entry.@params, nameof(PauseNodePlugin));
    }
}

//TODO Add a validation or check if there is no Plugin.
//PauseNodePlugin was not created and there was no any message that it was missing 