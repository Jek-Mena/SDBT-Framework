using System;

public class RotateToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(RotateToTargetNodeFactory);
        var config = nodeData.Settings ?? throw new Exception($"[{scriptName}] Missing {CoreKeys.Config} for RotateToTarget node.");
        var rotationData = RotationDataBuilder.FromConfig(config, scriptName);
        
        return new RotateToTargetNode(rotationData);
    }
}

