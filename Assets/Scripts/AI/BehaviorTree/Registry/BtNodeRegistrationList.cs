using System;
using System.Collections.Generic;

/// <summary>
/// Explicit registration of Behavior Tree node keys and their corresponding factories.
/// Provides type-safe mappings from alias strings to node key types and factories used during BT deserialization.
///
/// This avoids reflection-based discovery (e.g., [BtNode("...")]) in favor of fail-fast, compile-time-safe registration.
/// While this requires manual updates, it ensures high clarity, IDE navigation, and safer renaming/refactoring.
///
/// TODO:
/// - Add Reg<TFactory, TKey>() helper to reduce repetitive (typeof, new) boilerplate in GetAll().
///   Example: Reg<MoveNodeFactory, MoveToNodeKey>() instead of (typeof(MoveToNodeKey), new MoveNodeFactory()).
///
/// - Group registrations by domain to improve readability and maintainability:
///     - GetActionNodes()
///     - GetDecoratorNodes()
///     - GetTimedExecutionNodes()
///     - GetCompositeNodes()
///   Combine them in GetAll() via .Concat(). This allows easier batching, testing, and modular development.
///
/// - Consider adding Alias<TKey>() helper to reduce alias map verbosity:
///     - Instead of: { MoveToNodeKey.Alias, typeof(MoveToNodeKey) }
///     - Use: Alias<MoveToNodeKey>(MoveToNodeKey.Alias)
///
/// - Optionally automate AliasToKeyMap construction from a shared key registry later, if node count grows significantly.
///
/// - Maintain alphabetical or domain-based grouping for both GetAll() and AliasToKeyMap entries.
/// </summary>
public static class BtNodeRegistrationList
{
    /// <summary>
    /// From alias string -> key type -> factory.
    /// List of all node key/factory registrations.
    /// Used by BtNodeRegistry to build the factory map.
    /// </summary>
    public static IEnumerable<(Type keyType, IBtNodeFactory factory)> GetAll()
    {
        return new (Type, IBtNodeFactory)[]
        {
            // Actions
            (typeof(MoveToTargetNodeKey), new MoveToTargetNodeFactory()),
            (typeof(ImpulseMoverNodeKey), new ImpulseMoverNodeFactory()),
            
            // Timed Execution
            (typeof(PauseNodeKey), new PauseNodeFactory()),
            
            // Decorators
            (typeof(TimeoutDecoratorNodeKey), new TimeoutDecoratorNodeFactory()),
            (typeof(RepeaterNodeKey), new BtRepeaterNodeFactory()),

            // Composite
            (typeof(SequenceNodeKey), new BtSequenceNodeFactory()),
            (typeof(ParallelNodeKey), new BtParallelNodeFactory()),
            (typeof(SelectorNodeKey), new BtSelectorNodeFactory()),

            // Add more here
        };
    }

    /// <summary>
    /// Maps string aliases (used in JSON) to node key types.
    /// Used for alias-based factory lookup at runtime.
    /// </summary>
    public static Dictionary<string, Type> AliasToKeyMap = new()
    {
        // Actions
        { MoveToTargetNodeKey.Alias, typeof(MoveToTargetNodeKey) },
        { ImpulseMoverNodeKey.Alias, typeof(ImpulseMoverNodeKey) },

        // Timed Execution
        { PauseNodeKey.Alias, typeof(PauseNodeKey) },

        // Decorators
        { TimeoutDecoratorNodeKey.Alias, typeof(TimeoutDecoratorNodeKey) },
        { RepeaterNodeKey.Alias, typeof(RepeaterNodeKey) },

        // Composite
        { SequenceNodeKey.Alias, typeof(SequenceNodeKey) },
        { ParallelNodeKey.Alias, typeof(ParallelNodeKey) },
        { SelectorNodeKey.Alias, typeof(SelectorNodeKey) },

        // Add more here
    };
}