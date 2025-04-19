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

        var health = entity.GetComponent<HealthSystem>();
        if (health == null)
            Debug.LogError($"HealthSystem not found on entity {entity.name}.");

        var player = GameObject.FindWithTag("Player");
        if (player == null)
            Debug.LogError("Player GameObject with tag not found");

        return new Blackboard
        {
            MovementLogic = movement,
            ImpulseLogic = impulse,
            TimedExecutionLogic = timer,
            Health = health,
            Target = player.transform,
            IsStunned = false,
            CurrentAnimationState = "Idle",
            ImpulseDirection = Vector3.zero
        };
    }
}