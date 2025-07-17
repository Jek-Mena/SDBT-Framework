using System;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using Keys;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public class MoveToTargetNodeFactory : IBtNodeFactory
    {
        public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNodeRecurs)
        {
            // TODO Check if we can use the Config at BtContext instead of nodeData.
            var scriptName = nameof(MoveToTargetNodeFactory);
            var config = nodeData.Settings;
            if (config == null)
                throw new Exception($"[{scriptName}] Missing {BtJsonFields.ConfigField} for MoveToTarget node.");
        
            // Get the movement and targeting profile key
            var movementProfileKey = config[BtJsonFields.Config.Movement]?.ToString();
            var targetProfileKey = config[BtJsonFields.Config.Target]?.ToString();
        
            return new MoveToTargetNode(movementProfileKey, targetProfileKey);
        }
    }
}