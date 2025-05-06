using UnityEngine;

public class Blackboard
{
    public IMovementNode MovementLogic;
    public ModifierProvider<MovementSettings> MovementModifiers;
    public IImpulseNode ImpulseLogic;

    public ITimedExecutionNode TimedExecutionLogic;
    public TimedExecutionData TimerData;

    public HealthSystem Health;
    public Transform Target; // To be replaced by Targeting System
    public bool IsStunned;

    // Add other runtime states as needed, but keep it focused
    public string CurrentAnimationState;
    public Vector3 ImpulseDirection;
} 