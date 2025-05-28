// BtNodeSchemaRegistrationList.cs
// Purpose: RegisterSchema all node schemas into the validator registry
// Triggered via RuntimeInitializeOnLoadMethod or Editor static context

using UnityEngine;

public static class BtNodeSchemaRegistrationList
{
    
    public static void InitializeDefaults()
    {
        var entries = new (string alias, IBtNodeSchema schema)[]
        {
            // Actions / Leaves
            MakeEntry<MoveToTargetNodeSchema>(BtNodeAliases.Movement.MoveToTarget),
        };

        foreach (var (alias, schema) in entries)
            BtNodeSchemaRegistry.Register(alias, schema);
    }
    private static (string, IBtNodeSchema) MakeEntry<TSchema>(string alias)
        where TSchema : IBtNodeSchema, new()
    {
        return (alias, new TSchema());
    }
}