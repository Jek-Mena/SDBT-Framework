using Assets.Scripts.Shared.AI;

public interface IBehaviorModule
{
    BTStatus Tick(NPCBehaviorTreeController npc);  // Each behavior defines what "Tick" means
}