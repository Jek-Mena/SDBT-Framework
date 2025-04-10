using System.Collections.Generic;
using Assets.Scripts.Shared.AI;

public class ParallelNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public ParallelNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BTStatus Tick(NPCBehaviorTreeController npcBehaviorTree)
    {
        var allSucceeded = true;
        var anyRunning = false;

        foreach (var child in _children)
        {
            var result = child.Tick(npcBehaviorTree);

            if (result == BTStatus.Failure)
                return BTStatus.Failure;

            if (result == BTStatus.Running)
                anyRunning = true;

            if(result != BTStatus.Success)
                allSucceeded = false;
        }

        if (anyRunning) return BTStatus.Running;
        return allSucceeded ? BTStatus.Success : BTStatus.Running;
    }
}