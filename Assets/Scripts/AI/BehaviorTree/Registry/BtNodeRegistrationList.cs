/// <summary>
/// Provides a centralized registry for behavior tree node factories and their associated aliases.
/// This class facilitates the registration and retrieval of behavior tree node factories,
/// enabling dynamic creation of behavior tree nodes at runtime.
/// </summary>
public static class BtNodeRegistrationList
{
    private static (string, IBtNodeFactory) MakeEntry<TFactory>(string alias)
        where TFactory : IBtNodeFactory, new()
    {
        return (alias, new TFactory());
    }

    public static void InitializeDefaults()
    {
        var entries = new (string alias, IBtNodeFactory factory)[]
        {
            // Actions / Leaves
            MakeEntry<MoveToTargetNodeFactory>(BtNodeAliases.Movement.MoveToTarget),
            MakeEntry<ImpulseMoverNodeFactory>(BtNodeAliases.Movement.ImpulseMover),
            
            // Timed Execution
            MakeEntry<BtPauseNodeFactory>(BtNodeAliases.TimedExecution.Pause),
            
            // Decorators
            MakeEntry<BtTimeoutDecoratorNodeFactory>(BtNodeAliases.Decorators.Timeout),
            MakeEntry<BtRepeaterNodeFactory>(BtNodeAliases.Decorators.Repeater),
            
            // Composite
            MakeEntry<BtSequenceNodeFactory>(BtNodeAliases.Composite.Sequence),
            MakeEntry<BtParallelNodeFactory>(BtNodeAliases.Composite.Parallel),
            MakeEntry<BtSelectorNodeFactory>(BtNodeAliases.Composite.Selector)
            
            // Add more here
        };

        foreach (var (alias, factory) in entries)
            BtNodeRegistry.Register(alias, factory);
    }
}