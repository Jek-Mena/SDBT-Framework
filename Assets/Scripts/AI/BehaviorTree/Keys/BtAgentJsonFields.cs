namespace AI.BehaviorTree.Keys
{
    /// <summary>
    /// [2025-07-06 ARCHITECTURE NOTE]
    /// Defines the universal JSON grammar shared across all entity.
    /// These keys are NOT domain- or feature-specific.
    /// They are the backbone of the data schema.
    /// 
    /// - Use ONLY for parsing/loading config from JSON/YAML, never for runtime or blackboard.
    /// </summary>
    public static class BtAgentJsonFields
    {
        // Top-level fields
        public const string EntityId = "entityId";
        public const string Prefab = "prefab";
        public const string AgentCurrentPersonaProfile = "agentCurrentPersonaProfile";
        public const string AgentProfilesField = "agentProfiles";
        public const string BehaviorProfilesField = "behaviorProfiles";
    
        public static class AgentProfiles
        {
            public const string HealthProfiles = "HealthProfiles";
            public const string FearProfiles = "FearProfiles";
            public const string CurveProfiles = "CurveProfiles";
            public const string PersonaProfiles = "PersonaProfiles";
        
            public const string DefaultHealth = "DefaultHealth";
            public const string DefaultFear = "DefaultFear";
            public const string DefaultCurves = "DefaultCurves";
            public const string DefaultPersona = "DefaultPersona";
        
            public static class HealthProfile
            {
                public const string MaxHp = "MaxHP";
                public const string RegenRate = "RegenRate";
            }

            public static class FearProfile
            {
                public const string DetectionRange = "DetectionRange";
                public const string DecayDuration = "DecayDuration";
                public const string MaxDuration = "MaxDuration";
                public const string Threshold = "Threshold";
                public const string CurveType = "CurveType";
            }

            public static class CurveProfile
            {
                public const string CurveName = "curveName";
                public const string CurveType = "curveType";
                public const string Center = "center";
                public const string Sharpness = "sharpeness";
                public const string Max = "max";
                public const string BehaviorTree = "behaviorTree";
            }
        
            public static class PersonaProfile
            {
                public const string SituationKey = "situationKey";
                public const string BehaviorTree = "behaviorTree";
                
                public const string DefaultSituation = "Default";
            }
        }
    
        public static class BehaviorProfiles
        {
            public const string TargetingProfiles = "TargetingProfiles";
            public const string MovementProfiles = "MovementProfiles";
            public const string RotationProfiles = "RotationProfiles";
            public const string TimingProfiles = "TimingProfiles";
        
            public static class TargetingProfile
            {
                public const string TargetTag = "targetTag";
                public const string Style = "style";
                public const string BlackboardKey = "blackboardKey";
                public const string MaxRange = "maxRange";
                public const string MinRange = "minRange";
                public const string FallOffRange = "fallOffRange";
                public const string SearchRange = "searchRange";
                public const string MaxTargets = "maxTargets";
                public const string AllowNull = "allowNull";
            }
        
            public static class MovementProfile
            {
                public const string Speed = "speed";
                public const string AngularSpeed = "angularSpeed";
                public const string Acceleration = "acceleration";
                public const string StoppingDistance = "stoppingDistance";
                public const string UpdateThreshold = "updateThreshold";
                public const string Direction = "direction";
            }

            public static class RotationProfile
            {
                public const string Speed = "speed";
                public const string AngleThreshold = "angleThreshold";
                public const string UpdateThreshold = "updateThreshold";
            }

            public static class TimingProfile
            {
                public const string Label = "label";                    // Logical or label (e.g., "PauseAfterMove")
                public const string TimerId = "timerId";                // Affected domains (e.g., "Domain/Movement")
                public const string Duration = "duration";              // Duration of effect or wait
                public const string StartDelay = "startDelay";          // Delay before behavior starts
                public const string Interruptible = "interruptible";    // Can the behavior be interrupted early
                public const string FailOnInterrupt = "failOnInterrupt";// Should it return failure on interruption
                public const string ResetOnExit = "resetOnExit";        // Reset timer or state on early exit
                public const string Mode = "mode";                      // Determines how the timer or state should reset upon early exit
            }
        
            private static class Common
            {
                public const string Duration = "duration";              // Duration of effect or wait
                public const string Label = "label";                    // Logical or label (e.g., "PauseAfterMove")
                public const string Domains = "domains";                // Affected domains (e.g., "Domain/Movement")
                public const string StartDelay = "startDelay";          // Delay before behavior starts
                public const string Interruptible = "interruptible";    // Can the behavior be interrupted early
                public const string FailOnInterrupt = "fbtjsonailOnInterrupt";// Should it return failure on interruption
                public const string ResetOnExit = "resetOnExit";        // Reset timer or state on early exit
                public const string Mode = "mode";                      // Determines how the timer or state should reset upon early exit
            }
        }
    }
}