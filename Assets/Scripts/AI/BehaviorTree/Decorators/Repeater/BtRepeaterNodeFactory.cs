using System;
using Newtonsoft.Json.Linq;

public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var config = JsonUtils.GetConfig(jObject, nameof(BtRepeaterNodeFactory));

        var childToken = jObject[JsonFields.Children];
        if (childToken == null)
            throw new Exception("[BtRepeaterNodeFactory] 'children' field required.");

        var child = build(childToken);
        var maxRepeats = config.Value<int?>(JsonFields.BtFields.MaxRepeats) ?? -1;

        return new BtRepeaterNode(child, maxRepeats);
    }
}