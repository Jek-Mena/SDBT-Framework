using System;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Composites.Parallel;
using Keys;

public class BtParallelNodeFactory : CompositeNodeFactory<BtParallelNode>
{
    protected override BtParallelNode CreateNodeInternal(
        System.Collections.Generic.List<IBehaviorNode> children,
        TreeNodeData nodeData
    )
    {
        var config = nodeData.Settings;
        var exitCondition = ParallelExitCondition.FirstSuccess;
        
        if (config != null && config.TryGetValue(BtJsonFields.Config.Nodes.Parallel.ExitCondition, out var exitToken) 
                           && Enum.TryParse(exitToken.ToString(), out ParallelExitCondition parsed))
            exitCondition = parsed;
        
        return new BtParallelNode(children, exitCondition);
    }
}