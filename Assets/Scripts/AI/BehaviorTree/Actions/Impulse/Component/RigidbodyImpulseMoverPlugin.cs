using Newtonsoft.Json.Linq;
using UnityEngine;

public class RigidbodyImpulseMoverPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var rbImpulseNode = entity.RequireComponent<RigidbodyImpulseNode>();

        var context = nameof(RigidbodyImpulseMoverPlugin);

        // Parse config
        var config = JsonUtils.GetConfig(jObject, context);

        rbImpulseNode.Initialize(new ImpulseMoverData
        {
            ImpulseStrength = JsonUtils.RequireFloat(config, MovementKeys.Json.ImpulseStrength, context),
            Tolerance = JsonUtils.RequireFloat(config, MovementKeys.Json.Tolerance, context),
            StateTimeout= JsonUtils.RequireFloat(config, MovementKeys.Json.StateTimeout, context)
        });
    }
}