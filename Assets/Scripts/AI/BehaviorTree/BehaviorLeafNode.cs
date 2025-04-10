using Assets.Scripts.Shared.AI;

public class BehaviorLeafNode : IBehaviorNode
{
    private readonly IBehaviorModule _behavior;

    public BehaviorLeafNode(IBehaviorModule behavior)
    {
        _behavior = behavior;
    }

    public BTStatus Tick(NPCBehaviorTreeController npc)
    {
        return _behavior.Tick(npc);
    }
}