using System;

// BtConditionNode.cs (New Base Class for Decorators)
// [PHASE 1: Uses BtContext internally]
public abstract class BtConditionNode : IBehaviorNode
{
    protected IBehaviorNode _child;
    
    /// <summary>
    /// Assigns a child node to this condition node.
    /// </summary>
    /// <param name="child">The child node to be assigned. Must implement <see cref="IBehaviorNode"/>.</param>
    public void SetChild(IBehaviorNode child)
    {
        _child = child;
    }

    public BtStatus Tick(BtController controller)
    {
        var context = new BtContext(controller);
        return Tick(context);
    }
    public abstract BtStatus Tick(BtContext context);
}

// [PHASE 1: Decorator-compatible, uses BtContext internally]

/*public class WithinRangeConditionFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard,
        Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(WithinRangeConditionFactory);

        // === Extract config ===
        var config = nodeData.Config;
        if (config == null) 
            throw new Exception($"[{context}] Missing 'config' for WithinRangeCondition node.");
        
        if (config.TryGetValue(CoreKeys.Ref, out _)) config = BtConfigResolver.Resolve(nodeData.Raw, blackboard, context);

        // === Extract range and target ===
        var range = JsonUtils.RequireFloat(config, BehaviorTreeKeys.Json.Node.Range, context);
        var target = blackboard.Target;
        if (target == null)
            throw new Exception($"[{context}] Target Transform not found in blackboard. Ensure it is properly set.");

        return new WithinRangeConditionNode(target, range);
    }
}*/