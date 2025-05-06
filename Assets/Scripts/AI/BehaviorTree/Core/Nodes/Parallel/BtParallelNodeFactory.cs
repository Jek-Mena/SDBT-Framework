using System;
using System.Linq;
using Newtonsoft.Json.Linq;

[BtNode(BtNodeName.ControlFlow.Parallel)]
public class BtParallelNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(
        JObject config, 
        Blackboard blackboard, 
        Func<JToken, IBehaviorNode> buildChild)
    {
        var childrenToken = config[JsonFields.Children];
        if( childrenToken == null)
            throw new Exception("[BtParallelNodeFactory] 'children' field is missing.");

        var children = childrenToken
            .Select(buildChild)
            .ToList();

        return new BtParallelNode(children);
    }
}