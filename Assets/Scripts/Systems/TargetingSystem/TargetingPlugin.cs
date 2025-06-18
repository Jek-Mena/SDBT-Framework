using System.Collections.Generic;
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

        // TODO replace or rename targetingProfiles into skillBlock. Written 12-06-2025 9:43pm 
        // Get targetingProfiles block for multiple profiles

        var profileBlock = configData.RawJson[CoreKeys.ProfilesBlock.Targeting] as JObject;
        if (profileBlock != null)
        {
            var profiles = new Dictionary<string, TargetingData>();
            foreach (var prop in profileBlock.Properties())
            {
                profiles[prop.Name] = TargetingDataBuilder.FromConfig(prop.Value as JObject, prop.Name);
            }

            blackboard.TargetingProfiles = profiles;
            Debug.Log("[TargetingPlugin] Injected TargetingProfiles into blackboard.");
            
            // Optional: For backward compatibility, you may assign a default TargetingData
            // e.g., use "ChaseTarget" as the default if present
            if (profiles.TryGetValue("ChaseTarget", out var defaultProfile))
            {
                blackboard.TargetingData = defaultProfile;
                blackboard.TargetResolver = TargetResolverRegistry.ResolveOrClosest(defaultProfile.Style);
                blackboard.Target = blackboard.TargetResolver.ResolveTarget(entity, defaultProfile);
            }
            return; // We're done; don't process old targeting block
        }

        // Old: Fallback for single targeting block (legacy support)
        var targetingConfig = configData.RawJson[CoreKeys.ProfilesBlock.Targeting] as JObject;
        if (targetingConfig == null)
        {
            Debug.LogError($"[{context}] Missing 'targeting' block in BtConfig.");
            return;
        }

        // Inject TargetingData into the blackboard using the configuration data.
        blackboard.TargetingData = TargetingDataBuilder.FromConfig(targetingConfig, context);

        // Retrieve the appropriate TargetResolver based on the targeting style.
        blackboard.TargetResolver = TargetResolverRegistry.ResolveOrClosest(blackboard.TargetingData.Style);

        // Resolve and assign the target entity based on the configured targeting style.
        blackboard.Target = blackboard.TargetResolver.ResolveTarget(entity, blackboard.TargetingData);

        Debug.Log("[TargetingPlugin] TargetingData injected into blackboard.");
    }
}