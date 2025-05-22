using Newtonsoft.Json.Linq;

/// <summary>
/// Implement on any node/component that should be reconfigurable at runtime
/// via JSON-based updates (e.g., procedural AI, live tuning, mod support).
/// </summary>
public interface IJsonConfigurable
{
    /// <summary>
    /// Reconfigure this object instance at runtime using a JSON config block.
    /// </summary>
    /// <param name="config">Configuration data (JObject).</param>
    void LoadConfig(JObject config);
}
