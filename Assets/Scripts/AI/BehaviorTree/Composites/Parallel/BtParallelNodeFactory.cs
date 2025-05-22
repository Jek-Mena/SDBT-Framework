using System;

public class BtParallelNodeFactory : CompositeNodeFactory<BtParallelNode>
{
    protected override BtParallelNode CreateNodeInternal(
        System.Collections.Generic.List<IBehaviorNode> children,
        TreeNodeData nodeData,
        Blackboard blackboard
    )
    {
        var config = nodeData.Config;
        var exitCondition = ParallelExitCondition.FirstSuccess;
        
        if (config != null && config.TryGetValue(BtNodeFields.Parallel.ExitCondition, out var exitToken) 
                           && Enum.TryParse(exitToken.ToString(), out ParallelExitCondition parsed))
            exitCondition = parsed;

        return new BtParallelNode(children, exitCondition);
    }
}