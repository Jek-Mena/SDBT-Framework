using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Switching;
using UnityEngine;

public class StimuliSwitcher : MonoBehaviour, IBehaviorTreeSwitcher
{
    public event Action<string, string, string> OnSwitchRequested;
    private string _lastKey;

    private const string ScriptName = nameof(StimuliSwitcher);
    
    /// <summary>
    /// Checks the stimulus from the blackboard and switches a tree if needed.
    /// Fires OnSwitchRequested and logs all transitions.
    /// </summary>
    public string EvaluateSwitch(BtContext context, string currentTreeKey)
    {
        if (context.Blackboard.SwitchProfiles == null ||
            !context.Blackboard.SwitchProfiles.TryGetValue(AgentProfileSelectorKeys.Switch.DefaultProfile, out var conditions)
            || conditions == null 
            || conditions.Count == 0)
        {
            Debug.LogError($"[{ScriptName}] No switch profile '{AgentProfileSelectorKeys.Switch.DefaultProfile}' found on agent {context.Agent.name}!");
            return null;
        }
        
        foreach (var cond in conditions)
        {
            var value = context.Blackboard.Get<float>(cond.stimulusKey);
            var pass = cond.comparisonOperator switch
            {
                "LessThan" => value < cond.threshold,
                "LessThanOrEqual" => value <= cond.threshold,
                "GreaterThan" => value > cond.threshold,
                "GreaterThanOrEqual" => value >= cond.threshold,
                "Equal" => Math.Abs(value - cond.threshold) < 0.001f,
                _ => false
            };

            if (pass)
            {
                if (cond.behaviorTree != currentTreeKey)
                {
                    OnSwitchRequested?.Invoke(_lastKey, cond.behaviorTree, $"{cond.stimulusKey}={value}");
                    Debug.Log($"[{ScriptName}] Switch: {_lastKey ?? "(none)"} -> {cond.behaviorTree} ({cond.stimulusKey}={value})");
                    return cond.behaviorTree;
                }
                // If already on correct tree, just exit quietly
                return null;
            }
        }
        Debug.LogError($"[{ScriptName}] No switch condition passed! Stimuli state: {DumpStimuli(context, conditions)} Conditions: {DumpConditions(conditions)}");
        return null;
    }
    
    private string DumpStimuli(BtContext context, List<SwitchCondition> conditions)
    {
        var s = "";
        foreach (var cond in conditions)
            s += $"{cond.stimulusKey}={context.Blackboard.Get<float>(cond.stimulusKey)}, ";
        return s;
    }

    private string DumpConditions(List<SwitchCondition> conditions)
    {
        return string.Join("; ", conditions.Select(c => $"{c.stimulusKey} {c.comparisonOperator} {c.threshold} -> {c.behaviorTree}"));
    }

    public void Reset() => _lastKey = null;
}
