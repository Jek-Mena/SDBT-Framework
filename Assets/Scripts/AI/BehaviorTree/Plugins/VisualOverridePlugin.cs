using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.VisualOverride)]
public class VisualOverridePlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        if (config["tint"] != null && ColorUtility.TryParseHtmlString(config["tint"].ToString(), out var color))
        {
            var renderer = entity.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material.color = color;
        }

        if (config["fx"] != null)
        {
            var fxName = config["fx"].ToString();
            var fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
            if (fxPrefab != null)
            {
                GameObject.Instantiate(fxPrefab, entity.transform.position, Quaternion.identity, entity.transform);
            }
        }
    }
}