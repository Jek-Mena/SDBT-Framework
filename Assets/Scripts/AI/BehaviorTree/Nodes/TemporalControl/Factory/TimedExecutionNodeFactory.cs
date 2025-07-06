using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Nodes.TemporalControl;
using AI.BehaviorTree.Runtime.Context;
using Newtonsoft.Json.Linq;

public class TimedExecutionNodeFactory<TNode> : IBtNodeFactory where TNode : IBehaviorNode
{
    private readonly string _alias;
    private readonly bool _hasChild;
    private readonly bool _acceptsDomains;

    // Map supported TNode types to constructors
    private static readonly Dictionary<Type, Func<IBehaviorNode, TimedExecutionData, string[], IBehaviorNode>> NodeConstructors =
        new()
        {
            { typeof(TimeoutNode), (child, timeData, domains) => new TimeoutNode(child, timeData, domains) },
            { typeof(BtPauseNode), (child, timeData, domains) => new BtPauseNode(timeData, domains) },
            { typeof(TimerNode),   (child, timeData, domains) => new TimerNode(timeData) },
            // Add future timed node types here
        };
    
    public TimedExecutionNodeFactory(string alias, bool hasChild = false, bool acceptsDomains = false)
    {
        _alias = alias;
        _hasChild = hasChild;
        _acceptsDomains = acceptsDomains;
    }

    public virtual IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var agentProfiles = context.AgentProfiles;

        var config = nodeData.Settings;
        if (config == null)
            throw new Exception($"[{_alias}] Missing 'config' for {typeof(TNode)} node.");
        
        // Get the timing profile key
        var timingProfileKey = config[BtConfigFields.Profiles.Timing]?.ToString();
        var timeData = agentProfiles.GetTimingProfile(timingProfileKey);
        
        IBehaviorNode child = null;
        if (_hasChild)
        {
            var children = nodeData.Children?
                .Select(childToken => buildChildNode(new TreeNodeData((JObject)childToken)))
                .ToList() ?? new List<IBehaviorNode>();
            
            if (children.Count != 1)
                throw new Exception($"[{_alias}] Node must have exactly one (1) child. Found: {children.Count}");
            child = children[0];
        }
        
        string[] domains = null;
        if (_acceptsDomains && config.TryGetValue(CoreKeys.Domain, out var domainsToken))
        {
            if (domainsToken.Type == JTokenType.Array)
                domains = domainsToken.ToObject<string[]>();
            else if (domainsToken.Type == JTokenType.String)
                domains = new[] { domainsToken.ToString() };
        }
        
        if (NodeConstructors.TryGetValue(typeof(TNode), out var ctor))
        {
            var node = ctor(child, timeData, domains);
            // Call Initialize if supported (covers TimedExecutionNode and derived types)
            (node as TimedExecutionNode)?.Initialize(context);
            return node;
        }
        
        throw new Exception($"[{_alias}] Unknown node type {typeof(TNode).Name} in factory.");
    }
}