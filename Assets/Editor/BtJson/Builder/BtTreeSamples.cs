using Newtonsoft.Json.Linq;

namespace Editor.BtJson.Builder
{
    public static class BtTreeSamples
    {
        private static CompositeNodeBuilder Composite => new();
        private static MovementActionBuilder Movement => new();
        private static TimedExecutionBuilder Timed => new();
        private static DecoratorNodeBuilder Decorator => new();
        /// <summary>
        /// Constructs a JSON object that represents the root node of a behavior tree.
        /// The node provided will be set as the entry point of the tree.
        /// </summary>
        /// <param name="node">
        /// A JSON object representing the behavior node to set as the root of the tree.
        /// </param>
        /// <returns>
        /// A JSON object containing the root node configuration for the behavior tree.
        /// </returns>
        public static JObject Root(JObject node) =>
            new JObject
            {
                [CoreKeys.Root] = node
            };
    
        public static JObject BasicChase()
        {
            return Root(
                Composite.Sequence(
                    Movement.MoveToTarget()
                )
            );
        }
    
        public static JObject MoveAndWait()
        {
            var moveTimeOutData = new TimedExecutionData()
            {
                Label = "MoveTimeout",
                Duration = 5
            };

            var pauseAfterMove = new TimedExecutionData()
            {
                Label = "PauseAfterMove",
                Duration = 5
            };
        
            return Root(
                Decorator.Repeater(
                    -1,
                    Composite.Sequence(
                        Composite.Parallel(
                            nameof(ParallelExitCondition.FirstSuccess),
                            Movement.MoveToTarget(),
                            Decorator.Timeout(moveTimeOutData)
                        ),
                        Timed.Pause(pauseAfterMove, DomainKeys.Default)
                    )
                )
            );
        }
    }
}