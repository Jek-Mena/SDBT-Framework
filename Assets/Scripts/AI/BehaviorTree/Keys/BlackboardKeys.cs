namespace AI.BehaviorTree.Keys
{
    /// <summary>
    /// [2025-07-05]
    /// Centralized string keys for all dynamic, runtime blackboard fields used by the AI system.
    /// 
    /// - These are NOT config/profile selectors. These are per-agent, runtime "slots" for working memory/state.
    /// - Every key here is used as a string index for storing or retrieving values on the blackboard (typically a Dictionary&lt;string, object&gt;).
    /// - Grouped by domain/system for clarity. Only keep keys that are actively used in the current pipeline.
    /// - ONLY defined here if actually read/written by Blackboard.Set/Get in your pipeline.
    /// 
    /// Correct usage:
    /// <code>
    ///     // Store/update a value on the blackboard at runtime
    ///     Context.Blackboard.Set(BlackboardKeys.Fear.Level, currentFearValue);
    ///
    ///     // Retrieve a value from the blackboard at runtime
    ///     var source = Context.Blackboard.Get&lt;FearStimulus&gt;(BlackboardKeys.Fear.Source);
    /// </code>
    ///
    /// DO NOT use BlackboardKeys for:
    /// - Config/profile block or selector names (use AgentProfileSelectorKeys or NodeProfileSelectorKeys instead)
    /// - Static, editor-only, or design-time data
    /// </summary>
    public static class BlackboardKeys
    {
        /// <summary>
        /// [2025-07-24]
        /// "Group" category covers all runtime modules related to multi-agent coordination:
        /// - Squad (tactical teams, patrols, fireteams)
        /// - Party (RPG party systems, companions)
        /// - Minion/follower logic
        /// - Formations (lines, wedges, phalanxes)
        /// - Boss/raid groups, and similar group behaviors.
        /// 
        /// Guidelines:
        /// - Prefix keys with subcategory (e.g., "SquadAgent", "PartyLogic", "MinionManager").
        /// - Do NOT use "Group" for non-coordination systems (inventory, dialogue, etc).
        /// - If in doubt, document new usages here.
        /// </summary>
        public static class Group {
            public const string SquadAgent = "Group.SquadAgent";
        }
        
        public static class Multipliers
        {
            public const string Movement = "MovementMultiplier";
            public const string HealthMultiplier = "HealthMultiplier";
            public const string Armor = "ArmorMultiplier";
            public const string Attack = "AttackMultiplier";
        }

        public static class Health
        {
            public const string CurrentHealth = "CurrentHealth";
        }
    
        public static class Fear
        {
            public const string StimuliNearby = "StimuliNearby";
            public const string StimulusLevel = "FearStimulusLevel";
            public const string CurrentLevel = "CurrentFearLevel";
            public const string Cooldown = "FearCooldown";
            public const string Duration = "FearDuration";
            public const string Strength = "FearStrength";
            public const string Radius = "FearRadius";
            public const string Source = "FearSource";
            public const string FleePoint = "FearFleePoint";
        }
    }
}