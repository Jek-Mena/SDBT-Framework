using System;
using UnityEngine;

/// <summary>
/// [2025-06-13] Core context builder modules for AI blackboard injection.
/// - Groups all always-required context builder modules (timer, status, phase executor, etc.)
/// - Each builder is fail-fast, logs clearly, and fully documented for assessment and future audit.
/// </summary>
public class CoreContextModules
{
    // ----- Timer Injection -----
    /// <summary>
    /// Injects TimeExecutionManager into the blackboard (required for timed BT nodes).
    /// </summary>
    public class TimerContextBuilder : IContextBuilderModule
    {
        public void Build(GameObject entity, Blackboard blackboard)
        {
            var timer = entity.RequireComponent<TimeExecutionManager>();
            if (!timer)
                throw new Exception($"[TimerContextBuilder] {nameof(TimeExecutionManager)} missing on {entity.name}");
            blackboard.TimeExecutionManager = timer;
            Debug.Log($"[TimerContextBuilder] Injected {nameof(TimeExecutionManager)} for {entity.name}");
        }
    }
    
    // ----- Status Effect Injection -----
    /// <summary>
    /// Injects StatusEffectManager for handling debuffs, stuns, and other effects.
    /// </summary>
    public class StatusEffectContextBuilder : IContextBuilderModule
    {
        public void Build(GameObject entity, Blackboard blackboard)
        {
            var effect = entity.RequireComponent<StatusEffectManager>();
            if (!effect)
                throw new Exception($"[StatusEffectContextBuilder] {nameof(StatusEffectManager)} missing on {entity.name}");
            blackboard.StatusEffectManager = effect;
            Debug.Log($"[StatusEffectContextBuilder] Injected {nameof(StatusEffectManager)} for {entity.name}");
        }
    }
    
    // ----- Update Phase Executor Injection -----
    /// <summary>
    /// Injects UpdatePhaseExecutor for orchestrating AI actions across update phases.
    /// </summary>
    public class UpdatePhaseExecutorContextBuilder : IContextBuilderModule
    {
        public void Build(GameObject entity, Blackboard blackboard)
        {
            var exec = entity.GetComponent<UpdatePhaseExecutor>();
            if (!exec)
                throw new Exception($"[UpdatePhaseExecutorContextBuilder] {nameof(UpdatePhaseExecutor)} missing on {entity.name}");
            blackboard.UpdatePhaseExecutor = exec;
            Debug.Log($"[UpdatePhaseExecutorContextBuilder] Injected {nameof(UpdatePhaseExecutor)} for {entity.name}");
        }
    }
}