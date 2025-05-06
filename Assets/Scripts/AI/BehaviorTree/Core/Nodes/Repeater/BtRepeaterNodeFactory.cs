using System;
using Newtonsoft.Json.Linq;

[BtNode(BtNodeName.ControlFlow.Repeater)]
public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(
        JObject config, 
        Blackboard blackboard, 
        Func<JToken, IBehaviorNode> buildChild
    )
    {
        var childToken = config[JsonFields.Children];
        if (childToken == null)
            throw new Exception($"[RepeaterNodeFactory] requires a child node.");

        var child = buildChild(childToken);
        var maxRepeats = config.Value<int?>(JsonKeys.Bt.MaxRepeats) ?? -1;

        return new BtRepeaterNode(child, maxRepeats);
    }
}