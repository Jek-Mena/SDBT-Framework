using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

[BtNode(BtNodeName.ControlFlow.Sequence)]
public class BtSequenceNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard, Func<JToken, IBehaviorNode> recurse)
    {
        // Ensure 'children' array exists
        var childrenArray = config[JsonFields.Children] as JArray;
        if (childrenArray == null || childrenArray.Count == 0)
            throw new System.Exception($"[BtSequenceNodeFactory] requires/missing a {JsonFields.Children} array.");

        var children = childrenArray
            .Select(recurse)
            .ToList();

        return new BtSequenceNode(children);
    }
}