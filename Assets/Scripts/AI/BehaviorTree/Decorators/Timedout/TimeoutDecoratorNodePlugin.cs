using Newtonsoft.Json.Linq;
using UnityEngine;

public class TimeoutDecoratorNodePlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        if (!blackboard.TimeExecutionManager)
        {
            Debug.LogError("[TimeoutNodePlugin] Timer system not found! TimeoutNode will not work.");
            return;
        }

        var context = nameof(TimeoutDecoratorNodePlugin);
        var config = JsonUtils.GetConfig(jObject, context);
        
        var id = controller.gameObject.GetInstanceID();
        blackboard.TimerData = new TimedExecutionData
        {
            Label = $"Timer:{id} : Alias: {BtNodeAliases.TimedExecution.TimeoutDecorator}",
            Duration = JsonUtils.RequireFloat(config, BtConfigFields.Common.Duration, context),
            Interruptible = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.Interruptible, false, context),
            FailOnInterrupt = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.FailOnInterrupt, false, context)
        };
    }
}