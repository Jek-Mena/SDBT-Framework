using System;
using Newtonsoft.Json.Linq;

public class BtRepeaterNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var childToken = jObject[CoreKeys.Child];

        if (childToken == null)
            throw new Exception("[BtRepeaterNodeFactory] 'child' field required.");

        var child = build(childToken);

        var config = JsonUtils.GetConfig(jObject, nameof(BtRepeaterNodeFactory));
        var maxRepeats = config.Value<int?>(BehaviorTreeKeys.Json.Node.MaxRepeats) ?? -1;

        return new BtRepeaterNode(child, maxRepeats);
    }
}