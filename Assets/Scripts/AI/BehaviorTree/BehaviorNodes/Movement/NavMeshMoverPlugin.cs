using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNode_MovementNavMesh)]
public class NavMeshMoverPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var movement = entity.RequireComponent<NavMeshMover>();

        movement.Initialize(new MovementData
        {
            Speed = config.RequireFloat(JsonKeys.Movement.Speed),
            Acceleration = config.RequireFloat(JsonKeys.Movement.Acceleration),
            StoppingDistance = config.RequireFloat(JsonKeys.Movement.StoppingDistance)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Movement), entry.@params, nameof(NavMeshMoverPlugin));
    }
}