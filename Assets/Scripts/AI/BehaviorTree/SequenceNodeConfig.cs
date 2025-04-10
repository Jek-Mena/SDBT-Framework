//TODO ParallelNodeConfig.cs and SelectorNodeConfig.cs

using System.Collections.Generic;
using Assets.Scripts.Shared.AI.BehaviorTree;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BT Node/Sequence")]
public class SequenceNodeConfig : BTNodeConfig
{
    public List<BTNodeConfig> Children; // Must implement IBehaviorConfig

    public override IBehaviorNode CreateNode(BTNodeContext context)
    {
        var childNodes = new List<IBehaviorNode>();
        foreach (var child in Children)
        {
            childNodes.Add(child.CreateNode(context));
        }

        return new SelectorNode(childNodes);
    }
}