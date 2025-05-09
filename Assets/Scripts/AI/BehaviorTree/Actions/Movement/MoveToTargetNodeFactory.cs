using Newtonsoft.Json.Linq;
using System;

public class MoveToTargetNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        var config = JsonUtils.GetConfig(jObject, nameof(MoveToTargetNodeFactory));

        var stopRange = config.Value<float?>(JsonKeys.Movement.Speed) ?? 0.5f;
        var angularSpeed = config.Value<float?>(JsonKeys.Movement.AngularSpeed) ?? 120f;
        var overrideSpeed = config.Value<float?>(JsonKeys.Movement.OverrideSpeed);
        var target = config.Value<string>("target") ?? "Player";

        return new MoveToTargetNode(); // Leaf node — no children
    }
}