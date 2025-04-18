using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IEntityComponentPlugin
{
    PluginKey Key { get; }
    void Apply(GameObject entity, JObject config);
}