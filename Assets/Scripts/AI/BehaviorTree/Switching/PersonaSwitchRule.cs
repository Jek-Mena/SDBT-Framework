using System;
using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Switching
{
    public class PersonaSwitchRule
    {
        public string SituationKey { get; set; }
        public string MainTreeKey { get; set; }

        // Add more fields (comparison, value, etc) if you want rule-based switching.
        // For now, keep it simple: match on current situation.

        public bool IsMatch(string activeSituation)
            => string.Equals(SituationKey, activeSituation, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => $"{SituationKey} => {MainTreeKey}";
    }
}