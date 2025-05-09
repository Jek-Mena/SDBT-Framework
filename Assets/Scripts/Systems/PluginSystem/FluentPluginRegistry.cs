using System;
using System.Collections.Generic;
using System.Reflection;

public static class FluentPluginRegistry
{
    private static readonly List<PluginMetadata> _all = new();
    public static IReadOnlyList<PluginMetadata> All => _all;

    public static void Register<TPlugin>(
        PluginPhase phase,
        params Type[] dependsOn
    ) where TPlugin : IEntityComponentPlugin
    {
        // Grab the [Plugin(PluginKey.XXX)] attribute
        var key = typeof(TPlugin)
            .GetCustomAttribute<PluginAttribute>()?
            .Key ?? throw new Exception($"Missing [Plugin] on {typeof(TPlugin).Name}");

        _all.Add(new PluginMetadata
        {
            Key = key,
            PluginType = typeof(TPlugin),
            Phase = phase,
            DependsOn = dependsOn ?? Array.Empty<Type>()
        });
    }
}