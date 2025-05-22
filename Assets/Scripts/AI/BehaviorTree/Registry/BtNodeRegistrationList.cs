using System;
using System.Collections.Generic;

/// <summary>
/// Provides a centralized registry for behavior tree node factories and their associated aliases.
/// This class facilitates the registration and retrieval of behavior tree node factories,
/// enabling dynamic creation of behavior tree nodes at runtime.
/// </summary>
public static class BtNodeRegistrationList
{
    /// <summary>
    /// Registers a behavior tree node factory with a specified alias.
    /// </summary>
    /// <typeparam name="TFactory">
    /// The type of the factory to register. Must implement <see cref="IBtNodeFactory"/> and have a parameterless constructor.
    /// </typeparam>
    /// <param name="alias">
    /// The alias to associate with the factory. This alias is used to identify the factory during retrieval.
    /// </param>
    /// <returns>
    /// A tuple containing the alias and an instance of the registered factory.
    /// </returns>
    private static (string alias, IBtNodeFactory factory) Register<TFactory>(string alias)
        where TFactory : IBtNodeFactory, new() => (alias, new TFactory());

    /// <summary>
    /// Retrieves all registered behavior tree node factories along with their associated aliases.
    /// </summary>
    /// <returns>
    /// <description><c>factory</c>: An instance of <see cref="IBtNodeFactory"/> representing the factory for the behavior tree node.</description>
    /// </returns>
    public static IEnumerable<(string alias, IBtNodeFactory factory)> GetAll()
    {
        return new[]
        {
            // Movement
            Register<MoveToTargetNodeFactory>(BtNodeAliases.Movement.MoveToTarget),
            Register<ImpulseMoverNodeFactory>(BtNodeAliases.Movement.ImpulseMover),
            
            // Timed Execution
            Register<BtPauseNodeFactory>(BtNodeAliases.TimedExecution.Pause),
            
            // Decorators
            Register<TimeoutDecoratorNodeFactory>(BtNodeAliases.Decorators.Timeout),
            Register<BtRepeaterNodeFactory>(BtNodeAliases.Decorators.Repeater),
            
            // Composite
            Register<BtSequenceNodeFactory>(BtNodeAliases.Composite.Sequence),
            Register<BtParallelNodeFactory>(BtNodeAliases.Composite.Parallel),
            Register<BtSelectorNodeFactory>(BtNodeAliases.Composite.Selector)
            
            // Add more here
        };
    }

    // Maps aliases to their corresponding factory types for quick lookup.
    public static readonly Dictionary<string, Type> AliasToKeyMap = new()
    {
        // Movement
        { BtNodeAliases.Movement.MoveToTarget, typeof(MoveToTargetNodeFactory) },
        { BtNodeAliases.Movement.ImpulseMover, typeof(ImpulseMoverNodeFactory) },
        
        // Timed Execution
        { BtNodeAliases.TimedExecution.Pause, typeof(BtPauseNodeFactory) },
        
        // Decorators
        { BtNodeAliases.Decorators.Timeout, typeof(TimeoutDecoratorNodeFactory) },
        { BtNodeAliases.Decorators.Repeater, typeof(BtRepeaterNodeFactory) },
        
        // Composite
        { BtNodeAliases.Composite.Sequence, typeof(BtSequenceNodeFactory) },
        { BtNodeAliases.Composite.Parallel, typeof(BtParallelNodeFactory) },
        { BtNodeAliases.Composite.Selector, typeof(BtSelectorNodeFactory) },
    };

}