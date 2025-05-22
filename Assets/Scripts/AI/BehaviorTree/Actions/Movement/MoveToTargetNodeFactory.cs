using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(MoveToTargetNodeFactory);

        // Resolve config, including $ref
        var config = nodeData.Config;
        if (config == null)
            throw new Exception($"[{context}] Missing 'config' for MoveToTarget node.");

        // After ref resolution, config IS the movement config
        JObject movementConfig = config as JObject;

        if (movementConfig == null)
            throw new Exception($"[{context}] Could not resolve movement config for MoveToTargetNodeFactory.");

        var movementData = MovementDataBuilder.FromConfig(movementConfig, context);
        return new MoveToTargetNode(movementData);
    }
}