using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyImpulseNode : MonoBehaviour, IImpulseNode, IInitializeBehavior<ImpulseMoverData>
{
    private BtController _btController;
    private Rigidbody _rb;
    private UpdatePhaseExecutor _actionExecutor;
    private Vector3 _direction;
    private float _impulseStrength;
    private float _stateTimeout;
    private float _tolerance;
    private float _elapsed;
    private bool _dashing;

    [SerializeField] private ImpulseDirectionMode _directionMode = ImpulseDirectionMode.Forward;

    private void Awake()
    {
        _rb = this.RequireComponent<Rigidbody>();
        _actionExecutor = this.RequireComponent<UpdatePhaseExecutor>();
        _btController = this.RequireComponent<BtController>();
    }
    
    public void Initialize(ImpulseMoverData data)
    {
        _impulseStrength = data.ImpulseStrength;
        _tolerance = data.Tolerance;
        _stateTimeout = data.StateTimeout;
    }

    public bool TryImpulse()
    {
        if (_dashing || _actionExecutor == null) return false;

        var impulseDirection = ResolveDirection();

        _actionExecutor.EnqueueFixedUpdate(new StartImpulseCommand(this, impulseDirection));
        return true;
    }
    private Vector3 ResolveDirection()
    {
        Vector3? direction = _directionMode switch
        {
            ImpulseDirectionMode.Forward => transform.forward,
            ImpulseDirectionMode.Left => -transform.right,
            ImpulseDirectionMode.Right => transform.right,
            ImpulseDirectionMode.Up => transform.up,
            ImpulseDirectionMode.Down => -transform.up,
            ImpulseDirectionMode.ToTarget when _btController.Blackboard.Target != null => (_btController.Blackboard.Target.position - transform.position).normalized,
            ImpulseDirectionMode.AwayFromTarget when _btController.Blackboard.Target != null => (transform.position - _btController.Blackboard.Target.position).normalized,
            ImpulseDirectionMode.Custom when _btController.Blackboard.ImpulseDirection != Vector3.zero => _btController.Blackboard.ImpulseDirection.normalized,
            _ => null
        };

        if (direction.HasValue)
            return direction.Value;

        Debug.LogWarning($"ImpulseDirectionMode {_directionMode} could not resolve a valid direction, defaulting to transform.forward.");
        return transform.forward;

    }

    public void PerformImpulse(Vector3 direction)
    {
        _rb.linearVelocity = Vector3.zero; // stop prior movement
        _rb.AddForce(direction.normalized * _impulseStrength, ForceMode.Impulse);
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

    public bool IsImpulsing() => _dashing;
    public bool IsImpulseComplete() => !_dashing;
}