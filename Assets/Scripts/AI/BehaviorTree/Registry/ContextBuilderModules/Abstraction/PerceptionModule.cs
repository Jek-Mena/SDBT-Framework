using System.Collections.Generic;
using System.Diagnostics;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

/// <summary>
/// [2025-07-04]
/// Abstract base for all perception modules. TStimulus: type of stimulus (e.g., FearStimulus).
/// TProfile: domain-specific config/profile (e.g., FearPerceptionData).
/// </summary>
public abstract class PerceptionModule<TStimulus, TProfile>: MonoBehaviour, IPerceptionModule
{
    [Header("Perception Config (Injected from JSON/Profile)")]
    public TProfile Profile; // Set via context pipeline or inspector (for now)
    
    protected BtContext Context;
    
    /// <summary>
    /// Called once at <see cref="PerceptionBuilderModule"/> to inject BT context into the perception module.
    /// </summary>
    public virtual void Initialize(BtContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Called every by Update.
    /// </summary>
    public void UpdatePerception()
    {
        if(Context == null) return;
        
        var stimuli = QueryStimuli();
        WriteStimuliToBlackboard(stimuli);
        ProcessStimuli(stimuli);
    }

    /// <summary>
    /// Implement: Any additional processing (scoring, curve application, main threat, etc.).
    /// </summary>
    protected abstract void ProcessStimuli(List<TStimulus> stimuli);

    /// <summary>
    /// Implement: Write all stimuli data (raw or processed) to blackboard.
    /// </summary>
    protected abstract void WriteStimuliToBlackboard(List<TStimulus> stimuli);

    /// <summary>
    /// Implement: Query stimuli from the relevant manager/system for this domain.
    /// </summary>
    protected abstract List<TStimulus> QueryStimuli();
}