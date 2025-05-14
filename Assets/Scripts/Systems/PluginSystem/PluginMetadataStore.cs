using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Central registry for all plugin metadata used during Behavior Tree and entity initialization.
/// This replaces legacy [Plugin(...)] attributes and the PluginKey enum system.
/// </summary>
public static class PluginMetadataStore
{
    private static readonly List<PluginMetadata> Registrations = new();
    private static readonly object LockObj = new();
    private static bool _frozen = false;

    /// <summary>
    /// All registered plugin metadata entries, used for initialization and plugin ordering.
    /// Returns a safe, immutable snapshot.
    /// </summary>
    public static IReadOnlyList<PluginMetadata> RegisteredPlugins
    {
        get { lock (LockObj) return Registrations.ToList(); }
    }

    /// <summary>
    /// Registers a plugin and its metadata manually.
    /// </summary>
    public static void Register<TPlugin>(
        string pluginKey,
        string schemaKey,
        PluginExecutionPhase phase,
        Type[] dependsOn
    ) where TPlugin : IEntityComponentPlugin
    {
        lock (LockObj)
        {
            if (_frozen)
                throw new InvalidOperationException("Cannot register after finalization.");

            if (Registrations.Any(x => x.PluginKey == pluginKey))
                throw new InvalidOperationException($"Plugin '{pluginKey}' is already registered.");

            Registrations.Add(new PluginMetadata
            {
                PluginKey = pluginKey,
                PluginType = typeof(TPlugin),
                SchemaKey = schemaKey,
                ExecutionPhase = phase,
                DependsOn = dependsOn ?? Array.Empty<Type>()
            });
        }
    }

    /// <summary>
    /// Shortcut for registering a plugin without dependencies.
    /// </summary>
    public static void Register<TPlugin>(
        string pluginKey,
        string schemaKey,
        PluginExecutionPhase phase
    ) where TPlugin : IEntityComponentPlugin
    {
        Register<TPlugin>(pluginKey, schemaKey, phase, null);
    }

    /// <summary>
    /// Prevents further registration. Intended to be called after setup.
    /// </summary>
    public static void FinalizeRegistrations()
    {
        lock (LockObj)
        {
            _frozen = true;
        }
    }

    /// <summary>
    /// (Optional for tests) Clears all registrations. Should not be used in production.
    /// </summary>
    internal static void ClearForTests()
    {
        lock (LockObj)
        {
            _frozen = false;
            Registrations.Clear();
        }
    }
}