using System;
using UnityEngine;

public class QuaternionLookAt : MonoBehaviour, IRotationNode, IInitializeBehavior<RotationData>, IUsesStatusEffectManager
{
    private RotationData _rotationData;
    private StatusEffectManager _statusEffectManager;
    
    public void Initialize(RotationData data)
    {
        _rotationData = data;
    }
    
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

    private void OnDestroy()
    {
        if(!_statusEffectManager) return;

        _statusEffectManager.DomainBlocked -= OnDomainBlocked;
        _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
    }
    
    public void ApplySettings(RotationData data)
    {
        
    }

    public bool TryRotateTo(Vector3 targetPosition)
    {
        // Check for movement blocking
        if (!_statusEffectManager)
        {
            Debug.LogError($"[NavMeshMoveToTarget] {name} StatusEffectManager not found. Movement blocking will not be applied.");
            return false;
        }
        
        if(_statusEffectManager.IsBlocked(BlockedDomain.Rotation))
            return false;
        
        // Only rotate on the XZ plane (ignore Y axis to prevent "looking up/down" if you don't want that)
        var direction = targetPosition - transform.position;
        direction.y = 0;
        
        if (direction.sqrMagnitude < _rotationData.UpdateThreshold)
            return true;  // Already there or super close
        
        // Target rotation
        var targetRotation = Quaternion.LookRotation(direction.normalized);
        
        // Speed: Degrees per second from RotationData.Speed (make sure your Speed is set appropriately, e.g., 180 = half-turn per second)
        var step = _rotationData.Speed * Time.deltaTime;
        
        // Rotate towards the target smoothly
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

        return true;
    }

    public void StopRotation()
    {
        throw new System.NotImplementedException();
    }

    public void StartRotation()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFacingTarget(Vector3 targetPosition)
    {
        var toTarget = targetPosition - transform.position;
        toTarget.y = 0; // Ignore vertical tilt for Y-axis facing
    
        if (toTarget.sqrMagnitude < 0.01f)
            return true; // Target is super close

        // TODO add direction resolver similar to impulse
        var forward = transform.forward;
        forward.y = 0;

        var angle = Vector3.Angle(forward.normalized, toTarget.normalized);
        return angle <= _rotationData.ThresholdAngle;
    }

    public void OnDomainBlocked(string domain)
    {
        if (string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase))
            StopRotation();
    }

    public void OnDomainUnblocked(string domain)
    {
        if (string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase))
            StartRotation();
    }
}