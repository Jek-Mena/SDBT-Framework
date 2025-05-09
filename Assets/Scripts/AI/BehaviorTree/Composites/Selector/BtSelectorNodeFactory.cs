using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class BtSelectorNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var childrenToken = jObject[JsonFields.Children];
        if (childrenToken is not JArray childrenArray || childrenArray.Count == 0)
            throw new Exception($"[BtSelectorFactory] {JsonFields.Children} array is required and must not be empty.");

        var children = childrenArray.Select(build).ToList();

        return new BtSelectorNode(children);
    }
}