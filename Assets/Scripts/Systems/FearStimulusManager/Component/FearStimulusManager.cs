using System.Collections.Generic;
using UnityEngine;

public class FearStimulusManager : MonoBehaviour
{
    private static FearStimulusManager _instance;
    public static FearStimulusManager Instance
    {
        get
        {
            if (_instance) return _instance;
            
            var obj = new GameObject("FearStimulusManager");
            _instance = obj.AddComponent<FearStimulusManager>();
            return _instance;
        }
    }
    
    private readonly List<FearStimulus> _stimuli = new List<FearStimulus>();

    public static List<FearStimulus> Query(Vector3 position, float range)
    {
        var found = new List<FearStimulus>();
        foreach (var stim in Instance._stimuli)
        {
            if((stim.Position - position).sqrMagnitude < (range * range))
                found.Add(stim);
        }
        
        return found;
    }
    
    public static void Register(FearStimulus stimulus) => Instance._stimuli.Add(stimulus);
    public static void Unregister(FearStimulus stimulus) => Instance._stimuli.Remove(stimulus);
    public static void ClearAll() => Instance._stimuli.Clear();

    //Clean up completed timers (helpful for long sessions)
    private void LateUpdate()
    {
        if (Instance._stimuli.Count == 0) return;
        var expiredStimulus = new List<FearStimulus>();
        foreach (var stim in Instance._stimuli)
        {
            if (stim.Duration > 0 && stim.Duration <= 0)
                expiredStimulus.Add(stim);
        }
        
        foreach (var stim in expiredStimulus)
            Instance._stimuli.Remove(stim);
    }
}