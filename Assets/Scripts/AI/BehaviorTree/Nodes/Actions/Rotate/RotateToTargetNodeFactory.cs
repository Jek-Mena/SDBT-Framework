using System;

namespace AI.BehaviorTree.Actions.Rotate
{
    public class RotateToTargetNodeFactory : IBtNodeFactory
    {
        public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
        {
            var context = nameof(RotateToTargetNodeFactory);
            var config = nodeData.Settings ?? throw new Exception($"[{context}] Missing {CoreKeys.Config} for RotateToTarget node.");
            var rotationData = RotationDataBuilder.FromConfig(config, context);
            
            return new RotateToTargetNode(rotationData);
        }
    }
}
