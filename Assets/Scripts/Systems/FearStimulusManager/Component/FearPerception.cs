using UnityEngine;

/// <summary>
/// [2025-06-27]
/// Called one per tick in AI/BT update
/// </summary>
public class FearPerception : MonoBehaviour
{
    // Maximum distance at which this agent can perceive fear stimuli—stimuli beyond this range are ignored.
    public FearPerceptionData PerceptionData; // <<--- This is where I left off... need to initialize
    // need to initialize the data and the contextmodulebuilder
    
    private BtContext _context;
    
    public void Initialize(BtContext context) => _context = context;

    public void UpdatePerception()
    {
        // Quary all nearby fear stimuli
        var position = transform.position;
        var stimuli = FearStimulusManager.Query(position, DetectionRange);
        
        // Write raw stimuli to blackboard
        _context.Blackboard.Set(BlackboardKeys.Fear.StimuliNearby, stimuli);
        
        // Evaluate total fear level and main threat
        var totalFear = 0f;
        var maxContrition = 0f;
        FearStimulus? mainThreat = null;
        
        foreach (var stim in stimuli)
        {
            var dist = Vector3.Distance(position, stim.Position);

            if (dist < stim.Radius)
            {
                // Simple weighted: linear falloff
                float contribution = stim.Strength * (1f - dist / stim.Radius);
                totalFear += contribution;
                if (contribution > maxContrition)
                {
                    maxContrition = contribution;
                    mainThreat = stim;
                }
            }
        }
        
        _context.Blackboard.Set(BlackboardKeys.Fear.Level, totalFear);
        if (mainThreat.HasValue)
            _context.Blackboard.Set(BlackboardKeys.Fear.Source, mainThreat.Value);            
        else
            _context.Blackboard.Remove(BlackboardKeys.Fear.Source);
    }
}