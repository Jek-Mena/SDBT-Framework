using System;
using System.Collections.Generic;

//(Singleton-style, but you can swap to DI if needed. Place in a core or systems folder.)
namespace AI.BehaviorTree.Core
{
    public static class AgentRegistry
    {
        private static readonly Dictionary<string, AgentDefinition> Agents = new();
    
        public static void Register(AgentDefinition def)
        {
            if (string.IsNullOrWhiteSpace(def.EntityId))
                throw new Exception("[EntityRegistry] Cannot register entity with null/empty ID.");

            Agents[def.EntityId] = def;
        }

        //(Singleton-style, but you can swap to DI if needed. Place in a core or systems folder.)
        public static AgentDefinition Get(string entityId)
        {
            if (Agents.TryGetValue(entityId, out var def))
                return def;
            throw new Exception($"[EntityRegistry] No entity registered for ID '{entityId}'");
        }

        public static IEnumerable<string> AllEntityIds => Agents.Keys;

    }
}