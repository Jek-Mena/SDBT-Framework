/// <summary>
/// Centralized keys for all behavior tree nodes and plugins that involve timed execution,
/// such as Pause, TimeoutDecorator, Sleep, Channeling, etc.
///
/// These keys serve multiple purposes:
/// - <b>Alias</b>: For use in JSON "type"/"btKey" fields to identify BT nodes.
/// - <b>Plugin</b>: For GameObject plugin registration (if applicable).
/// - <b>Schema</b>: To uniquely name schema specs for validation/editor tools.
/// - <b>Json</b>: Actual config field keys used inside node/plugin params.
/// </summary>
public static class TimedExecutionKeys
{
    /// <summary>
    /// Behavior Tree node aliases.
    /// These values are used in BT JSON under the "type" or "btKey" field.
    /// </summary>
    public static class Alias
    {
        public const string TimeoutDecorator = "Bt/TimeoutDecorator";
        public const string Pause = "Bt/Pause";
        public const string Sleep = "Bt/Sleep";
        public const string Channeling = "Bt/Channeling";
        public const string StandStill = "Bt/StandStill"; // Optional: alias with purpose
    }

    /// <summary>
    /// Plugin registration keys (for GameObject components).
    /// These are referenced in the "plugin" field of entity config.
    /// </summary>
    public static class Plugin
    {
        public const string Pause = "Plugin/Pause";
        public const string TimeoutDecorator = "Plugin/TimeoutDecorator";
    }

    /// <summary>
    /// Schema IDs used to validate configuration structure.
    /// Used in validation tools and editor integrations.
    /// </summary>
    public static class Schema
    {
        public const string Pause = "Schema/TimedExecution/Pause";
        public const string TimeoutDecorator = "Schema/TimedExecution/TimeoutDecorator";
        public const string Channeling = "Schema/TimedExecution/Channeling";
    }

    /// <summary>
    /// JSON keys used inside the "params" or "config" objects.
    /// These represent actual configuration values used by the system.
    /// </summary>
    public static class Json
    {
        public const string Duration = "duration";
        public const string Interruptible = "interruptible";
        public const string FailOnInterrupt = "failOnInterrupt";
        public const string ResetOnExit = "resetOnExit";
        public const string Mode = "mode"; // e.g., Normal, Loop, UntilSuccess, UntilFailure
        public const string Label = "label"; // Previously "key" — used for tagging/timeline/debug
        public const string StartDelay = "startDelay";

        public static class Modes
        {
            public const string Normal = "Normal";
            public const string Loop = "Loop";
            public const string UntilSuccess = "UntilSuccess";
            public const string UntilFailure = "UntilFailure";
        }
    }

    /// <summary>
    /// Logical execution phase this system belongs to.
    /// Can be used to group or sort plugins/behaviors.
    /// </summary>
    public static class Phase
    {
        public const string Execution = "Phase/TimedExecution";
    }
}