using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class QuaternionLookAtTarget : IRotationExecutor, ITickableExecutor
    {
        private const string ScriptName = nameof(QuaternionLookAtTarget);
        public RotateToTargetNodeType Type => RotateToTargetNodeType.QuaternionLookAt;

        private readonly Transform _transform;
        private RotationData _currentSettings;
        private Vector3 _targetPosition;
        private bool _isRotating;
        
        private const float DefaultSqrArrivalDistanceThreshold = 0.05f;
        private const float DefaultAngleThreshold = 5f;
        private const float DefaultRotationSpeed = 120f;
        
        public QuaternionLookAtTarget(Transform transform)
        {
            _transform = transform;
        }
        
        public void ApplySettings(RotationData data)
        {
            if (_currentSettings == null) throw new System.ArgumentNullException(nameof(data));
            if (_currentSettings != null && _currentSettings == data) return;
            _currentSettings = data;
        }
 
        /// <summary>
        /// Issues a new rotation command. Only call when intent changes.
        /// Do not call every frame.
        /// </summary>
        public bool TryRotateTo(Vector3 targetPosition)
        {
            if (!_transform)
            {
                Debug.LogError($"[{ScriptName}] Transform is null.");
                return false;
            }
            _targetPosition = targetPosition;
            _isRotating = true;
            return true;
        }
        
        /// <summary>
        /// Advances rotation by the current tick's deltaTime.
        /// Always called once per frame by the orchestrator/controller.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!_isRotating || !_transform) return;

            var currentPosition = _transform.position;
            var direction = _targetPosition - currentPosition;
            direction.y = 0; // Only rotate on XZ

            // Distance threshold to "arrive" at target
            var sqrArrivalThreshold = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            if (direction.sqrMagnitude < sqrArrivalThreshold)
            {
                _isRotating = false;
                return;
            }
            
            // Rotate toward the target at a configured speed
            var targetRotation = Quaternion.LookRotation(direction.normalized);
            var step = (_currentSettings?.Speed ?? DefaultRotationSpeed) * deltaTime;
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, step);

            // Stop if we're facing the target within an angle threshold
            if (IsFacingTarget(_targetPosition))
                _isRotating = false;
        }
        
        public void StartRotation()
        {
            if (!_transform) return;
            
            var sqrArrivalThreshold = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            if ((_transform.position - _targetPosition).sqrMagnitude > sqrArrivalThreshold)
                _isRotating = true;
        }

        public void PauseRotation()
        {
            _isRotating = false;
        }

        public void CancelRotation()
        {
            _isRotating = false;
            if (_transform)
                _targetPosition = _transform.position;
            else
                _targetPosition = Vector3.zero;
        }

        public bool IsFacingTarget(Vector3 targetPosition)
        {
            if (!_transform) return false;

            var toTarget = targetPosition - _transform.position;
            toTarget.y = 0;
                
            var sqrArrivalThreshold = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            if (toTarget.sqrMagnitude < sqrArrivalThreshold)
                return true;

            var forward = _transform.forward;
            forward.y = 0;

            var angle = Vector3.Angle(forward.normalized, toTarget.normalized);
            var angleThreshold = _currentSettings?.AngleThreshold ?? DefaultAngleThreshold;
            return angle <= angleThreshold;
        }
        
        public bool IsCurrentRotation(Vector3 target, RotationData data)
        {
            // Check if we have a target position to compare
            if (_targetPosition == Vector3.zero) return false;
            
            // 1. Position check: are we already rotating to (or at) the requested target?
            var sqrDist = (_targetPosition - target).sqrMagnitude;
            if (sqrDist >= (data?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold))
                return false;
            
            // 2. Settings check: are we rotating with the same settings?
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