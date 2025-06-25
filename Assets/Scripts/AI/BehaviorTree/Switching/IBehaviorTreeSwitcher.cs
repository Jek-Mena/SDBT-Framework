using System;

/// <summary>
/// [2025-06-24]
/// Represents an interface for evaluating and managing behavior tree switches in an AI system.
/// </summary>
public interface IBehaviorTreeSwitcher
{
    /// <summary>
    /// Called every tick or whenever relevant context/stimulus changes.
    /// Evaluates context and decides if a switch is needed.
    /// </summary>
    string EvaluateSwitch(BtContext context, string currentTreeKey);
    
    /// <summary>
    /// Event for observers to subscribe to switch requests.
    /// Args: fromKey, toKey, reason/context string.
    /// </summary>
    event Action<string, string, string> OnSwitchRequested; // fromKey, toKey, reason
    
    /// <summary>
    /// Optionally, let the switcher reset/re-initialize.
    /// </summary>
    void Reset();
}