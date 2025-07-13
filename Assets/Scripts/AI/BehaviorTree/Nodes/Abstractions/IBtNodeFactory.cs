using System;
using AI.BehaviorTree.Runtime.Context;

public interface IBtNodeFactory
{
    /// <summary>
    /// [2025-06-14] Refactored all node factories to use BtContext exclusively.
    /// Creates an instance of a behavior node using the provided node data, blackboard, and child node creation function.
    /// </summary>
    /// <param name="nodeData">The data associated with the tree node, including configuration and child structure.</param>
    /// <param name="context">The shared context instance used to manage state during execution.</param>
    /// <param name="buildChildNodeRecurs">A function to build child nodes recursively based on the given tree node data.</param>
    /// <returns>An instance of <see cref="IBehaviorNode"/> representing the created behavior node.</returns>
    IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context,
        Func<TreeNodeData, IBehaviorNode> buildChildNodeRecurs);
}