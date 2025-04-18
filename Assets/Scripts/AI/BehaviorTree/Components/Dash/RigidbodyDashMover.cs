using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyDashMover : MonoBehaviour, IDashBehavior, IInitializeBehavior<DashData>
{
    private Rigidbody _rb;
    private TimedActionExecutor _actionExecutor;
    private Vector3 _direction;
    private float _speed;
    private float _stateTimeout;
    private float _tolerance;
    private float _elapsed;
    private bool _dashing;

    private void Awake()
    {
        _rb = this.RequireComponent<Rigidbody>();
        _actionExecutor = this.RequireComponent<TimedActionExecutor>();
    }

    public bool IsDashing() => _dashing;
    public bool IsDashComplete() => !_dashing;
    
    public void Initialize(DashData data)
    {
        if (_rb == null) return;
        _speed = data.speed;
        _tolerance = data.tolerance;
        _stateTimeout = data.stateTimeout;
    }

    public bool TryDashTo(Vector3 destination)
    {
        if (_dashing || _actionExecutor == null) return false;

        _actionExecutor.EnqueueFixedUpdate(new StartDashCommand(this, destination));
        return true;
    }

    public void PerformDash(Vector3 target)
    {
        _direction = (target - transform.position).normalized;

        var dashImpulse = _direction * _speed;

        _rb.linearVelocity = Vector3.zero; // reset prior movement

        _rb.AddForce(dashImpulse, ForceMode.Impulse);
        
        _elapsed = 0f;
        _dashing = true;
    }

    public void FixedUpdate()
    {
        if (!_dashing) return;

        _elapsed += Time.fixedDeltaTime;

        if (_elapsed >= _stateTimeout)
        {
            _dashing = false;
            _rb.linearVelocity = Vector3.zero;
        }

        Debug.DrawRay(transform.position, _rb.linearVelocity, Color.red, 0.1f);
    }
}