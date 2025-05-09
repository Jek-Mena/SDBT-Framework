/// <summary>
/// Literal node type identifiers used in JSON behavior tree definitions.
/// These strings appear under the `"type"` field in node objects,
/// and are used to map JSON config to registered node factories.
///
/// Example JSON structure:
/// {
///     "type": "Timeout",
///     "config": {
///         "duration": 3.0
///     },
///     "children": {
///         "type": "MoveTo",
///         "config": {
///             "overrideSpeed": 3.5
///         }
///     }
/// }
///
/// Each string constant here (e.g., "Timeout", "MoveTo") must match:
/// - The alias used in IBtNodeKey<T>.Alias
/// - The type string used when registering node factories in BtNodeRegistry
///
/// This mapping is fail-fast. If "type" in JSON does not match a constant + registered key,
/// node creation will throw a descriptive error.
///
/// Use this class to centralize and validate all legal "type" strings in behavior JSON.
/// </summary>
public static class JsonLiterals
{
    public static class Behavior
    {
        /// <summary>
        /// Movement-related BT actions.
        /// </summary>
        public static class Movement
        {
            public const string MoveTo = "MoveTo";
            public const string ImpulseMover = "ImpulseMover";
        }

        /// <summary>
        /// Timed execution nodes (pause, delay, timeout, etc.).
        /// </summary>
        public static class TimedExecution
        {
            public const string Timeout = "Timeout";
            public const string Pause = "Pause";
        }

        /// <summary>
        /// Composite behavior node that controls execution flow among multiple children — e.g., Sequence, Selector.
        /// </summary>
        public static class Composite
        {
            public const string Sequence = "Sequence";
            public const string Selector = "Selector";
            public const string Parallel = "Parallel";
        }

        /// <summary>
        /// Behavior nodes that modify or conditionally control a single child node's execution.
        /// Used for logic like inversion, repetition, filtering, or constraints.
        /// </summary>
        public static class Decorator
        {
            public const string Inverter = "Inverter";
            public const string Repeater = "Repeater";
            public const string Succeeder = "Succeeder";
            public const string Failer = "Failer";
        }

    }
}