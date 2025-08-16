using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Stimulus;
using Systems.Abstractions;
using UnityEngine;

namespace AI.BehaviorTree.Switching
{
    public class PersonaBtSwitcher : ISystemCleanable
    {
        private const string ScriptName = nameof(PersonaBtSwitcher);
        public event Action<string, string, string> OnSwitchRequested;

        private List<PersonaSwitchRule> _rules;
        
        private string _defaultTreeKey;
        private string _lastTreeKey;
        private float _lastStimulusValue;
        private float _lastSwitchTime; // To force the initial switch
        private float _switchCooldown = 3.0f; // Some of these might need to be exposed in the editor for AI fine tuning. 
        private float _hysteresisThreshold = 0.5f; // See Hysteresis and Pitfalls.pdf
        
        // Load rules from context / agentProfiles on startup
        public PersonaBtSwitcher(BtContext context)
        {
            var personaProfileKey = context.AgentProfiles.CurrentPersonaProfile;
            Debug.Log($"[{ScriptName}] Initializing. Using persona profile key: '{personaProfileKey}'");

            if (string.IsNullOrEmpty(personaProfileKey))
                Debug.LogError($"[{ScriptName}] Persona profile key is null/empty!");

            _rules = context.AgentProfiles.GetPersonaProfile(personaProfileKey);
            if (_rules == null || _rules.Count == 0)
            {
                Debug.LogError($"[{ScriptName}] No persona rules found for key '{personaProfileKey}'!");
                throw new Exception($"[{ScriptName}] No persona rules found for key '{personaProfileKey}'!");
            }
            
            // Set default as the rule with "Default" situationKey (optional: make configurable)
            _defaultTreeKey = _rules.FirstOrDefault(r => r.SituationKey == BtAgentJsonFields.AgentProfiles.PersonaProfile.DefaultSituation)?.MainTreeKey;

            //Debug.Log($"[{ScriptName}]📤🌲Loaded {_rules.Count} rules. Default tree: {_defaultTreeKey}");
            //Debug.Log($"[{ScriptName}]📤🌲Default tree: '{_defaultTreeKey}', All rules: {string.Join(", ", _rules.Select(r => $"{r.SituationKey}->{r.MainTreeKey}"))}");
            
            foreach (var rule in _rules)
                Debug.Log($"[{ScriptName}][PersonaRule] situationKey={rule.SituationKey} mainTreeKey={rule.MainTreeKey}");
            
            //Debug.Log($"_activeEffects after BT switch: " 
            //          + string.Join(", ", context.Blackboard.StatusEffectManager.GetActiveEffects().Select(e => e.Name + "->" + string.Join("/", e.Domains))) );
            
            // To force the initial switch
            _lastStimulusValue = -999f;
            _lastSwitchTime = -999f; 
            _lastTreeKey = null;
        }

        public string EvaluateSwitch(BtContext context, string currentTreeKey)
        {
            const string stimulusKey = "FearStimulusLevel"; // AKA stimulusId
            var stimulusValue = context.Blackboard.Get<float>(stimulusKey);
            // When you later want multiple stimuli (e.g., Health, Hunger, Fear), just extend
            // Replace stimulusValue with: float weightedStimulus = EvaluateStimulusFusion(context, allStimuli);
            
            var deltaStimulus = Mathf.Abs(stimulusValue - _lastStimulusValue);
            var switchDeltaTime = Time.time - _lastSwitchTime;
            
            if (switchDeltaTime < _switchCooldown || deltaStimulus < 0.05f)
                return null; // debounce & jitter guard
                
            var curveProfiles = context.AgentProfiles.GetCurveProfile("DefaultCurves");
            if (curveProfiles == null || curveProfiles.Count == 0)
            {
                Debug.LogError($"[{ScriptName}] No curve profiles found for stimulus-driven switching.");
                return null;
            }

            var currentScore = 0.0f;
            var bestScore = float.MinValue;
            var bestTreeKey = currentTreeKey;

            foreach (var curve in curveProfiles)
            {
                var score = EvaluateCurveResponse(curve, stimulusValue);
                
                // Prefer previous selection unless score delta is significant
                if (curve.StimuliBehaviorTree == currentTreeKey)
                    currentScore = score;
                if (curve.StimuliBehaviorTree == _lastTreeKey)
                    score += _hysteresisThreshold; // Sticky bias
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTreeKey = curve.StimuliBehaviorTree;
                }
                //Debug.Log($"[{ScriptName}] Curve '{curve.CurveName}' scored {score:0.000} for {stimulusKey}={stimulusValue}");
            }
            
            if (bestTreeKey != currentTreeKey && bestScore - currentScore >= 0.05f)
            {
                _lastTreeKey = bestTreeKey;
                _lastSwitchTime = Time.time;
                _lastStimulusValue = stimulusValue;
                Debug.Log($"[{ScriptName}] Stimulus response selected new tree: {bestTreeKey} (was: {currentTreeKey})");
                return bestTreeKey;
            }
            return null; // No changes
        }

        private float EvaluateCurveResponse(CurveProfileEntry curve, float x)
        {
            return curve.CurveType switch
            {
                "Linear" => curve.Max * Mathf.Clamp01(x),
                "Gaussian" => curve.Max * Mathf.Exp(-curve.Sharpness * Mathf.Pow(x - curve.Center, 2)),
                "Sigmoid" => curve.Max / (1 + Mathf.Exp(-curve.Sharpness * (x - curve.Center))),
                _ => 0f
            };
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

        public void Reset() => _defaultTreeKey = null;
        
        public void ReleaseSystem(BtContext context)
        {
            // Remove all persona rules, default keys, events, etc.
            _rules?.Clear();
            _defaultTreeKey = null;
            OnSwitchRequested = null;
        }
    }
}
