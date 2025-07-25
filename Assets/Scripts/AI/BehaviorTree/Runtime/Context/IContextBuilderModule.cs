namespace AI.BehaviorTree.Runtime.Context
{
    /// <summary>
    /// [ARCHITECTURE UPDATE -- 2025-06-17]
    /// All context builder modules receive the full BtContext,
    /// ensuring access to the Agent, Blackboard, Controller, and all other shared runtime systems.
    /// This avoids fragmenting the pipeline between GameObject/Blackboard-only phases and full-context phases.
    /// 
    /// This interface is for a single piece of logic (e.g., MovementContextBuilder),
    /// </summary>
    public interface IContextBuilderModule
    {
        /// <summary>
        /// Populates part of the given blackboard for the provided entity.
        ///
        /// - Must NOT create a new Blackboard.
        /// - Called once per entity during BtLoader.ApplyAll().
        /// - All modules operate on the same shared Blackboard instance.
        /// </summary>
        void Build(BtContext context);
    }
}