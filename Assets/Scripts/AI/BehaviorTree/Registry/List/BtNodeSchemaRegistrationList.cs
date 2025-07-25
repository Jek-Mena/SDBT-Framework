// BtNodeSchemaRegistrationList.cs
// Purpose: RegisterSchema all node schemas into the validator registry
// Triggered via RuntimeInitializeOnLoadMethod or Editor static context

using AI.BehaviorTree.Keys;

namespace AI.BehaviorTree.Registry.List
{
    public static class BtNodeSchemaRegistrationList
    {
        private static bool _hasInitialized;
        public static void InitializeDefaults()
        {
            if(_hasInitialized) return;
            _hasInitialized = true;
        
            var entries = new (string alias, IBtNodeSchema schema)[]
            {
                // Actions / Leaves
                MakeEntry<MoveToTargetSchema>(BtNodeTypes.Movement.MoveToTarget),
            
                // Decorators
                MakeEntry<RepeaterSchema>(BtNodeTypes.Decorators.Repeater),
            
                // Composites
                MakeEntry<SequenceSchema>(BtNodeTypes.Composite.Sequence),
                MakeEntry<ParallelSchema>(BtNodeTypes.Composite.Parallel),
                MakeEntry<SelectorSchema>(BtNodeTypes.Composite.Selector)
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
}