using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class PluginRegistry
{
    private static Dictionary<string, IEntityComponentPlugin> _plugins = new();

    public static void RegisterAll()
    {
        var pluginTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(IEntityComponentPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in pluginTypes)
        {
            var attr = type.GetCustomAttribute<PluginAttribute>();
            if (attr == null) continue;

            var instance = (IEntityComponentPlugin)Activator.CreateInstance(type);
            _plugins[attr.PluginKey] = instance;
        }
    }

    public static bool TryGet(string id, out IEntityComponentPlugin plugin)
    {
        return _plugins.TryGetValue(id, out plugin);
    }
}