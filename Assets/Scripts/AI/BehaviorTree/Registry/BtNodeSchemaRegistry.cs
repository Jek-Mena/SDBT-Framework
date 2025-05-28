using System;
using System.Collections.Generic;

public static class BtNodeSchemaRegistry
{
    private static readonly Dictionary<string, IBtNodeSchema> Schemas = new();
    public static void Register(string alias, IBtNodeSchema schema)
    {
        if (Schemas.ContainsKey(alias))
            throw new InvalidOperationException($"Duplicate schema key: '{alias}' from {schema.GetType().Name}");

        Schemas[alias] = schema;
    }

    public static bool IsKnown(string alias)
    {
        return Schemas.ContainsKey(alias);
    }


    public static bool TryGet(string alias, out IBtNodeSchema schema)
    {
        return Schemas.TryGetValue(alias, out schema);
    }

    public static IEnumerable<string> AllRegisteredAliases => Schemas.Keys;

}