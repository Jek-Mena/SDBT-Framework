using Newtonsoft.Json.Linq;
using System;

[BtNode(BtNodeName.Tasks.MoveTo)]
public class MoveNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> recurse)
    {
        var config = BtJsonUtils.GetConfig(jObject);

        var stopRange = config.Value<float?>(JsonKeys.Movement.Speed) ?? 0.5f;
        var overrideSpeed = config.Value<float?>("overrideSpeed");
        var target = config.Value<string>("target") ?? "Player";

        return new MoveNode(); // Leaf node — no children
    }
}