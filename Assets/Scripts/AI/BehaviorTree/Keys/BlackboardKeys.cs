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
    /// Stores the current agent's EntityRuntimeData (live config block) in the blackboard.
    /// Used by BT/Node/context systems for runtime config and profile resolution.
    /// Critical for dynamic config and context resolution at runtime.
    /// DO NOT REMOVE unless you refactor all BT/context/config resolution paths.
    /// </summary>
    public const string EntityConfig = "EntityConfig";
        
    /// <summary>
    /// [2025-06-13]
    /// Keys for core/shared AI systems and context fields.
    /// Use these for direct blackboard property lookups and diagnostics.
    /// </summary>
    public static class Core
    {
        // Subsystems or managers commonly stored on blackboard
        public static class Actions 
        {
            public const string MovementLogic = "MovementLogic";
            public const string ImpulseLogic = "ImpulseLogic";
            public const string RotationLogic = "RotationLogic";
        }
        
        public static class Managers
        {
            public const string StatusEffectManager = "StatusEffectManager";
            public const string TimeExecutionManager = "TimeExecutionManager";
            public const string UpdatePhaseExecutor = "UpdatePhaseExecutor";
        }
        
        public static class Common
        {
            public const string CurrentHealth = "CurrentHealth";
        }
        public static class Profiles
        {
            public const string TargetingProfiles = "TargetingProfiles";
            public const string MovementProfiles = "MovementProfiles";
        }
        
        public static class Data
        {   
            public const string TimerData = "TimerData";
            public const string TargetingData = "TargetingData";
        }
        
        
        public static class Resolver
        {
            public const string TargetResolver = "TargetResolver";
        }
        

        public static class Multipliers
        {
            public const string Movement = "MovementMultiplier";
            public const string HealthMultiplier = "HealthMultiplier";
            public const string Armor = "ArmorMultiplier";
            public const string Attack = "AttackMultiplier";
        }
        // Add more as needed.
    }

    public static class Health
    {
        public const string CurrentHealth = "CurrentHealth";
    }
    
    public static class Fear
    {
        public const string StimuliNearby = "StimuliNearby";
        public const string Level = "FearLevel";
        public const string Cooldown = "FearCooldown";
        public const string Duration = "FearDuration";
        public const string Strength = "FearStrength";
        public const string Radius = "FearRadius";
        public const string Source = "FearSource";
    }
}