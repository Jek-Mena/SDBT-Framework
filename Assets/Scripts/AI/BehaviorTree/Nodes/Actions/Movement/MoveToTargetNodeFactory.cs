using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(MoveToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
        {
            throw new Exception($"[{context}] Missing {CoreKeys.Config} for MoveToTarget node.");
        }

        var movementData = MovementDataBuilder.FromConfig(config, context);
        
        return new MoveToTargetNode(movementData);
    }
}