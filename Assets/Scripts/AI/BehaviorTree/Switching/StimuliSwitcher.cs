using System;
using UnityEngine;

public class StimuliSwitcher : MonoBehaviour, IBehaviorTreeSwitcher
{
    [Header("Tree Keys")]
    [SerializeField] private string chaseKey = "Chase";
    [SerializeField] private string fleeKey = "Flee";
    
    [Header("Stimulus Source")]
    [SerializeField] private string stimulusKey = "HP";
    [SerializeField] private float fleeThreshold = 20f;
    
    public event Action<string, string, string> OnSwitchRequested;
    private string _lastKey;
    
    /// <summary>
    /// Checks the stimulus from the blackboard and switches tree if needed.
    /// Fires OnSwitchRequested and logs all transitions.
    /// </summary>
    public string EvaluateSwitch(BtContext context)
    {
        var value = context.Blackboard.Get<float>(stimulusKey);
        var newKey = value < fleeThreshold ? fleeKey : chaseKey;

        if (newKey != _lastKey)
        {
            OnSwitchRequested?.Invoke(_lastKey, newKey, $"{stimulusKey}={value}");
            Debug.Log($"[SimpleStimuliSwitcher] Switch: {_lastKey ?? "(none)"} -> {newKey} ({stimulusKey}={value})");
            _lastKey = newKey;
            return newKey;
        }
        return null;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}