using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;
using Systems.FearPerception;

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

        public static class Target
        {
            // Might be in the wrong place, as this one is not used for blackboard
            public const string PlayerTag = "Player"; 
            public const string Formation = "Target.Formation";
            public const string Combat = "Target.Combat";
            public const string CurrentTarget = "Target.CurrentTarget";
            public const string CurrentTargetSource = "Target.CurrentTargetSource";
        }
        
        public static class Fear
        {
            // Stable handle to the long-lived list stored on the BB.
            public static readonly BbKey<IReadOnlyList<FearStimulus>> StimuliNearby = BbKey.Create<IReadOnlyList<FearStimulus>>("Fear.StimuliNearby");
            public static readonly BbKey<float> StimulusLevel = BbKey.Create<float>("Fear.StimulusLevel");
            public static readonly BbKey<float> CurrentLevel = BbKey.Create<float>("Fear.CurrentLevel");
            public const string Cooldown = "FearCooldown";
            public const string Duration = "FearDuration";
            public const string Strength = "FearStrength";
            public const string Radius = "FearRadius";
            public const string Source = "FearSource";
            public const string FleePoint = "FearFleePoint";
        }
    }
}