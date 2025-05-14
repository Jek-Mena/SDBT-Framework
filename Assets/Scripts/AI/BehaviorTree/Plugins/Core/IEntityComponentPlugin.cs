using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Interface for game entity plugin components.
/// Every plugin must expose a static metadata object that defines how it integrates with the system.
/// </summary>
public interface IEntityComponentPlugin
{
    /// <summary>
    /// Called to apply this plugin's behavior to a game object using parsed JSON.
    /// </summary>
    void Apply(GameObject entity, JObject jObject);
}