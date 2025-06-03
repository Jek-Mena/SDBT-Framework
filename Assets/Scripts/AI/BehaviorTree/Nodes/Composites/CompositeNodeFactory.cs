using System;
using System.Linq;
using Newtonsoft.Json.Linq;

/// <summary>
/// Generic factory for composite nodes (e.g., Sequence, Selector).
/// Instantiates any IBehaviorNode subclass that accepts a List<see cref="IBehaviorNode"/> in its constructor.
/// Usage: Inherit for concrete node factories to reduce boilerplate.
/// </summary>
public abstract class CompositeNodeFactory<TNode> : IBtNodeFactory
    where TNode : IBehaviorNode
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var childrenArray = nodeData.Children;
        if (childrenArray== null || childrenArray.Count == 0)
            throw new Exception($"{typeof(TNode).Name} requires/missing a 'children' array.");

        var children = childrenArray
            .Select(childToken => buildChildNode(new TreeNodeData((JObject)childToken)))
            .ToList();

        // By default, assumes TNode(List<IBehaviorNode>)
        return CreateNodeInternal(children, nodeData, blackboard);
    }

    /// <summary>
    /// Override in derived factories if you need to pass more constructor arguments (e.g., custom config).
    /// Default behavior assumes a constructor with just children.
    /// </summary>
    protected virtual TNode CreateNodeInternal(
        System.Collections.Generic.List<IBehaviorNode> children,
        TreeNodeData nodeData,
        Blackboard blackboard
    )
    {
        // This default is for nodes like Sequence, Selector, etc.
        return (TNode)Activator.CreateInstance(typeof(TNode), children);
    }
}