using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class NavMeshMoveToTargetPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var context = nameof(NavMeshMoveToTargetPlugin);
        var controller = entity.RequireComponent<BtController>();
        var mover = entity.RequireComponent<NavMeshMoveToTarget>();
        var blackboard = controller.Blackboard;

        var config = blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin)
            ?.RawJson?[CoreKeys.SettingsBlock.Movement] as JObject;

        if (config == null)
        {
            Debug.LogError($"[{context}] Missing config block: '{CoreKeys.SettingsBlock.Movement}'");
            return;
        }

        // Assigns the movement logic to the blackboard for runtime use.
        blackboard.MovementLogic = mover;
        
        var data = MovementDataBuilder.FromConfig(config, context);

        mover.SetStatusEffectManager(blackboard.StatusEffectManager);
        mover.Initialize(data);
        
        Debug.Log($"[NavMeshMoveToTargetPlugin] Plugins applied to {entity.name} " +
                  $"and initialized with " +
                  $"Speed: {data.Speed} " +
                  $"AngularSpeed: {data.AngularSpeed} " +
                  $"Acceleration: {data.Acceleration} " +
                  $"StoppingDistance: {data.StoppingDistance} " +
                  $"UpdateThreshold: {data.UpdateThreshold}"); 
    }
}