using UnityEngine;

public class Blackboard
{
    public IMovementNode MovementLogic;
    public IImpulseNode ImpulseLogic;
    public ITimedExecutionNode TimedExecutionLogic;
    public HealthSystem Health;
    public Transform Target;
    public bool IsStunned;

    // Add other runtime states as needed, but keep it focused
    public string CurrentAnimationState;
    public Vector3 ImpulseDirection;
    public TimedExecutionData CurrentNodeData { get; set; }
}