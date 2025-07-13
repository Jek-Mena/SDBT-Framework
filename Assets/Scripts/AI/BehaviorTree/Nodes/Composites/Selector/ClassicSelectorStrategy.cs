using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.Composites.Selector
{
    public class ClassicSelectorStrategy : IChildSelectorStrategy
    {
        private int _currentIndex = 0;
        public int SelectChildIndex(IReadOnlyList<IBehaviorNode> children, BtContext context)
        {
            while (_currentIndex < children.Count)
            {
                var status = children[_currentIndex].Tick(context);
                
                if (status == BtStatus.Success)
                {
                    var index = _currentIndex;
                    _currentIndex = 0;
                    return index;
                }

                if (status == BtStatus.Failure)
                    return _currentIndex++;
                
                _currentIndex++;
            }

            _currentIndex = 0;
            return -1;
        }
    }
}