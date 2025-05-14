using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class BtParallelNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var childrenToken = jObject[CoreKeys.Config];
        if (childrenToken is not JArray childrenArray || childrenArray.Count == 0)
            throw new Exception("[BtParallelNodeFactory] 'children' array is missing or empty.");

        var children = childrenArray.Select(build).ToList();
        return new BtParallelNode(children);
    }
}

// TODO: In future, parse config["mode"] and pass into BtParallelNode
// Currently defaults to ParallelExecutionMode.All
