using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class BaseEntityData
{
    public string id;
    public string prefab;

    [JsonConverter(typeof(ComponentEntryArrayConverter))]
    public List<ComponentEntry> components = new();
    // Add more optional sections as needed
}