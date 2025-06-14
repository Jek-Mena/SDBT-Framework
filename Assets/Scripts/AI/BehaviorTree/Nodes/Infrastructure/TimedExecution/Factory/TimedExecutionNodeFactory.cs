using System;

public class TimedExecutionNodeFactory<TNode> : IBtNodeFactory where TNode : IBehaviorNode
{
    private readonly string _alias;

    protected TimedExecutionNodeFactory(string alias)
    {
        _alias = alias;
    }

    public virtual IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var blackboard = context.Blackboard;
        var timeData = BuildTimedExecutionData(nodeData, blackboard);

        // Instantiate the node (expects a TNode(TimedExecutionData) constructor)
        var node = (IBehaviorNode)Activator.CreateInstance(typeof(TNode), timeData);

        if (node is TimedExecutionNode timedNode)
            timedNode.Initialize(blackboard);

        blackboard.TimerData = timeData;
        return node;
    }

    protected TimedExecutionData BuildTimedExecutionData(TreeNodeData nodeData, Blackboard blackboard)
    {
        var context = typeof(TNode).Name;
        var config = nodeData.Settings;

        return new TimedExecutionData()
        {
            Label = $"{_alias}:{blackboard.GetHashCode()}",
            Duration = JsonUtils.GetFloatOrDefault(config, BtConfigFields.Common.Duration, 1f, context),
            StartDelay = JsonUtils.GetFloatOrDefault(config, BtConfigFields.Common.StartDelay, 0f, context),
            Interruptible = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.Interruptible, true, context),
            FailOnInterrupt =
                JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.FailOnInterrupt, true, context),
            ResetOnExit = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.ResetOnExit, true, context),
            Mode = config.Value<string>(BtConfigFields.Common.Mode) switch
            {
                "Loop" => TimeExecutionMode.Loop,
                "UntilSuccess" => TimeExecutionMode.UntilSuccess,
                "UntilFailure" => TimeExecutionMode.UntilFailure,
                _ => TimeExecutionMode.Normal
            }
        };
    }
}