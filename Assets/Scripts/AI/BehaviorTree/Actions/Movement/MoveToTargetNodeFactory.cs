using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(MoveToTargetNodeFactory);

        var config = nodeData.Settings ?? throw new Exception($"[{context}] Missing 'config' for MoveToTarget node.");

        var movementData = MovementDataBuilder.FromConfig(config, context);
        return new MoveToTargetNode(movementData);
    }
}