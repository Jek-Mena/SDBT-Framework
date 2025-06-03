using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuaternionLookAtPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var context = nameof(QuaternionLookAtPlugin);
        var controller = entity.RequireComponent<BtController>();
        var rotator  = entity.RequireComponent<QuaternionLookAt>();
        var blackboard = controller.Blackboard;
        
        var config = blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin)
            ?.RawJson?[CoreKeys.ParamSections.Rotation] as JObject;

        if (config == null)
        {
            Debug.LogError($"[{context}] Missing {CoreKeys.ParamSections.Rotation} block in config.");
            return;
        }
        
        // Assigns the movement logic to the blackboard for runtime use.
        blackboard.RotationLogic = rotator;
        
        var data = RotationDataBuilder.FromConfig(config, context);
        
        // Initialize the rotator with the data.
        rotator.SetStatusEffectManager(blackboard.StatusEffectManager);
        rotator.Initialize(data); 
    }
}