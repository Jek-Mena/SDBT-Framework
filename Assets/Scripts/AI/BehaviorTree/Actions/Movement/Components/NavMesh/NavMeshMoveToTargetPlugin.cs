using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNodeMono_MovementNavMesh)]
public class NavMeshMoveToTargetPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var movement = entity.RequireComponent<NavMeshMoveToTarget>();

        var context = nameof(NavMeshMoveToTargetPlugin);

        var config = JsonUtils.GetConfig(jObject, context);

        movement.Initialize(new MovementData
        {
            Speed = JsonUtils.RequireFloat(config, JsonKeys.Movement.Speed, context),
            AngularSpeed = JsonUtils.RequireFloat(config, JsonKeys.Movement.AngularSpeed, context),
            Acceleration = JsonUtils.RequireFloat(config, JsonKeys.Movement.Acceleration, context),
            StoppingDistance = JsonUtils.RequireFloat(config, JsonKeys.Movement.StoppingDistance, context),
            UpdateThreshold = JsonUtils.RequireFloat(config, JsonKeys.Movement.UpdateThreshold, context)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Movement), entry.@params, nameof(NavMeshMoveToTargetPlugin));
    }
}