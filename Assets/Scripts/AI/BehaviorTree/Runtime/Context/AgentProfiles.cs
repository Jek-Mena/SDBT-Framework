using System;
using System.Collections.Generic;
using AI.BehaviorTree.Switching;

namespace AI.BehaviorTree.Runtime.Context
{
    public class AgentProfiles
    {
        private const string ScriptName = nameof(AgentProfiles);
        
        public Dictionary<string, TargetingData> TargetingProfiles { get; set; }
        public Dictionary<string, MovementData> MovementProfiles { get; set; }
        public Dictionary<string, RotationData> RotationProfiles { get; set; }
        public Dictionary<string, TimedExecutionData> TimingProfiles { get; set; }
        public Dictionary<string, HealthData> HealthProfiles { get; set; }
        public Dictionary<string, List<SwitchCondition>> SwitchProfiles { get; set; }
        public Dictionary<string, FearPerceptionData> FearProfiles { get; set; }

        /// [2025-06-18 ARCHITECTURE NOTE]
        /// All profile data access should use DRY helper methods (e.g., GetMovementProfile),
        /// which fail fast on errors. Do not access dictionaries directly from outside.
        public TargetingData GetTargetingProfile(string key)
            => GetProfile(TargetingProfiles, key, nameof(TargetingProfiles));

        public MovementData GetMovementProfile(string key)
            => GetProfile(MovementProfiles, key, nameof(MovementProfiles));

        public RotationData GetRotationProfile(string key)
            => GetProfile(RotationProfiles, key, nameof(RotationProfiles));

        public TimedExecutionData GetTimingProfile(string key)
            => GetProfile(TimingProfiles, key, nameof(TimingProfiles));

        public HealthData GetHealthProfile(string key)
            => GetProfile(HealthProfiles, key, nameof(HealthProfiles));

        public List<SwitchCondition> GetSwitchProfile(string key)
            => GetProfile(SwitchProfiles, key, nameof(SwitchProfiles));
        
        public FearPerceptionData GetFearPerceptionProfile(string key)
            => GetProfile(FearProfiles, key, nameof(FearProfiles));

        /// <summary>
        /// [2025-06-18 ARCHITECTURE NOTE]
        /// Safely retrieves a profile by key from the given dictionary.
        /// Throws a clear exception if missing (fail-fast, never returns null).
        /// Use for all profile-based lookups: movement, targeting, timing, etc.
        /// </summary>
        private TProfile GetProfile<TProfile>(Dictionary<string, TProfile> dict, string key, string dictName)
        {
            if (dict == null)
                throw new Exception(
                    $"[{ScriptName}] Profile dictionary '{dictName}' is null. Context/module may be missing.");
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception($"[{ScriptName}] Requested profile key is null or empty for '{dictName}'.");
            if (!dict.TryGetValue(key, out var profile))
                throw new Exception($"[{ScriptName}] Profile '{key}' not found in '{dictName}'. " +
                                    $"Available: [{string.Join(", ", dict.Keys)}]");
            return profile;
        }
        
        /// <summary>
        /// [2025-07-06] Refactored
        /// DumpContents for diagnostics after context build. 
        /// Migrated from the Blackboard which was added on [2025-06-13].
        /// </summary>
        public string DumpContents()
        {
            var lines = new List<string>();
            var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                var val = prop.GetValue(this);

                // Only interested in dictionaries (profile blocks)
                if (!prop.PropertyType.IsGenericType ||
                    prop.PropertyType.GetGenericTypeDefinition() != typeof(Dictionary<,>)) continue;
                
                var count = val is System.Collections.IDictionary dict ? dict.Count : 0;
                var status = val == null ? "MISSING" : (count > 0 ? $"OK ({count})" : "EMPTY");
                lines.Add($"- {prop.Name}: {status}");
            }
            return string.Join("\n", lines);
        }
        
        // TODO: Add attribute support for runtime GUI/Inspector visibility
    }
}