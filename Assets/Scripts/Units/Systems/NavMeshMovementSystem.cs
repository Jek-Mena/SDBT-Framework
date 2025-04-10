using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovementSystem : MonoBehaviour, IUnitMovementSystem, IRequire<IMovementAttributes>
{
    private NavMeshAgent _agent;
    
    public void Inject(IMovementAttributes attributes)
    {
        Debug.Log($"Injecting attributes to NavMeshMovementSystem {gameObject.name}");
        _agent.speed = attributes.Speed;
        _agent.acceleration = attributes.Acceleration;
        _agent.stoppingDistance = attributes.StoppingDistance;
    }
    public void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        if (_agent != null && _agent.enabled)
            _agent.SetDestination(destination);
    }

    public bool IsAtDestination()
    {
        if (_agent == null) return false;
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public void Stop()
    {
        if(_agent != null) _agent.ResetPath();
    }
}