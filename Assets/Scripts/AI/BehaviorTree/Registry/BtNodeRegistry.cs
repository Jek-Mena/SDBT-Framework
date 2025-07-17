using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;

namespace AI.BehaviorTree.Registry
{
    /// <summary>
    /// Provides a registry for managing and accessing behavior tree node factories.
    /// This class allows for the registration, retrieval, and initialization of factories
    /// associated with behavior tree nodes, enabling dynamic creation of nodes at runtime.
    /// </summary>
    public static class BtNodeRegistry
    {
        private static readonly Dictionary<string, IBtNodeFactory> Map = new();
    
        public static void Register(string alias, IBtNodeFactory factory)
        {
            if (Map.ContainsKey(alias))
                throw new System.InvalidOperationException($"Duplicate factory key: '{alias}'");

            Map[alias] = factory;
        }

        public static bool TryGet(string alias, out IBtNodeFactory factory)
        {
            return Map.TryGetValue(alias, out factory);
        }

        public static IBtNodeFactory GetFactoryByAlias(string alias)
        {
            if (!TryGet(alias, out var factory))
                throw new System.Exception($"[BtNodeRegistry] No factory found for alias: {alias}");

            return factory;
        }

        public static bool IsKnown(string alias)
        {
            return Map.ContainsKey(alias);
        }
    }
}