using System;
using Assets.Scripts.Shared.AI;

public class ConditionNode : IBehaviorNode
{
    private readonly Func<NPCBehaviorTreeController, bool> _condition;
    private readonly IBehaviorNode _child;

    public ConditionNode(Func<NPCBehaviorTreeController, bool> condition, IBehaviorNode child)
    {
        _condition = condition;
        _child = child;
    }

    public BTStatus Tick(NPCBehaviorTreeController npc)
    {
        return _condition.Invoke(npc) ? _child.Tick(npc) : BTStatus.Failure;
    }
}