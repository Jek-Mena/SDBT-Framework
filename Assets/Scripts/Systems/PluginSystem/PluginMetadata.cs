using System;

/// <summary>
/// Defines metadata used to initialize and apply plugins in correct order.
/// </summary>
public record PluginMetadata
{
    public string PluginKey { get; set; }           // Plugin identifier
    public string SchemaKey { get; set; }     // JSON schema validation
    public Type PluginType { get; set; }      // Actual C# type
    public string Domain { get; set; }        // E.g., "Movement", "Visual"
    public PluginExecutionPhase ExecutionPhase { get; set; }  // For sorting/initialization
    public Type[] DependsOn { get; set; }
}
