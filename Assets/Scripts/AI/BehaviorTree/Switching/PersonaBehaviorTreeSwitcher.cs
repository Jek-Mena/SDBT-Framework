using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Switching
{
    public class PersonaBehaviorTreeSwitcher : IBtPersonaSwitcher
    {
        private const string ScriptName = nameof(PersonaBehaviorTreeSwitcher);
        public event Action<string, string, string> OnSwitchRequested;

        private List<PersonaSwitchRule> _rules;
        private string _defaultTreeKey;
    
        // Load rules from context / agentProfiles on startup
        public PersonaBehaviorTreeSwitcher(BtContext context)
        {
            var personaProfileKey = context.AgentProfiles.CurrentPersonaProfileKey;
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

            Debug.Log($"[{ScriptName}]📤🌲Loaded {_rules.Count} rules. Default tree: {_defaultTreeKey}");
            Debug.Log($"[{ScriptName}]📤🌲Default tree: '{_defaultTreeKey}', All rules: {string.Join(", ", _rules.Select(r => $"{r.SituationKey}->{r.MainTreeKey}"))}");
            
            foreach (var rule in _rules)
                Debug.Log($"[{ScriptName}][PersonaRule] situationKey={rule.SituationKey} mainTreeKey={rule.MainTreeKey}");
        }
    
        public string EvaluateSwitch(BtContext context, string currentTreeKey)
        {
            // Hardwired for now, because you have no perception/stimuli
            var currentSituation = "Default";
            var match = _rules.FirstOrDefault(r => r.IsMatch(currentSituation));
            if (match != null && match.MainTreeKey != currentTreeKey)
            {
                OnSwitchRequested?.Invoke(currentTreeKey, match.MainTreeKey, $"Persona rule matched: {match}");
                return match.MainTreeKey;
            }
            // Fallback to default
            return _defaultTreeKey;
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
    }
}
