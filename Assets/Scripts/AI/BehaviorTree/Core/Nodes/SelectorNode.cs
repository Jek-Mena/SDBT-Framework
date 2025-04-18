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

        public BtStatus Tick(BtController controller)
        {
            foreach (var child in _children)
            {
                var status = child.Tick(controller);
                if (status == BtStatus.Success)
                    return BtStatus.Success;
                if (status == BtStatus.Running)
                    return BtStatus.Running;
            }

            return BtStatus.Failure;
        }
    }
}