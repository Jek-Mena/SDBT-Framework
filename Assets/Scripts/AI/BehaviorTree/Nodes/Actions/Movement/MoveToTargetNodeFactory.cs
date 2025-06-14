using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(MoveToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {CoreKeys.Config} for MoveToTarget node.");
        
        // Always build movement data (from resolved config, whether from $ref or inline), resolved upstream.
        var movementData = MovementDataBuilder.FromConfig(config, scriptName);

        // If present, use a targeting profile key (post-$ref, resolution)
        var targetingData = config[CoreKeys.ResolvedProfiles.Targeting]?.ToObject<TargetingData>();
        
        return new MoveToTargetNode(movementData, targetingData);
    }
}