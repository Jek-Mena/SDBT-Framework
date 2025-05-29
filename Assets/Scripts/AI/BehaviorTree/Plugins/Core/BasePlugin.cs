using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Abstract base class for all component plugins.
/// Optional convenience if most plugins don't need extra logic besides Apply.
/// </summary>
public abstract class BasePlugin : IEntityComponentPlugin
{
    /// <summary>
    /// Apply behavior to the given entity using this plugin's config.
    /// Override in derived classes.
    /// </summary>
    public abstract void Apply(GameObject entity, JObject jObject);
}