using System;

public class RotateToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(RotateToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {CoreKeys.Config} for RotateToTarget node.");

        // Get the rotation profile key
        var rotationProfileKey = config[BtConfigFields.Profiles.Rotation]?.ToString();
        
        // Get the targeting profile key
        var targetProfileKey = config[BtConfigFields.Profiles.Targeting]?.ToString();
        
        return new RotateToTargetNode(rotationProfileKey, targetProfileKey);
    }
}

