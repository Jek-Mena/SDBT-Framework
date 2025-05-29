using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Plugin to inject TargetingData into the blackboard from the entity config.
/// Run after the ConfigPlugin, before the BT execution.
/// </summary>
public class TargetingPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var context = nameof(TargetingPlugin);
        var controller = entity.RequireComponent<BtController>();
        var blackboard = controller.Blackboard;
        
        // Get config injected by ConfigPlugin
        if(!blackboard.TryGet<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin, out var configData))
        {
            Debug.LogError($"[{context}] BtConfig missing from blackboard!");
            return;
        }

        var targetingConfig = configData.RawJson[CoreKeys.SettingsBlock.Targeting] as JObject;
        if (targetingConfig == null)
        {
            Debug.LogError($"[{context}] Missing 'targeting' block in BtConfig.");
            return;
        }

        // Inject TargetingData into the blackboard using the configuration data.
        // This step ensures the TargetResolver and Target are properly initialized.
        blackboard.TargetingData = TargetingDataBuilder.FromConfig(targetingConfig, context);
        
        // Retrieve the appropriate TargetResolver based on the targeting style.
        blackboard.TargetResolver = TargetResolverRegistry.Get(blackboard.TargetingData.Style);
        
        // Resolve and assign the target entity based on the configured targeting style.
        blackboard.Target = blackboard.TargetResolver.ResolveTarget(entity, blackboard.TargetingData);
        
        Debug.Log("[TargetingPlugin] TargetingData injected into blackboard.");
    }
}