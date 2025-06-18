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
        
        // Get the movement profile key
        var movementProfileKey = config[BtConfigFields.Profiles.Movement]?.ToString();

        // Get the targeting profile key
        var targetProfileKey = config[BtConfigFields.Profiles.Targeting]?.ToString();
        
        return new MoveToTargetNode(movementProfileKey, targetProfileKey);
    }
}