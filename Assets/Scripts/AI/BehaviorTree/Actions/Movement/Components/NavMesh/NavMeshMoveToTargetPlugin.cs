using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class NavMeshMoveToTargetPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        Debug.Log($"[NavMeshMoveToTargetPlugin] Applying plugin to: {entity.name}");

        var context = nameof(NavMeshMoveToTargetPlugin);
        var config = JsonUtils.GetConfig(jObject, context);

        var data = new MovementData
        {
            Speed = JsonUtils.RequireFloat(config, MovementKeys.Json.Speed, context),
            AngularSpeed = JsonUtils.RequireFloat(config, MovementKeys.Json.AngularSpeed, context),
            Acceleration = JsonUtils.RequireFloat(config, MovementKeys.Json.Acceleration, context),
            StoppingDistance = JsonUtils.RequireFloat(config, MovementKeys.Json.StoppingDistance, context),
            UpdateThreshold = JsonUtils.RequireFloat(config, MovementKeys.Json.UpdateThreshold, context)
        };
        
        var mover = entity.RequireComponent<NavMeshMoveToTarget>();
        if (mover != null)
        {
            mover.Initialize(data);
            Debug.Log($"[NavMeshMoveToTargetPlugin] Initialized with " +
                      $"Speed: {data.Speed} " +
                      $"AngularSpeed: {data.AngularSpeed} " +
                      $"Acceleration: {data.Acceleration} " +
                      $"StoppingDistance: {data.StoppingDistance} " +
                      $"UpdateThreshold: {data.UpdateThreshold}");
        }
        else
        {
            Debug.LogError($"[NavMeshMoveToTargetPlugin] NavMeshMoveToTarget not found on {entity.name}");
        }
    }

    public static PluginMetadata Metadata => new()
    {
        PluginKey = MovementKeys.Plugin.NavMeshMoveToTarget,
        SchemaKey = MovementKeys.Schema.NavMeshMoveToTarget,
        PluginType = typeof(NavMeshMoveToTargetPlugin),
        Domain = MovementKeys.Domain.Default,
        ExecutionPhase = MovementKeys.ExecutionPhase.Default,
        DependsOn = Array.Empty<Type>() // Or null, based on PluginRegistry default handling
    };


}