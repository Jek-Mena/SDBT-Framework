using UnityEngine;

[CreateAssetMenu(fileName = "MoveNodeConfig" ,menuName = "AI/Behavior Config/Move")]
public class MoveNodeConfig : BTNodeConfig
{
    public override IBehaviorNode CreateNode(BTNodeContext context)
    {
        var behavior = new MoveBehavior(context.Coordinator);
        return new BehaviorLeafNode(behavior);
    }
}