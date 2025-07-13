using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.Abstractions
{
    public interface IChildSelectorStrategy
    {
        int SelectChildIndex(IReadOnlyList<IBehaviorNode> children, BtContext context);
    }
}