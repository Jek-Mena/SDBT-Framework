// NavMeshMover(Low - Level Logic)
// • What it does: Takes orders and interprets them intelligently
// • What it can do: Suppress redundant calls to SetDestination(), throttle updates, etc.

// TODO [Performance]: Replace UpdateThreshold logic with a shared coroutine
// that calls SetDestination() every X seconds, to batch pathfinding calls.
//
// TODO: When implementing attack blocking, use the same pattern:
// if (blackboard.StatusEffectManager?.IsBlocked(BlockedDomain.Attack) ?? false)
//     return; // Block attack logic

using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMoveToTarget : MonoBehaviour, IMovementNode, IUsesStatusEffectManager
{
    // Optional control during freeze
    [SerializeField] private bool freezeRigidBody = true;
    private Rigidbody _rigidBody;
    
    private Vector3 _lastSetDestination;
    private NavMeshAgent _agent;
    private MovementData _movementData;

    private bool _hasDestination = false;

    // Reference to the StatusEffectManager, assigned either by the context builder or at runtime.
    private StatusEffectManager _statusEffectManager;

    // (Optional) Movement modifiers
    private ModifierStack<MovementSettings> _modifierStack = new();

    public void Initialize(MovementData data)
    {
        _agent = this.RequireComponent<NavMeshAgent>();
        _movementData = data;
        
        // Optional for this NavMesh
        _rigidBody = GetComponent<Rigidbody>();
    }

    ///<summary>Set the StatusEffectManager externally (from context builder/blackboard). </summary>
    public void SetStatusEffectManager(StatusEffectManager manager)
    {
        // Unsubscribe from previous manager (if any) to avoid leaks
        if (_statusEffectManager)
        {
            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }

        _statusEffectManager = manager;

        if (_statusEffectManager)
        {
            _statusEffectManager.DomainBlocked += OnDomainBlocked;
            _statusEffectManager.DomainUnblocked += OnDomainUnblocked;
        }
    }

    /// <summary>Apply settings each tick based on the BT node’s MovementData plus any modifiers.</summary>
    public void ApplySettings(MovementData data)
    {
        // Apply modifiers if present (optional)
        // TODO: Refactor modifier application logic.
        // - Move null-check (_modifierStack) and base settings construction into a helper method.
        // - Ensure MovementSettings is always fully populated after modifiers are applied.
        // - Remove redundant defensive null-coalescing when assigning agent values.
        // - Consider implementing a Null Object for modifier stack to simplify flow.
        // - Guarantee that modifier application never results in partial/invalid settings.
        MovementSettings effectiveSettings;
        if (_modifierStack != null)
            effectiveSettings = _modifierStack.Apply(new MovementSettings
            {
                Speed = data.Speed,
                AngularSpeed = data.AngularSpeed,
                Acceleration = data.Acceleration,
                StoppingDistance = data.StoppingDistance
            });
        else
            effectiveSettings = new MovementSettings
            {
                Speed = data.Speed,
                AngularSpeed = data.AngularSpeed,
                Acceleration = data.Acceleration,
                StoppingDistance = data.StoppingDistance
            };

        _agent.speed = effectiveSettings.Speed ?? data.Speed;
        _agent.angularSpeed = effectiveSettings.AngularSpeed ?? data.AngularSpeed;
        _agent.acceleration = effectiveSettings.Acceleration ?? data.Acceleration;
        _agent.stoppingDistance = effectiveSettings.StoppingDistance ?? data.StoppingDistance;
        Debug.Log($"[NavMeshMoveToTarget] Settings applied to {name}");
    }

    public bool TryMoveTo(Vector3 destination)
    {
        // Check for movement blocking
        if (!_statusEffectManager)
        {
            Debug.LogError($"[NavMeshMoveToTarget] {name} StatusEffectManager not found. Movement blocking will not be applied.");
            return false;
        }
        
        if(_statusEffectManager.IsBlocked(BlockedDomain.Movement))
            return false;

        // Check if the agent is valid on NavMesh
        if (!IsAgentValid())
            return false;

        // If first move or moved far enough, issue move command
        if (!_hasDestination || Vector3.Distance(_lastSetDestination, destination) > _movementData.UpdateThreshold)
        {
            _lastSetDestination = destination;
            var pathSet = _agent.SetDestination(destination);

            if (pathSet)
            {
                _hasDestination = true;
                return true;
            }

            Debug.LogWarning($"{gameObject.name} failed to SetDestination (may be unreachable or off mesh).");
            _hasDestination = false; // Being precise: can't move, so don't mark as having destination
            return false;
        }

        // No move was issued, but technically the agent is still on its way to the last set destination
        return true;
    }

    public bool IsAtDestination()
    {
        return !_agent.pathPending &&
               _agent.remainingDistance <= _agent.stoppingDistance &&
               (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
    }

    public void StopAgent()
    {
        if (_agent)
        {
            Debug.Log($"{gameObject.name} Stopping the NavMeshAgent.");
            _agent.ResetPath();
            _agent.isStopped = true;
            _hasDestination = false;
            return;
        }
        Debug.LogError($"{gameObject.name} NavMeshAgent is null");
    }

    public void StartAgent()
    {
        if (_agent)
        {
            Debug.Log($"{gameObject.name} Starting the NavMeshAgent.");
            _agent.isStopped = false;
            return;
        }
        Debug.LogError($"{gameObject.name} NavMeshAgent is null");
    }
    
    private void OnDestroy()
    {
        if(!_statusEffectManager) return;

        _statusEffectManager.DomainBlocked -= OnDomainBlocked;
        _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
    }

    public void OnDomainUnblocked(string domain)
    {
        if (string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase))
            StartAgent();
    }

    public void OnDomainBlocked(string domain)
    {
        if (string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase))
            StopAgent();
    }

    private bool IsAgentValid()
    {
        if (!_agent)
        {
            Debug.LogError($"[{name}] NavMeshAgent is null");
            return false;
        } 
        
        if (!_agent.isOnNavMesh)
        {
            Debug.LogWarning($"[{name}] NavMeshAgent is NOT on NavMesh");
            return false;
        }

        if (!_agent.enabled)
        {
            Debug.LogWarning($"[{name}] NavMeshAgent is DISABLED");
            return false;
        }

        // If agent is stopped, warn and refuse to move
        if (_agent.isStopped)
        {
            Debug.LogWarning($"{gameObject.name} TryMoveTo called while NavMeshAgent is stopped.");
            return false;
        }
        
        return true;
    }

}