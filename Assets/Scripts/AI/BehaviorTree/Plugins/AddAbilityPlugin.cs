using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtInjectAbilityNode)]
public class AddAbilityPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var bt = entity.GetComponent<BtController>();
        if (bt == null) return;

        var ability = config["ability"]?.ToString();
        if (!string.IsNullOrEmpty(ability))
        {
            bt.AddAbilityNode(ability); // You must implement AddAbilityNode logic inside the controller
        }
    }
}