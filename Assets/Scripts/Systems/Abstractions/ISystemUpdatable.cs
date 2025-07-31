using AI.BehaviorTree.Runtime.Context;

namespace Systems.Abstractions
{
    public interface ISystemUpdatable
    {
        void Update(BtContext context);
    }
}