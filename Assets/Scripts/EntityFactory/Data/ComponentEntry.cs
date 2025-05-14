using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class ComponentEntry
{
    public string PluginKey;
    public JObject @params;
    
    [JsonConstructor]
    public ComponentEntry(string pluginKey, JObject @params)
    {
        this.PluginKey = pluginKey;
        this.@params = @params;
    }
}