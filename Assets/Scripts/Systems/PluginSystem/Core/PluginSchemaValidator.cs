using System.Collections.Generic;
using UnityEngine;

public static class PluginSchemaValidator
{
    public static void Validate(IEnumerable<ComponentEntry> components)
    {
        foreach (var entry in components)
        {
            if (!PluginRegistry.TryGet(entry.type, out var plugin))
            {
                Debug.Log($"[Validator] Plugin not registered: {entry.type}");
                continue;
            }

            if (plugin is IValidatablePlugin validator)
            {
                validator.Validate(entry);
            }
        }
    }
}