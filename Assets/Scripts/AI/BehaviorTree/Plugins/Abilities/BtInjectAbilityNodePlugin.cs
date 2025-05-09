using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtInjectAbilityNode)]
public class BtInjectAbilityNodePlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.GetComponent<BtController>();
        if (controller == null) return;
    }

    public void Validate(ComponentEntry entry)
    {
        if (!entry.@params.ContainsKey(JsonKeys.Ability.Id))
        {
            Debug.LogWarning("[BtInjectAbilityNodePlugin] Missing required 'abilityId' in config.");
        }
    }
}