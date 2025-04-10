using UnityEngine;

public class UnitCoordinator : MonoBehaviour
{
    public Transform Target { get; private set; }
    public bool IsStunned { get; private set; }

    private IUnitMovementSystem _movement;
    private HealthSystem _health;

    private Vector3 _lastRequestedPos;

    public void Awake()
    {
        _movement = GetComponent<IUnitMovementSystem>();
        if (_movement == null)
            Debug.LogError($"No UnitMovementSystem found on {gameObject.name} UnitCoordinator.");

        _health = GetComponent<HealthSystem>();
        if (_health == null)
            Debug.LogError($"No HealthSystem found on {gameObject.name} UnitCoordinator.");

        Target = GameObject.FindGameObjectWithTag("Player").transform;
        if (Target == null)
            Debug.LogError($"No Player Target found on {gameObject.name} UnitCoordinator.");

    }
    
    public bool TryRequestMove(Vector3 destination, float threshold = 1f)
    {
        if (IsStunned || _movement == null) 
            return false;

        if ((_lastRequestedPos - destination).sqrMagnitude < threshold * threshold)
            return false;

        _lastRequestedPos = destination;
        _movement.MoveTo(destination);
        return true;
    }

    public void CancelMove() => _movement?.Stop();
    public bool AtDestination() => _movement?.IsAtDestination() ?? false;
    public void SetStunned(bool value) => IsStunned = value;
}