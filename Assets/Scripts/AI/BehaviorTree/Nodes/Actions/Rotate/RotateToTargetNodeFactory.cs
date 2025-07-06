using System;
using AI.BehaviorTree.Runtime.Context;
using Keys;

public class RotateToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(RotateToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {BtJsonFields.ConfigField} for RotateToTarget node.");

        // Get the rotation profile key
        var rotationProfileKey = config[BtJsonFields.Config.Rotation]?.ToString();
        
        // Get the targeting profile key
        var targetProfileKey = config[BtJsonFields.Config.Target]?.ToString();
        
        return new RotateToTargetNode(rotationProfileKey, targetProfileKey);
    }
}

