using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtInjectAbilityNode)]
public class BtInjectAbilityNodePlugin : BasePlugin, IValidatablePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var controller = entity.GetComponent<BtController>();
        if (controller == null) return;

        var abilityId = config[JsonKeys.Ability.Id]?.ToString();
        if (!string.IsNullOrWhiteSpace(abilityId))
            controller.AddAbilityNode(abilityId);
    }

    public void Validate(ComponentEntry entry)
    {
        if (!entry.@params.ContainsKey(JsonKeys.Ability.Id))
        {
            Debug.LogWarning("[BtInjectAbilityNodePlugin] Missing required 'abilityId' in config.");
        }
    }
}