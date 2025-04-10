using Assets.Scripts.Shared.AI;

public interface IBehaviorNode
{
    BTStatus Tick(NPCBehaviorTreeController npcController);
}
