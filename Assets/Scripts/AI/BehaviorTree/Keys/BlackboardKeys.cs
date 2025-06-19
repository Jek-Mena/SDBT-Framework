/// <summary>
/// [2025-06-13] Refactored and extended BlackboardKeys for AI blackboard consistency.
/// - Centralizes all blackboard key definitions (dynamic and static) for all AI runtime blackboards.
/// - Enables typo-proof runtime field lookups and standardized logging across the AI context pipeline.
/// - Groups keys by domain (Skill, Core, etc.) for clarity and future extensibility.
/// 
/// /// === Note on Plural vs. Singular Naming ===
///   There was initial confusion about the difference between the plural C# field names (e.g., TargetingProfiles)
///   and the singular JSON selector keys (e.g., "targetingProfile"). This is intentional:
///   - Plural keys like 'TargetingProfiles' in C# represent dictionaries/collections on the blackboard,
///     holding all possible configurations of a given type for an entity.
///   - Singular keys like "targetingProfile" in JSON configs refer to the *selector* key, indicating which
///     entry in the plural collection should be used for a given BT node or action.
///   This distinction helps keep the runtime blackboard flexible (able to hold multiple configs)
///   while making JSON configs simple and readable (you only select the relevant key).
/// </summary>
public static class BlackboardKeys
{
    public const string EntityConfig = "EntityConfig";
        
    /// <summary>
    /// [2025-06-13]
    /// Keys for core/shared AI systems and context fields.
    /// Use these for direct blackboard property lookups and diagnostics.
    /// </summary>
    public static class Core
    {
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
        
        public static class Actions 
        {
            public const string MovementLogic = "MovementLogic";
            public const string ImpulseLogic = "ImpulseLogic";
            public const string RotationLogic = "RotationLogic";
        }

        public static class Resolver
        {
            public const string TargetResolver = "TargetResolver";
        }
        
        public static class Managers
        {
            public const string StatusEffectManager = "StatusEffectManager";
            public const string TimeExecutionManager = "TimeExecutionManager";
            public const string UpdatePhaseExecutor = "UpdatePhaseExecutor";
        }
        
        // Add more as needed.
    }
    
    /// <summary>
    /// Dynamic key builders for per-skill, per-ability, or runtime extension fields.
    /// Ensures all skill-related keys are typo-proof and consistently generated.
    /// </summary>
    public static class Skill
    {
        public static string Targeting(string skillKey) => $"Skill.Targeting.{skillKey}";
        public static string Target(string skillKey) => $"Skill.Target.{skillKey}";
        public static string Cooldown(string skillKey) => $"Skill.Cooldown.{skillKey}";
        public static string Timing(string skillKey) => $"Skill.Timing.{skillKey}";
        public static string Activation(string skillKey) => $"Skill.Activation.{skillKey}";
        public static string Movement(string skillKey) => $"Skill.Movement.{skillKey}";
        // Add more as needed for your use cases.
    }    
    
    // Optionally add more nested classes for future groupings (e.g., Status, Effects, Buffs, etc.)
}