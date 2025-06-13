using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(MoveToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{context}] Missing {CoreKeys.Config} for MoveToTarget node.");
        
        // Always build movement data (from resolved config, whether from $ref or inline), resolved upstream.
        var movementData = MovementDataBuilder.FromConfig(config, context);

        // If present, use a targeting profile key (post-$ref, resolution)
        var targetingData = config[CoreKeys.ResolvedProfiles.Targeting]?.ToObject<TargetingData>();
        
        return new MoveToTargetNode(movementData, targetingData);
    }
}