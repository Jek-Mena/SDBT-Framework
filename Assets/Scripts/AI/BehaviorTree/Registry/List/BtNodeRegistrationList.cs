using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement;
using AI.BehaviorTree.Nodes.Composites.Selector;
using AI.BehaviorTree.Nodes.Composites.Selector.Stimuli;
using AI.BehaviorTree.Nodes.TemporalControl;
using AI.BehaviorTree.Registry;
using UnityEngine;

/// <summary>
/// Provides a centralized registry for behavior tree node factories and their associated aliases.
/// This class facilitates the registration and retrieval of behavior tree node factories,
/// enabling dynamic creation of behavior tree nodes at runtime.
/// </summary>
public static class BtNodeRegistrationList
{
    private const string ScriptName = nameof(BtNodeRegistrationList);
    
    private static (string, IBtNodeFactory) MakeEntry<TFactory>(string alias)
        where TFactory : IBtNodeFactory, new()
    {
        return (alias, new TFactory());
    }

    // New helper for timed execution nodes
    private static (string, IBtNodeFactory) MakeTimedExecutionEntry<TNode>(string alias, bool hasChild, bool acceptsDomains)
        where TNode : IBehaviorNode
        => (alias, new TimedExecutionNodeFactory<TNode>(alias, hasChild, acceptsDomains));
    
    public static void Initialize()
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
            MakeTimedExecutionEntry<BtPauseNode>(BtNodeTypes.TimedExecution.Pause, hasChild: false, acceptsDomains: true),
            MakeTimedExecutionEntry<TimerNode>(BtNodeTypes.TimedExecution.Timer, hasChild: false, acceptsDomains: false),
            // --- End Timed Execution ---
            
            // --- Decorators ---
            MakeTimedExecutionEntry<TimeoutNode>(BtNodeTypes.Decorators.Timeout, hasChild: true, acceptsDomains: true),
            MakeEntry<BtRepeaterNodeFactory>(BtNodeTypes.Decorators.Repeater),
            // --- End Decorators ---
            
            // --- Composite ---
            MakeEntry<BtSequenceNodeFactory>(BtNodeTypes.Composite.Sequence),
            MakeEntry<BtParallelNodeFactory>(BtNodeTypes.Composite.Parallel),
            
            // ---- Selector(s)
            MakeEntry<BtSelectorNodeFactory>(BtNodeTypes.Composite.Selector),
            MakeEntry<BtStimuliSelectorNodeFactory>(BtNodeTypes.Composite.StimuliSelector),
            // --- End Composite ---
            
            // Add more here
        };

        foreach (var (alias, factory) in entries)
            BtNodeRegistry.Register(alias, factory);

        Debug.Log($"[{ScriptName}] Bootstrap complete. Total of {entries.Length} registered nodes");
    }
}