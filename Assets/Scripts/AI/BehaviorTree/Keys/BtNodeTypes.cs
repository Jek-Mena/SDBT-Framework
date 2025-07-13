namespace AI.BehaviorTree.Keys
{
    /// <summary>
    /// BtNodeAliases defines canonical string keys for all behavior tree node types.
    /// These strings are used as identifiers in BT JSON files and for runtime factory lookup.
    /// Keeping them centralized ensures consistency and reduces typos or string drift across systems.
    ///
    /// First example usage:
    /// TreeNodeFactory.Create(BtNodeAliases.MoveToTarget, config);
    /// {
    ///   "type": "Bt/MoveToTarget",
    ///   "config": { ... }
    /// }
    ///
    /// Second example usage:
    /// 
    /// </summary>
    public static class BtNodeTypes
    {
        public static class Movement
        {
            public const string MoveToTarget = "Bt/MoveToTarget";
            public const string ImpulseMover = "Bt/ImpulseMover";
        }

        public static class Rotation
        {
            public const string RotateToTarget = "Bt/RotateToTarget";
        }

        public static class TimedExecution
        {
            public const string Pause = "Bt/Pause";
            public const string Timer = "Bt/Timer";
            public const string Sleep = "Bt/Sleep";
            public const string Channeling = "Bt/Channeling";
            public const string StandStill = "Bt/StandStill"; // Optional: alias with purpose
        }

        public static class Decorators
        {
            public const string Timeout = "Bt/TimeoutDecorator"; // TODO: Rewire this to use the TemporalCondition.Timeout 
            public const string Inverter = "Bt/Inverter";
            public const string Repeater = "Bt/Repeater";
        }

        public static class TemporalCondition
        {
            public const string Timeout = "Bt/Timeout";
        }
    
        public static class Composite
        {
            public const string Sequence = "Bt/Sequence";
            public const string Selector = "Bt/Selector";
            public const string Parallel = "Bt/Parallel";
            
            public const string StimuliSelector = "Bt/StimuliSelector";
        }
    }
}