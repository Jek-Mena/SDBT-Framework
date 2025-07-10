namespace AI.BehaviorTree.Nodes.Abstractions
{
    public interface ITickableExecutor
    {
        /// <summary>
        /// Called by the orchestrator or agent controller each tick to advance movement state by deltaTime.
        /// Returns void by design; check state/progress with explicit queries (e.g., IsAtDestination()).
        /// See README: "On Subsystem Tick Methods".
        /// </summary>
        void Tick(float deltaTime);
    }
}