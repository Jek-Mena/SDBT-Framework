using Newtonsoft.Json.Linq;
using UnityEngine;

public class PauseNodePlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        if(blackboard.TimedExecutionLogic == null)
            Debug.LogError("[PauseNodePlugin] TimedExecutionLogic is not initialized. Did you forget to register TimedExecutionContextBuilder?");

        var context = nameof(PauseNodePlugin);
        var config = JsonUtils.GetConfig(jObject, context);

        blackboard.TimerData = new TimedExecutionData
        {
            Duration = JsonUtils.RequireFloat(config, TimedExecutionKeys.Json.Duration, context),
            Label = $"Timer:{controller.gameObject.GetInstanceID()}:{TimedExecutionKeys.Alias.Pause}",
            FailOnInterrupt = false,
            Interruptible = false
        };
    }
}