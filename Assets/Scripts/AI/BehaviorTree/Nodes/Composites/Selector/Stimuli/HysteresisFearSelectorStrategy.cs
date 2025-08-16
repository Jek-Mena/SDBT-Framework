using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.Composites.Selector.Stimuli
{
    public class HysteresisFearSelectorStrategy : IChildSelectorStrategy
    {
        private readonly float _enterThreshold;
        private readonly float _exitThreshold;
        private int _activeIndex = 0; // 0 = calm/subtree0, 1 = fear/subtree1

        public HysteresisFearSelectorStrategy(float enterThreshold = 0.6f, float exitThreshold = 0.4f)
        {
            _enterThreshold = enterThreshold;
            _exitThreshold = exitThreshold;
        }

        public int SelectChildIndex(IReadOnlyList<IBehaviorNode> children, BtContext context)
        {
            // Read latched, decayed fear value from blackboard
            var fear = context.Blackboard.Get(BlackboardKeys.Fear.CurrentLevel);

            if (_activeIndex == 0 && fear > _enterThreshold)
                _activeIndex = 1;
            else if (_activeIndex == 1 && fear < _exitThreshold)
                _activeIndex = 0;

            return _activeIndex;
        }
    }
}