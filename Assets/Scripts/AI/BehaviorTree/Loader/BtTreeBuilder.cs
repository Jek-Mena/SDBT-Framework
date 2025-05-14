using System;
using Newtonsoft.Json.Linq;

public static class BtTreeBuilder
{
    public static IBehaviorNode Build(JToken json, Blackboard blackboard)
    {
        return Build(json, blackboard, nodeJson => Build(nodeJson, blackboard)); // Recursive
    }

    public static IBehaviorNode Build(JToken json, Blackboard blackboard, Func<JToken, IBehaviorNode> recurse)
    {
        if(json is not JObject obj)
            throw new Exception($"[BtTreeBuilder] Invalid node format (not JObject): {json}");

        var alias = obj[CoreKeys.Type]?.ToString();
        if (string.IsNullOrEmpty(alias))
            throw new Exception($"[BtTreeBuilder] Missing or empty 'type' in node: {obj}");

        var factory = BtNodeRegistry.GetFactoryByAlias(alias);
        return factory.CreateNode(obj, blackboard, recurse);
    }
}