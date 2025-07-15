using AI.BehaviorTree.Runtime.Context;

public interface IPerceptionModule
{
    void Initialize(BtContext context);
    void UpdatePerception();
}