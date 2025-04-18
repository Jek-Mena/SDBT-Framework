using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class ComponentEntry
{
    public PluginKey type;
    public JObject @params;
    
    [JsonConstructor]
    public ComponentEntry(PluginKey type, JObject @params)
    {
        this.type = type;
        this.@params = @params;
    }
}