using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class BtSequenceNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        // Ensure 'children' array exists
        var childrenArray = config[JsonFields.Children] as JArray;
        if (childrenArray == null || childrenArray.Count == 0)
            throw new System.Exception($"[BtSequenceNodeFactory] requires/missing a {JsonFields.Children} array.");

        // Recurse through children
        var children = childrenArray
            .Select(build)
            .ToList();

        return new BtSequenceNode(children);
    }
}