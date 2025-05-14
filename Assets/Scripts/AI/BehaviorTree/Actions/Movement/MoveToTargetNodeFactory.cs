using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var context = nameof(MoveToTargetNodeFactory);
        var config = JsonUtils.GetConfig(jObject, nameof(MoveToTargetNodeFactory));

        Debug.Log($"[MoveToTargetNodeFactory] Creating 'MoveTo' node. [Config] {config} ");
        return new MoveToTargetNode(); // Leaf node — no children
    }
}