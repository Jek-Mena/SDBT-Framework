using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.DashRigidbody)]
public class RigidbodyDashMoverPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var dasher = entity.RequireComponent<RigidbodyDashMover>();
        
        dasher.Initialize(new DashData
        {
            speed = config.RequireFloat(JsonKeys.Dash.StateTimeout),
            tolerance = config.RequireFloat(JsonKeys.Dash.Tolerance),
            stateTimeout = config.RequireFloat(JsonKeys.Dash.StateTimeout)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Dash), entry.@params, nameof(RigidbodyDashMoverPlugin));
    }
}