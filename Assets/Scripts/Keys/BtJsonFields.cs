namespace Keys
{
    /// <summary>
    /// [2025-07-06]
    /// Canonical constants for all BT JSON fields.
    /// - Top-level fields: e.g. Root, Type, Config
    /// - Config.Nodes: All node-type-specific config fields (Repeater, Parallel, etc.)
    /// Example: BtJsonFields.Config.Nodes.Repeater.MaxRepeats
    /// </summary>
    public static class BtJsonFields  {
        public const string Root = "root";
        public const string Type = "type";
        public const string Child = "child";
        public const string Children = "children";
        public const string ConfigField = "config";
        public const string Ref = "$ref";

        public static class Config
        {
            public const string Domains = "domains";
            public const string Target = "targetId";
            public const string Movement = "movementId";
            public const string Rotation = "rotationId";
            public const string Timing = "timingId";

            public static class Nodes
            {
                public static class Repeater
                {
                    // Number of times to repeat the child node. Used by BT/Repeater.
                    public const string MaxRepeats = "maxRepeats";
                }

                public static class Parallel
                {
                    // Condition to exit the parallel node execution.
                    public const string ExitCondition = "exitCondition";
                }
            }
        }
    }
}