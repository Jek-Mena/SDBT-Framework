using Newtonsoft.Json;
using System.Collections.Generic;

public class BaseEntityData
{
    /// <summary> Unique identifier for this entity config. Example: "enemy_standard_chaser" </summary>
    public string EntityId;
    /// <summary> Path to the prefab asset this entity should instantiate. Example: "Prefabs/Enemy/Standard/enemy_standard_chaser" </summary>
    public string Prefab;
    /// <summary> Serialized component plugins applied to the entity. Each plugin maps to a behavior (e.g., BT tree loader, movement system).</summary>
    [JsonConverter(typeof(ComponentEntryObjectConverter))]
    public List<ComponentEntry> Components = new();
    // Add more optional sections as needed
}