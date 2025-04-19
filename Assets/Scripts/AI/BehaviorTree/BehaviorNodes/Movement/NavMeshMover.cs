// This script is automatically applied to the GameObject via NavMeshMoverPlugin
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMover : MonoBehaviour, IMovementNode, IInitializeBehavior<MovementData>
{
    private NavMeshAgent _agent;

    private void Awake() => _agent = this.RequireComponent<NavMeshAgent>();

    public void Initialize(MovementData data)
    {
        if (_agent == null) return;
        _agent.speed = data.Speed;
        _agent.acceleration = data.Acceleration;
        _agent.stoppingDistance = data.StoppingDistance;
    }

    public bool TryMoveTo(Vector3 destination)
    {
        if (_agent == null || !_agent.enabled)
            return false;

        _agent.SetDestination(destination);
        return true;
    }

    public bool IsAtDestination()
    {
        if (_agent == null) return false;
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public void Stop()
    {
        if (_agent != null)
            _agent.ResetPath();
    }
}