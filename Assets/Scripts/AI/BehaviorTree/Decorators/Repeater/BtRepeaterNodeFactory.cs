using System;
using Newtonsoft.Json.Linq;

public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
    {
        var context = nameof(BtRepeaterNodeFactory);

        // Support single child as "children" array (standard BT schema)
        if (nodeData.Children == null || nodeData.Children.Count != 1)
            throw new Exception($"[{context}] 'children' array with 1 element is required.");

        var childNode = buildChildNode(new TreeNodeData((JObject)nodeData.Children.First));

        var config = nodeData.Config;
        var maxRepeats = -1;

        if (config != null)
        {
            if (config.TryGetValue(CoreKeys.Ref, out _))
                config = BtConfigResolver.Resolve(nodeData.Raw, blackboard, context);

            maxRepeats = JsonUtils.GetIntOrDefault(config, BtNodeFields.Repeater.MaxRepeats, -1, context);
        }

        return new BtRepeaterNode(childNode, maxRepeats);
    }
}