// NavMeshMover(Low - Level Logic)
// • What it does: Takes orders and interprets them intelligently
// • What it can do: Suppress redundant calls to SetDestination(), throttle updates, etc.

// TODO [Performance]: Replace UpdateThreshold logic with a shared coroutine
// that calls SetDestination() every X seconds, to batch pathfinding calls.

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMoveToTarget : MonoBehaviour, IMovementNode, IInitializeBehavior<MovementData>
{
    private Vector3 _lastSetDestination;
    private NavMeshAgent _agent;
    private MovementData _defaultData;

    private IMovementSettingsProvider _settingsProvider;
    private bool _hasDestination = false;

    public void Initialize(MovementData data)
    {
        _agent = this.RequireComponent<NavMeshAgent>();
        _defaultData = data;
    }

    public void SetSettingsProvider(IMovementSettingsProvider provider)
    {
        _settingsProvider = provider;
    }

    public bool TryMoveTo(Vector3 destination)
    {
        if (_agent == null || !_agent.enabled || _settingsProvider == null)
            return false;

        var settings = _settingsProvider.GetEffectiveSettings();

        if (settings.IsControlled)
        {
            _agent.isStopped = false;
            _agent.speed = 0;
            _agent.angularSpeed = 0;
            _agent.acceleration = 0;
            Debug.Log("[NavMeshMover] Movement is controlled (paused), not updating destination.");
            return true; // System is valid, just controlled (frozen, stun, etc.)
        }

        _agent.speed = settings.Speed ?? _defaultData.Speed;
        _agent.angularSpeed = settings.AngularSpeed ?? _defaultData.AngularSpeed;
        _agent.acceleration = settings.Acceleration ?? _defaultData.Acceleration;
        _agent.stoppingDistance = settings.StoppingDistance ?? _defaultData.StoppingDistance;

        var distance = Vector3.Distance(_lastSetDestination, destination);

        if (!_hasDestination || distance > _defaultData.UpdateThreshold)
        {
            _lastSetDestination = destination;
            _agent.SetDestination(destination);
            _hasDestination = true;
            Debug.Log("[BT] NavMeshMover -> status: Running");
        }

        // Debug.Log(
        //     $"[NavMeshMover] " +
        //     $"Threshold={_defaultData.UpdateThreshold} | " +
        //     $"Distance={Vector3.Distance(_lastSetDestination, destination)} | " +
        //     $"Speed={_agent.speed} | Acceleration={_agent.acceleration}" +
        //     $"Target destination={destination}"
        // );

        return true;
    }

    public bool IsAtDestination()
    {
        return !_agent.pathPending &&
               _agent.remainingDistance <= _agent.stoppingDistance &&
               (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
    }

    public void Stop()
    {
        if (_agent != null)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
            _hasDestination = false;
            return;
        }

        Debug.LogError($"{gameObject.name} NavMeshAgent is null");
    }
}