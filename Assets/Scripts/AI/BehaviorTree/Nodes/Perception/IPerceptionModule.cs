using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.Perception
{
    public interface IPerceptionModule
    {
        void Initialize(BtContext context);
        void UpdatePerception();
    }
}