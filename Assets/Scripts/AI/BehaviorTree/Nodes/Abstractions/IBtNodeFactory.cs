using System;

public interface IBtNodeFactory
{
    /// <summary>
    /// Creates an instance of a behavior node using the provided node data, blackboard, and child node creation function.
    /// </summary>
    /// <param name="nodeData">The data associated with the tree node, including configuration and child structure.</param>
    /// <param name="blackboard">The shared blackboard instance used to manage state during execution.</param>
    /// <param name="buildChildNode">A function to build child nodes recursively based on the given tree node data.</param>
    /// <returns>An instance of <see cref="IBehaviorNode"/> representing the created behavior node.</returns>
    IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard,
        Func<TreeNodeData, IBehaviorNode> buildChildNode);
}