using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class QuaternionLookAtTarget : IRotationExecutor, ITickableExecutor
    {
        private const string ScriptName = nameof(QuaternionLookAtTarget);
        public RotateToTargetNodeType Type => RotateToTargetNodeType.QuaternionLookAt;

        private readonly Transform _agentTransform;
        private Transform _targetTransform;
        private RotationData _currentSettings;
        private Vector3 _targetPosition;
        private Vector3 _lastSetTargetPosition;

        private const float DefaultSqrArrivalDistanceThreshold = 0.05f;
        private const float DefaultAngleThreshold = 5f;
        private const float DefaultRotationSpeed = 180f;
        
        private bool _isPaused;
        public QuaternionLookAtTarget(Transform agentTransform)
        {
            _agentTransform = agentTransform;
            Debug.Log($"[{ScriptName}] Created.");
        }
        
        public void ApplySettings(RotationData data)
        {
            if (data != null && _currentSettings == data) return;
            _currentSettings = data;
        }
 
        /// <summary>
        /// Issues a new rotation command. Only call when intent changes.
        /// Do not call every frame.
        /// </summary>
        public bool AcceptRotateIntent(Transform target, RotationData data)
        {
            if (!target || data == null)
            {
                Debug.LogError($"[QuaternionLookAtTarget] Received null target/settings! Intent ignored.");
                return false;
            }
            _isPaused = false;
            Debug.Log("[QuaternionLookAtTarget] Accepted intent and UNPAUSED rotation.");
            _targetTransform = target;
            _targetPosition = target.position;
            Debug.Log($"[QuaternionLookAtTarget] Accepted intent: target={target.name}, settings={data}");
            return true;
        }
        
        /// <summary>
        /// Always rotate to latest target position, every frame.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (_isPaused || !_agentTransform || !_targetTransform)
                return;

            var agentPos = _agentTransform.position;
            var targetPos = _targetTransform.position;

            var direction = targetPos - agentPos;
            direction.y = 0;

            // Distance threshold from settings
            var sqrThreshold = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            if (direction.sqrMagnitude < sqrThreshold)
                return;

            var desired = Quaternion.LookRotation(direction.normalized);
            var angle = Quaternion.Angle(_agentTransform.rotation, desired);

            // Angle threshold from settings
            var angleThreshold = _currentSettings?.AngleThreshold ?? DefaultAngleThreshold;
            if (angle <= angleThreshold)
            {
                // Optional snap when close enough
                _agentTransform.rotation = desired;
                return;
            }

            var rotateSpeed = _currentSettings?.Speed ?? DefaultRotationSpeed;
            var rotateStep = rotateSpeed * deltaTime;

            _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, desired, rotateStep);
            Debug.DrawRay(_agentTransform.position, _agentTransform.forward * 2f, Color.red, 0.1f);
        }

        
        public RotationData GetSettings()
        {
            return _currentSettings;
        }
        
        public void StartRotation() =>_isPaused = false;
        public void PauseRotation()
        {
            _isPaused = true;
            Debug.Log("[QuaternionLookAtTarget] Paused rotation.");
        }

        public void CancelRotation()
        {
            _isPaused = false;
            _targetPosition = !_agentTransform ? Vector3.zero : _agentTransform.position;
        }
        
        /// <summary>
        /// Returns true if facing the current tracked target (within an angle threshold).
        /// </summary>
        public bool IsFacingTarget(Transform target)
        {
            if (!target)
            {
                Debug.LogError($"[{ScriptName}] Target Transform is null.");
                return false;
            }
            
            var agentPos = _agentTransform.position;
            var targetPos = target.position - agentPos;
            targetPos.y = 0;
            
            // Proximity check: are we close enough that facing doesn't matter?
            // NOT Do This? If you’re building a security camera AI, where only angle matters (and proximity is never a factor).
            // But for now, this will be implemented as a default
            var sqrArrival = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            if ((targetPos - agentPos).sqrMagnitude < sqrArrival)
                return true;
            
            var toTarget = (targetPos - agentPos).normalized;
            var angle = Vector3.Angle(_agentTransform.forward, toTarget);
            var angleThreshold = _currentSettings?.AngleThreshold ?? DefaultAngleThreshold;
            return angle <= angleThreshold;
        }
        
        public bool IsCurrentRotation(Transform targetTransform, RotationData data)
        {
            // 1. Target reference check: are we looking at the same object?
            if (!ReferenceEquals(_targetTransform, targetTransform))
                return false;
            
            // 2. Position threshold: are we close enough to its *current* position?
            var sqrDist = (_targetTransform.position - targetTransform.position).sqrMagnitude;
            if (sqrDist >= (data?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold))
                return false;
            
            // 3. Settings check: do we have the same settings, or is it null?
            if (data == null) 
            {
                Debug.LogError($"[{ScriptName}] RotationData is null.");
                return false;
            }
            
            return _currentSettings != null && data.Equals(_currentSettings);
            
            // Optionally, add angle check for precision if you want (not strictly necessary if position is enough):
            // var toTarget = target - _transform.position;
            // toTarget.y = 0;
            // var angle = Vector3.Angle(_transform.forward, toTarget.normalized);
            // if (angle >= (data.AngleUpdateThreshold ?? 2f)) return false;
        }
    }
}