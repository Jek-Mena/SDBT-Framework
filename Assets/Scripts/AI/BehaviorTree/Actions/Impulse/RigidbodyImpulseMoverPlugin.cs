using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNodeMono_ImpulseRigidbody)]
public class RigidbodyImpulseMoverPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var dasher = entity.RequireComponent<RigidbodyImpulseNode>();
        
        dasher.Initialize(new ImpulseMoverData
        {
            ImpulseStrength = config.RequireFloat(JsonKeys.Impulse.ImpulseStrength),
            Tolerance = config.RequireFloat(JsonKeys.Impulse.Tolerance),
            StateTimeout = config.RequireFloat(JsonKeys.Impulse.StateTimeout)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Impulse), entry.@params, nameof(RigidbodyImpulseMoverPlugin));
    }
}