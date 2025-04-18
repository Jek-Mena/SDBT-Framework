using UnityEngine;

public class Blackboard
{
    public IMovementBehavior Movement;
    public IDashBehavior Dash;
    public HealthSystem Health;
    public Transform Target;
    public bool IsStunned;

    // Add other runtime states as needed, but keep it focused
    public string CurrentAnimationState;
    public Vector3 LastKnownTargetPosition;
}