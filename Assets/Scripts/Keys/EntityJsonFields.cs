using Unity.VisualScripting;

/// <summary>
/// [2025-07-06 ARCHITECTURE NOTE]
/// Defines the universal JSON grammar shared across all entity.
/// These keys are NOT domain- or feature-specific.
/// They are the backbone of the data schema.
/// 
/// - Use ONLY for parsing/loading config from JSON/YAML, never for runtime or blackboard.
/// </summary>
public static class EntityJsonFields
{
    // Top-level fields
    public const string EntityId = "entityId";
    public const string Prefab = "prefab";
    public const string AgentProfilesField = "agentProfiles";
    public const string BehaviorProfilesField = "behaviorProfiles";
    
    public static class AgentProfiles
    {
        public const string HealthKey = "HealthProfiles";
        public const string FearKey = "FearProfiles";
        public const string SwitchKey = "SwitchProfiles";
        
        public const string DefaultHealth = "DefaultHealth";
        public const string DefaultFear = "DefaultFear";
        public const string DefaultSwitch = "DefaultSwitch";
        
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

        public static class SwitchProfile
        {
            public const string StimulusKey = "stimulusKey";
            public const string ComparisonOperator = "comparisonOperator";
            public const string Threshold = "threshold";
            public const string BehaviorTree = "behaviorTree";
        }
    }
    
    public static class BehaviorProfiles
    {
        public const string TargetingKey = "TargetingProfiles";
        public const string MovementKey = "MovementProfiles";
        public const string RotationKey = "RotationProfiles";
        public const string TimingKey = "TimingProfiles";

        
        
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
        }

        public static class RotationProfile
        {
            public const string Speed = "speed";
            public const string AngleThreshold = "angleThreshold";
            public const string UpdateThreshold = "updateThreshold";
        }

        public static class TimingProfile
        {
            public const string Label = Common.Label;
            public const string Duration = Common.Duration;
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
    
    [System.Obsolete]
    public const string Components = "components";
    [System.Obsolete]
    public const string Params = "params";
    public const string ExitCondition = "exitCondition"; // Should be in BtConfigFields
    public const string Domain = "Domain"; // Should be in BtConfigFields
}

public static class BtJsonFields  {
    public const string Root = "root";
    public const string Type = "type";
    public const string Child = "child";
    public const string Children = "children";
    public const string Config = "config";
    public const string Ref = "$ref";

    public static class ConfigFields
    {
        public const string Domains = "domains";
        public const string Target = "targetId";
        public const string Movement = "movementId";
        public const string Rotation = "rotationId";
        public const string Timing = "timingId";
    }
}