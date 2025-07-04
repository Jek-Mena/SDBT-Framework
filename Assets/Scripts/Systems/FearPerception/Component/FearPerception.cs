using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Always-sensing fear perception. Queries the manager, processes fear, and writes to the blackboard.
/// </summary>
public class FearPerception : PerceptionModule<FearStimulus, FearPerceptionData>
{
    private const string ScriptName = nameof(FearPerception);
    
    public override void Initialize(BtContext context)
    {
        base.Initialize(context);
        
        Profile = context.Blackboard.GetFearPerceptionProfile(AgentProfileFields.CurrentFearProfile);
        if (Profile == null)
            Debug.LogError($"[{ScriptName}] No FearPerceptionData profile found for this agent/context!");
    }

    protected override void ProcessStimuli(List<FearStimulus> stimuli)
    {
        var position = transform.position;
        var totalFear = 0f;
        var maxContribution = 0f;
        FearStimulus? mainThreat = null;

        foreach (var stim in stimuli)
        {
            var distance = Vector3.Distance(position, stim.Position);
            if (distance < stim.Radius)
            {
                // Simple weighted: linear falloff
                var contribution = stim.Strength * (1f - distance / stim.Radius);
                totalFear += contribution;
                if (contribution > maxContribution)
                {
                    maxContribution = contribution;
                    mainThreat = stim;
                }
            }
        }

        Context.Blackboard.Set(BlackboardKeys.Fear.Level, totalFear);

        if (mainThreat.HasValue)
            Context.Blackboard.Set(BlackboardKeys.Fear.Source, mainThreat.Value);
        else
            Context.Blackboard.Remove(BlackboardKeys.Fear.Source);
    }
    
    protected override List<FearStimulus> QueryStimuli()
    {
        if (Profile == null)
        {
            Debug.LogError($"[{ScriptName}] Profile missing; cannot query stimuli.");
            return new List<FearStimulus>();
        }

        return FearStimulusManager.Instance?.Query(transform.position, Profile.DetectionRange) 
               ?? new List<FearStimulus>();
    }
    
    protected override void WriteStimuliToBlackboard(List<FearStimulus> stimuli)
    {
        Context.Blackboard.Set(BlackboardKeys.Fear.StimuliNearby, stimuli);
    }
    
    private void Update()
    {
         // (Optional) Next: PerceptionPipelineManager
         // If you want to support multiple perception modules (sound, vision, etc.) with clean ticking, make a PerceptionPipelineManager that ticks all attached PerceptionModule<>s each frame.
         // But for now, a per-module Update is acceptable.
        UpdatePerception();
    }
}