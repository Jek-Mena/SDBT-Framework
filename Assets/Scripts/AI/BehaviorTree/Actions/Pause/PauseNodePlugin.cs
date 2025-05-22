using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Represents a plugin for pausing the behavior tree execution of an entity.
/// </summary>
/// <remarks>
/// The <see cref="PauseNodePlugin"/> is responsible for configuring a timed execution context
/// within the behavior tree of an entity. It ensures that the necessary timer data is initialized
/// and applied to the entity's blackboard, enabling controlled pauses in the behavior tree execution.
/// </remarks>
public class PauseNodePlugin : BasePlugin
{
    
    /// <summary>
    /// Configures the behavior of the specified entity by applying the PauseNodePlugin logic.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="GameObject"/> representing the entity to which the plugin logic will be applied.
    /// </param>
    /// <param name="jObject">
    /// A JSON object containing configuration data required to initialize the plugin.
    /// </param>
    /// <remarks>
    /// This method sets up a timed execution context for the entity's behavior tree, ensuring that
    /// the necessary configuration is retrieved and applied. If the required timed execution logic
    /// is not initialized, an error is logged.
    /// </remarks>
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;

        if(!blackboard.TimeExecutionManager)
            Debug.LogError("[PauseNodePlugin] TimeExecutionManager is not initialized. Did you forget to set the TimeExecutionManager in the 'BtBlackboardBuilder'?");

        var context = nameof(PauseNodePlugin);
        var config = JsonUtils.GetConfig(jObject, context);

        blackboard.TimerData = new TimedExecutionData
        {
            Duration = JsonUtils.GetFloatOrDefault(config, BtConfigFields.Common.Duration, 3, context),
            Label = $"Timer:{controller.gameObject.GetInstanceID()} : Alias: {BtNodeAliases.TimedExecution.Pause}",
            FailOnInterrupt = false,
            Interruptible = false
        };
    }
}