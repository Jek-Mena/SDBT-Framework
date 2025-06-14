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
            // --- Actions / Leaves ---
            // Movement
            MakeEntry<MoveToTargetNodeFactory>(BtNodeTypes.Movement.MoveToTarget),
            MakeEntry<ImpulseMoverNodeFactory>(BtNodeTypes.Movement.ImpulseMover),
            
            // Rotation
            MakeEntry<RotateToTargetNodeFactory>(BtNodeTypes.Rotation.RotateToTarget),
            // --- End Actions / Leaves ---
            
            
            // --- Timed Execution
            MakeEntry<BtPauseNodeFactory>(BtNodeTypes.TimedExecution.Pause),
            // --- End Timed Execution ---
            
            
            // --- Decorators ---
            MakeEntry<TimeoutNodeFactory>(BtNodeTypes.Decorators.Timeout),
            MakeEntry<BtRepeaterNodeFactory>(BtNodeTypes.Decorators.Repeater),
            // --- End Decorators ---
            
            
            // --- Composite ---
            MakeEntry<BtSequenceNodeFactory>(BtNodeTypes.Composite.Sequence),
            MakeEntry<BtParallelNodeFactory>(BtNodeTypes.Composite.Parallel),
            MakeEntry<BtSelectorNodeFactory>(BtNodeTypes.Composite.Selector)
            // --- End Composite ---
            
            // Add more here
        };

        foreach (var (alias, factory) in entries)
            BtNodeRegistry.Register(alias, factory);
    }
}