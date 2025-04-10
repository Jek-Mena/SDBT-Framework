using System.Collections.Generic;

namespace Assets.Scripts.Shared.AI.BehaviorTree
{
    public class SelectorNode : IBehaviorNode
    {
        private readonly List<IBehaviorNode> _children;
        public SelectorNode(List<IBehaviorNode> children)
        {
            _children = children;
        }

        public BTStatus Tick(NPCBehaviorTreeController npc)
        {
            foreach (var child in _children)
            {
                var status = child.Tick(npc);
                if (status == BTStatus.Success)
                    return BTStatus.Success;
                if (status == BTStatus.Running)
                    return BTStatus.Running;
            }

            return BTStatus.Failure;
        }
    }
}