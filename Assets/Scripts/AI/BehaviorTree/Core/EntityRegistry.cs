using System;
using System.Collections.Generic;

//(Singleton-style, but you can swap to DI if needed. Place in a core or systems folder.)
public static class EntityRegistry
{
    private static readonly Dictionary<string, EntityDefinition> _entities = new();
    
    public static void Register(EntityDefinition def)
    {
        if (string.IsNullOrWhiteSpace(def.EntityId))
            throw new Exception("[EntityRegistry] Cannot register entity with null/empty ID.");

        _entities[def.EntityId] = def;
    }

    //(Singleton-style, but you can swap to DI if needed. Place in a core or systems folder.)
    public static EntityDefinition Get(string entityId)
    {
        if (_entities.TryGetValue(entityId, out var def))
            return def;
        throw new Exception($"[EntityRegistry] No entity registered for ID '{entityId}'");
    }

    public static IEnumerable<string> AllEntityIds => _entities.Keys;

}