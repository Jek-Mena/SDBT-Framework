/// <summary>
/// BtConfigFields defines standardized JSON field names (keys) used across all behavior tree nodes and plugin configurations.
/// These constants represent the LEFT-HAND SIDE of the JSON config definitions.
///
/// Example Usage in JSON:
/// {
///   "type": "Bt/Pause",
///   "config": {
///     "label": "PauseAfterMove",      -- Common
///     "duration": 3.0,                -- Common
///     "domains": ["Domain/Movement"]  -- Common
///   }
/// }
///
/// Example Usage in Code:
/// JsonUtils.RequireFloat(config, BtConfigFields.Common.Duration, context);
/// </summary>
public static class BtConfigFields
{
    /// <summary>
    /// Universal config fields shared across multiple node types (e.g., Pause, TimeoutDecorator, Channel, Sleep).
    /// </summary>
    public static class Common
    {
        public const string Duration = "duration";              // Duration of effect or wait
        public const string Label = "label";                    // Logical or label (e.g., "PauseAfterMove")
        public const string Domains = "domains";                // Affected domains (e.g., "Domain/Movement")
        public const string StartDelay = "startDelay";          // Delay before behavior starts
        public const string Interruptible = "interruptible";    // Can the behavior be interrupted early
        public const string FailOnInterrupt = "failOnInterrupt";// Should it return failure on interruption
        public const string ResetOnExit = "resetOnExit";        // Reset timer or state on early exit
        public const string Mode = "mode";                      // Determines how the timer or state should reset upon early exit
    }

    /// <summary>
    /// Movement tuning fields used by navigation plugins and nodes like MoveToTarget.
    /// </summary>
    public static class Movement
    {
        public const string Speed = "speed";
        public const string AngularSpeed = "angularSpeed";
        public const string Acceleration = "acceleration";
        public const string StoppingDistance = "stoppingDistance";
        public const string UpdateThreshold = "updateThreshold";
    }

    public static class Rotation
    {
        public const string Speed = "speed";
        public const string AngleThreshold = "angleThreshold";
        public const string UpdateThreshold = "updateThreshold";
    }
    
    /// <summary>
    /// Impulse movement settings, especially for dash-style behaviors using RigidbodyImpulseMover.
    /// </summary>
    public static class Impulse
    {
        public const string Strength = "strength";
        public const string Tolerance = "tolerance";
        public const string StateTimeout = "stateTimeout";
    }

    /// <summary>
    /// Target selection fields used by targeting plugins or nodes that require spatial logic.
    /// </summary>
    public static class Targeting
    {
        public const string TargetTag = "targetTag";
        public const string TargetingStyle = "targetingStyle";
        public const string MaxRange = "maxRange";
        public const string AllowNull = "allowNull";
    }
}