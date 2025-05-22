using System;
using System.Collections.Generic;

/// <summary>
/// Provides a registry for managing and accessing behavior tree node factories.
/// This class allows for the registration, retrieval, and initialization of factories
/// associated with behavior tree nodes, enabling dynamic creation of nodes at runtime.
/// </summary>
public static class BtNodeRegistry
{
    private static readonly Dictionary<string, IBtNodeFactory> Map = new();

    /// <summary>
    /// Returns all registered node aliases.
    /// </summary>
    public static IEnumerable<string> RegisteredAliases => Map.Keys;

    /// <summary>
    /// Retrieves the factory associated with the specified alias.
    /// </summary>
    public static IBtNodeFactory GetFactoryByAlias(string alias)
    {
        if (!Map.TryGetValue(alias, out var factory))
            throw new InvalidOperationException($"Factory not registered for alias: '{alias}'");

        return factory;
    }

    /// <summary>
    /// Initializes the factory registry with all aliases and corresponding factories.
    /// </summary>
    public static void InitializeDefaults()
    {
        foreach (var (keyType, factory) in BtNodeRegistrationList.GetAll())
        {
            if(Map.ContainsKey(keyType))
                throw new InvalidOperationException($"Factory already registered. Duplicate factory key: {keyType}");

            Map[keyType] = factory;
        }
    }

    /// <summary>
    /// Logs all registered aliases and their corresponding factory types.
    /// </summary>
    public static void PrintRegisteredNodes()
    {
        foreach (var (alias, factory) in Map)
        {
            UnityEngine.Debug.Log($"Alias '{alias}' → Factory '{factory.GetType().Name}'");
        }
    }
}