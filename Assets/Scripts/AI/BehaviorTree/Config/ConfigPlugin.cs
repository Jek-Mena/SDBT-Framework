using Newtonsoft.Json.Linq;
using UnityEngine;

public class ConfigData
{
    public JObject RawJson;
}

public class ConfigPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();
        var config = new ConfigData { RawJson = jObject };

        controller.Blackboard.Set(PluginMetaKeys.Core.BtConfig.Plugin, config);
        Debug.Log($"[ConfigPlugin] Injected config into blackboard for '{entity.name}'");
    }
}