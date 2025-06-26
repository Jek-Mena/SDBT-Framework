/// <summary>
/// Provides a centralized set of constant names for behavior tree node types.
/// This class organizes behavior tree node names into logical categories to
/// enhance discoverability and maintainability within the behavior tree system.
///
/// Currently used for BtDebugTools and ref their respective nodes to display the name for DebugOverlay
/// </summary>
public static class BtNodeDisplayName
{
    public static class Movement
    {
        public const string MoveToTarget = "MoveToTarget";
        public const string ImpulseMover = "ImpulseMover";
    }

    public static class Rotation
    {
        public const string RotateToTarget = "RotateToTarget";
    }

    public static class TimedExecution
    {
        public const string Pause = "Pause";
        public const string Timer = "Timer";
        public const string Sleep = "Sleep";
        public const string Channeling = "Channeling";
        public const string StandStill = "StandStill"; // Optional: alias with purpose
    }

    public static class Decorators
    {
        public const string Timeout = "TimeoutDecorator"; // TODO: Rewire this to use the TemporalCondition.Timeout 
        public const string Inverter = "Inverter";
        public const string Repeater = "Repeater";
    }

    public static class TemporalCondition
    {
        public const string Timeout = "Timeout";
    }
    
    public static class Composite
    {
        public const string Sequence = "Sequence";
        public const string Selector = "Selector";
        public const string Parallel = "Parallel";
    }
}