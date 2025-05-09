using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNodeMono_ImpulseRigidbody)]
public class RigidbodyImpulseMoverPlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var dasher = entity.RequireComponent<RigidbodyImpulseNode>();

        var context = nameof(RigidbodyImpulseMoverPlugin);

        // Parse config
        var config = JsonUtils.GetConfig(jObject, context);

        dasher.Initialize(new ImpulseMoverData
        {
            ImpulseStrength = JsonUtils.RequireFloat(config, JsonKeys.Impulse.ImpulseStrength, context),
            Tolerance = JsonUtils.RequireFloat(config, JsonKeys.Impulse.Tolerance, context),
            StateTimeout= JsonUtils.RequireFloat(config, JsonKeys.Impulse.StateTimeout, context)
        });
    }

    public void Validate(ComponentEntry entry)
    {
        JTokenExtensions.ValidateRequiredKeys(typeof(JsonKeys.Impulse), entry.@params, nameof(RigidbodyImpulseMoverPlugin));
    }
}