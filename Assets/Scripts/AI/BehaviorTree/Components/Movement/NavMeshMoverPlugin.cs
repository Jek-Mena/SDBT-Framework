using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.MovementNavMesh)]
public class NavMeshMoverPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var movement = entity.RequireComponent<NavMeshMover>();

        movement.Initialize(new MovementData
        {
            speed = config.RequireFloat(JsonKeys.Movement.Speed),
            acceleration = config.RequireFloat(JsonKeys.Movement.Acceleration),
            stoppingDistance = config.RequireFloat(JsonKeys.Movement.StoppingDistance)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Movement), entry.@params, nameof(NavMeshMoverPlugin));
    }
}