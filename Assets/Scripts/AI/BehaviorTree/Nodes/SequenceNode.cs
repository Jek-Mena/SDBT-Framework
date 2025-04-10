using System.Collections.Generic;
using Assets.Scripts.Shared.AI;

public class SequenceNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public SequenceNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BTStatus Tick(NPCBehaviorTreeController npc)
    {
        foreach (var child in _children)
        {
            var status = child.Tick(npc);
            if (status != BTStatus.Success)
                return status;
        }
        return BTStatus.Success;
    }
}