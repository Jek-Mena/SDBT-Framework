using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.StatusBuff)]
public class BuffPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var health = entity.GetComponent<HealthSystem>();
        if (health == null) return;
        
        var multiplier = jObject["healthMultiplier"]?.Value<float>() ?? 1f;
        health.Multiply(multiplier);
    }
}