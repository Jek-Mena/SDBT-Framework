using System;

public class PluginMetadata
{
    public PluginKey Key { get; set; }
    public Type PluginType { get; set; }
    public PluginPhase Phase { get; set; }
    public Type[] DependsOn { get; set; }
}