using AI.BehaviorTree.Runtime.Context;

namespace Systems.Abstractions
{
    /// <summary>
    /// Marker for behavior nodes that require lifecycle hooks (e.g. OnExit).
    /// Currently supports OnExit(), but may expand to full lifecycle methods in the future:
    /// - OnEnter()
    /// - Reset()
    /// - Cleanup()
    /// 
    /// Consider renaming this to ILifecycleBehavior or refactoring to modular lifecycle interface:
    /// public interface ILifecycleBehavior
    /// {
    ///     void OnEnter();
    ///     void OnExit();
    ///     void Reset();
    /// }
    /// </summary>
    public interface ISystemCleanable
    {
        void ReleaseSystem(BtContext context);
    }
}