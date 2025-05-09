using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.VisualOverride)]
public class VisualOverridePlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        if (jObject["tint"] != null && ColorUtility.TryParseHtmlString(jObject["tint"].ToString(), out var color))
        {
            var renderer = entity.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material.color = color;
        }

        if (jObject["fx"] != null)
        {
            var fxName = jObject["fx"].ToString();
            var fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
            if (fxPrefab != null)
            {
                GameObject.Instantiate(fxPrefab, entity.transform.position, Quaternion.identity, entity.transform);
            }
        }
    }
}