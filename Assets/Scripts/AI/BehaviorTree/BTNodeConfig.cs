using UnityEngine;

public abstract class BTNodeConfig : ScriptableObject
{
    public abstract IBehaviorNode CreateNode(BTNodeContext context);
}