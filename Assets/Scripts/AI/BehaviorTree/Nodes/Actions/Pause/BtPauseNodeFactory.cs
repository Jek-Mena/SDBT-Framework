using Newtonsoft.Json.Linq;
using System;

public class BtPauseNodeFactory : TimedExecutionNodeFactory<BtPauseNode>
{
    public BtPauseNodeFactory() : base(BtNodeTypes.TimedExecution.Pause) { }
    
    public override IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> _)
    {
        var timeData = BuildTimedExecutionData(nodeData, blackboard); // Call base logic

        // Parse domains as before
        string[] domains = null;
        var config = nodeData.Settings;
        if (config != null && config.TryGetValue(CoreKeys.Domain, out var domainsToken))
        {
            if (domainsToken.Type == JTokenType.Array)
                domains = domainsToken.ToObject<string[]>();
            else if (domainsToken.Type == JTokenType.String)
                domains = new[] { domainsToken.ToString() };
        }

        var node = new BtPauseNode(timeData, domains);
        node.Initialize(blackboard); // Or whatever your setup requires
        blackboard.TimerData = timeData;
        return node;
    }
}