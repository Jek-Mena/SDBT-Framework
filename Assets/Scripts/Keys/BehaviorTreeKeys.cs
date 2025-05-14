public static class BehaviorTreeKeys
{
    public static class Plugin
    {
        public const string BtLoadTree = "Plugin/BtLoadTree";
    }

    public static class Schema
    {
        public const string BtLoadTree = "Schema/BtLoadTree";
    }

    public static class Alias
    {
        public static class Composite
        {
            public const string Sequence = "Bt/Sequence";
            public const string Selector = "Bt/Selector";
            public const string Parallel = "Bt/Parallel";
        }

        public static class Decorator
        {
            public const string Inverter = "Bt/Inverter";
            public const string Repeater = "Bt/Repeater";
            public const string Succeeder = "Bt/Succeeder";
            public const string Failer = "Bt/Failer";
        }

        // etc.
    }

    public static class Json
    {
        public static class Node
        {
            /// <summary>
            /// Number of times to repeat the child node. Used by BT/Repeater.
            /// </summary>
            public const string MaxRepeats = "maxRepeats";
        }
    }
}