using System;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Switching
{
    /// <summary>
    /// [2025-06-24]
    /// Represents an interface for evaluating and managing behavior tree switches in an AI system.
    /// </summary>
    public interface IBtPersonaSwitcher
    {
        /// <summary>
        /// Event for observers to subscribe to switch requests.
        /// Args: fromKey, toKey, reason/context string.
        /// </summary>
        event Action<string, string, string> OnSwitchRequested; // fromKey, toKey, reason
    
        /// <summary>
        /// Called every tick or whenever relevant context/stimulus changes.
        /// Evaluates context and decides if a switch is needed.
        /// </summary>
        string OldEvaluateSwitch(BtContext context, string currentTreeKey);
    
        string EvaluateSwitch(BtContext context, string currentTreeKey);
        
        /// <summary>
        /// Optionally, let the switcher reset/re-initialize.
        /// </summary>
        void Reset();
    }
}