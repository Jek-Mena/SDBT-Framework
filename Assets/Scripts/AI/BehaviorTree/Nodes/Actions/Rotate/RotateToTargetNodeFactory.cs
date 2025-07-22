using System;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class RotateToTargetNodeFactory : IBtNodeFactory
    {
        private const string ScriptName = nameof(RotateToTargetNodeFactory);
        
        public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNodeRecurs)
        {
            var config = nodeData.Settings;
            if (config == null)
                throw new Exception($"[{ScriptName}] Missing {BtJsonFields.ConfigField} for RotateToTarget node.");

            // Get the rotation and targeting profile key
            var rotationProfileKey = config[BtJsonFields.Config.Rotation]?.ToString();
            var targetProfileKey = config[BtJsonFields.Config.Target]?.ToString();

            if (rotationProfileKey == null)
            {
                Debug.LogError($"[{ScriptName}]🔁❌🔑Missing {BtJsonFields.Config.Rotation} for RotateToTarget node.");
                throw new Exception($"[{ScriptName}]🔁❌🔑Missing {BtJsonFields.Config.Rotation} for RotateToTarget node.");
            }

            if (targetProfileKey == null)
            {
                Debug.LogError($"[{ScriptName}]🔁❌🔑Missing {BtJsonFields.Config.Target} for RotateToTarget node.");
                throw new Exception($"[{ScriptName}]🔁❌🔑Missing {BtJsonFields.Config.Target} for RotateToTarget node.");       
            }
        
            //Debug.Log($"🔁🔑RotationProfileKey: {rotationProfileKey}---🎯🔑TargetProfileKey: {targetProfileKey}");
            return new RotateToTargetNode(rotationProfileKey, targetProfileKey);
        }
    }
}

