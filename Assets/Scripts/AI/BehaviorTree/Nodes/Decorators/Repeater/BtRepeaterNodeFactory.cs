using System;
using Newtonsoft.Json.Linq;

public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(BtRepeaterNodeFactory);
        var settings = nodeData.Settings; // Optional
        var maxRepeats = -1;
        
        // Optional safety net
        if (settings != null)
        {
            if (settings.ContainsKey(CoreKeys.Ref))
                throw new InvalidOperationException($"[{context}] Config contains unresolved {CoreKeys.Ref}. ResolveRefs failed upstream.");
            maxRepeats = JsonUtils.GetIntOrDefault(settings, BtNodeFields.Repeater.MaxRepeats, -1, context);
        }
        
        var childNode = buildChildNode(nodeData.GetSingleChild(context));
        return new BtRepeaterNode(childNode, maxRepeats);
    }
}