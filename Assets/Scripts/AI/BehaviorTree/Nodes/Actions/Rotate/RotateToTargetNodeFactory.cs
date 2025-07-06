using System;
using AI.BehaviorTree.Runtime.Context;

public class RotateToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(RotateToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {BtJsonFields.Config} for RotateToTarget node.");

        // Get the rotation profile key
        var rotationProfileKey = config[BtJsonFields.ConfigFields.Rotation]?.ToString();
        
        // Get the targeting profile key
        var targetProfileKey = config[BtJsonFields.ConfigFields.Target]?.ToString();
        
        return new RotateToTargetNode(rotationProfileKey, targetProfileKey);
    }
}

