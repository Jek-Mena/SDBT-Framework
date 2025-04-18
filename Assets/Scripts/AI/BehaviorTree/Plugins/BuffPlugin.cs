using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.StatusBuff)]
public class BuffPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var health = entity.GetComponent<HealthSystem>();
        if (health == null) return;
        
        var multiplier = config["healthMultiplier"]?.Value<float>() ?? 1f;
        health.Multiply(multiplier);
    }
}