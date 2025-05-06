using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class ComponentEntry
{
    public PluginKey Key;
    public JObject @params;
    
    [JsonConstructor]
    public ComponentEntry(PluginKey key, JObject @params)
    {
        this.Key = key;
        this.@params = @params;
    }
}