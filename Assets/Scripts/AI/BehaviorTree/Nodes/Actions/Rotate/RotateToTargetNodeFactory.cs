using System;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Unity.VisualScripting;
using UnityEngine;

public class RotateToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNodeRecurs)
    {
        var scriptName = nameof(RotateToTargetNodeFactory);
        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{scriptName}] Missing {BtJsonFields.ConfigField} for RotateToTarget node.");

        // Get the rotation and targeting profile key
        var rotationProfileKey = config[BtJsonFields.Config.Rotation]?.ToString();
        var targetProfileKey = config[BtJsonFields.Config.Target]?.ToString();

        if (rotationProfileKey == null)
        {
            Debug.LogError($"[{scriptName}]🔁❌🔑Missing {BtJsonFields.Config.Rotation} for RotateToTarget node.");
            throw new Exception($"[{scriptName}]🔁❌🔑Missing {BtJsonFields.Config.Rotation} for RotateToTarget node.");
        }

        if (targetProfileKey == null)
        {
            Debug.LogError($"[{scriptName}]🔁❌🔑Missing {BtJsonFields.Config.Target} for RotateToTarget node.");
            throw new Exception($"[{scriptName}]🔁❌🔑Missing {BtJsonFields.Config.Target} for RotateToTarget node.");       
        }
        
        Debug.Log($"🔁🔑RotationProfileKey: {rotationProfileKey}---🎯🔑TargetProfileKey: {targetProfileKey}");
        return new RotateToTargetNode(rotationProfileKey, targetProfileKey);
    }
}

