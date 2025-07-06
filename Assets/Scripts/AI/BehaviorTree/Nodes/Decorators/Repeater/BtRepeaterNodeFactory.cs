using System;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Newtonsoft.Json.Linq;

public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var scriptName = nameof(BtRepeaterNodeFactory);
        var settings = nodeData.Settings; // Optional
        var maxRepeats = -1;
        
        // Optional safety net
        if (settings != null)
        {
            if (settings.ContainsKey(BtJsonFields.Ref))
                throw new InvalidOperationException($"[{scriptName}] Config contains unresolved {BtJsonFields.Ref}. ResolveRefs failed upstream.");
            maxRepeats = JsonUtils.GetIntOrDefault(settings, BtJsonFields.Config.Nodes.Repeater.MaxRepeats, -1, scriptName);
        }
        
        var childNode = buildChildNode(nodeData.GetSingleChild(scriptName));
        return new BtRepeaterNode(childNode, maxRepeats);
    }
}