using Newtonsoft.Json.Linq;
using UnityEngine;

public class TimeoutDecoratorNodePlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        blackboard.TimedExecutionLogic = entity.GetComponent<ITimedExecutionNode>();
        if (blackboard.TimedExecutionLogic == null)
        {
            Debug.LogError("[TimeoutNodePlugin] Timer system not found! TimeoutNode will not work.");
            return;
        }

        var context = nameof(TimeoutDecoratorNodePlugin);
        var config = JsonUtils.GetConfig(jObject, context);
        
        var id = controller.gameObject.GetInstanceID();
        blackboard.TimerData = new TimedExecutionData
        {
            Label = $"Timer:{id}:{TimedExecutionKeys.Alias.TimeoutDecorator}",
            Duration = JsonUtils.RequireFloat(config, TimedExecutionKeys.Json.Duration, context),
            Interruptible = jObject.TryGetBool(TimedExecutionKeys.Json.Interruptible, false),
            FailOnInterrupt = jObject.TryGetBool(TimedExecutionKeys.Json.FailOnInterrupt, false)
        };
    }
}