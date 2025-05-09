using UnityEngine;

public class ContextBuilder : IContextBuilder
{
    public Blackboard Build(GameObject entity)
    {
        if (entity == null)
        {
            Debug.LogError("Entity is null.");
            return null;
        }

        var movement = entity.GetComponent<IMovementNode>();
        if (movement == null)
            Debug.LogError($"IMovementNode not found on entity {entity.name}.");

        var impulse = entity.GetComponent<IImpulseNode>();
        if (impulse == null)
            Debug.LogError($"IImpulseNode not found on entity {entity.name}.");

        var timer = entity.GetComponent<ITimedExecutionNode>();
        if (timer == null)
            Debug.LogError($"ITimedExecutionNode not found on entity {entity.name}.");

        var player = GameObject.FindWithTag("Player");
        if (player == null)
            Debug.LogError("Player GameObject with tag not found");

        return new Blackboard
        {
            MovementLogic = movement,
            MovementModifiers = new ModifierProvider<MovementSettings>(),
            ImpulseLogic = impulse,
            TimedExecutionLogic = timer,
            TimerData = new TimedExecutionData
            {
                Key = "", // Or a default you expect nodes to override later
                Duration = 3f,
                Interruptible = false,
                FailOnInterrupt = false
            },
            Target = player.transform,
            IsStunned = false,
            CurrentAnimationState = "Idle",
            ImpulseDirection = Vector3.zero
        };
    }
}