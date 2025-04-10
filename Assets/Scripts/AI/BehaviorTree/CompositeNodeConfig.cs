using System.Collections.Generic;
using Assets.Scripts.Shared.AI.BehaviorTree;
using UnityEngine;

namespace Assets.Scripts.Shared.AI.Config
{
    [CreateAssetMenu(menuName = "AI/BT Node/Composite")]
    public class CompositeNodeConfig : BTNodeConfig
    {
        [SerializeField] private CompositeType _compositeType;
        [SerializeField] private List<BTNodeConfig> _children;

        public override IBehaviorNode CreateNode(BTNodeContext context)
        {
            var childNodes = new List<IBehaviorNode>();
            foreach (var child in _children)
            {
                var node = child?.CreateNode(context);
                if (node != null) childNodes.Add(node);
            }

            return _compositeType switch
            {
                CompositeType.Sequence => new SequenceNode(childNodes),
                CompositeType.Selector => new SelectorNode(childNodes),
                CompositeType.Parallel => new ParallelNode(childNodes),
                _ => null
            };
        }
    }
}