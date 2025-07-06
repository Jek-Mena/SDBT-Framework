using Newtonsoft.Json.Linq;
using System;
using AI.BehaviorTree.Runtime.Context;
using Keys;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(MoveToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {BtJsonFields.ConfigField} for MoveToTarget node.");
        
        // Get the movement profile key
        var movementProfileKey = config[BtJsonFields.Config.Movement]?.ToString();

        // Get the targeting profile key
        var targetProfileKey = config[BtJsonFields.Config.Target]?.ToString();
        
        return new MoveToTargetNode(movementProfileKey, targetProfileKey);
    }
}