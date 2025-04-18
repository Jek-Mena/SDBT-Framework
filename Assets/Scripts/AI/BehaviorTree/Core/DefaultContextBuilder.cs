using UnityEngine;

public class DefaultContextBuilder : IContextBuilder
{
    public Blackboard Build(GameObject entity)
    {
        if (entity == null)
        {
            Debug.LogError("Entity is null.");
            return null;
        }

        var movement = entity.GetComponent<IMovementBehavior>();
        if (movement == null)
            Debug.LogError($"IMovementBehavior not found on entity {entity.name}.");

        var dash = entity.GetComponent<IDashBehavior>();
        if (dash == null)
            Debug.LogError($"IDashBehavior not found on entity {entity.name}.");

        var health = entity.GetComponent<HealthSystem>();
        if (health == null)
            Debug.LogError($"HealthSystem not found on entity {entity.name}.");

        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag not found");
            return null;
        }

        return new Blackboard
        {
            Movement = entity.GetComponent<IMovementBehavior>(),
            Dash = entity.GetComponent<IDashBehavior>(),
            Health = entity.GetComponent<HealthSystem>(),
            Target = GameObject.FindWithTag("Player").transform,
            IsStunned = false,
            CurrentAnimationState = "Idle",
            LastKnownTargetPosition = Vector3.zero
        };
    }
}