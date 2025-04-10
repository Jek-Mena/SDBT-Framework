using Assets.Scripts.Shared.AI;
public class MoveBehavior : IBehaviorModule
{
    private readonly UnitCoordinator _coordinator;
    
    public MoveBehavior (UnitCoordinator coordinator)
    {
        _coordinator = coordinator;
    }

    public BTStatus Tick(NPCBehaviorTreeController npcController)
    {
        // Note to Self add a tolerance or threshold to avoid checking the position every tick in the TryRequestMove in the event of performance issue
        if(!_coordinator.TryRequestMove(_coordinator.Target.position))
            return BTStatus.Failure;

        return _coordinator.AtDestination() ? BTStatus.Success : BTStatus.Running;
    }
}