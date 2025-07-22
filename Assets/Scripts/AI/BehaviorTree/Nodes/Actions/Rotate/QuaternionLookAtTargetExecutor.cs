using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class QuaternionLookAtTargetExecutor : IRotationExecutor
    {
        private const string ScriptName = nameof(QuaternionLookAtTargetExecutor);
        public RotateToTargetNodeType Type => RotateToTargetNodeType.QuaternionLookAt;

        private readonly Transform _agentTransform;
        private RotationData _currentSettings;
        private Transform _targetTransform;
        private Vector3 _lastSetTargetPosition;

        private const float DefaultSqrArrivalDistanceThreshold = 0.05f;
        private const float DefaultAngleThreshold = 1f;
        private const float DefaultRotationSpeed = 180f;
        
        private bool _isPaused;
        public QuaternionLookAtTargetExecutor(Transform agentTransform)
        {
            _agentTransform = agentTransform;
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
            
            if (IsWithinArrivalDistance(direction))
                return;

            var desired = Quaternion.LookRotation(direction.normalized);
            
            if (IsAngleAligned(direction))
            {
                _agentTransform.rotation = desired;
                return;
            }

            var rotateSpeed = _currentSettings?.Speed ?? DefaultRotationSpeed;
            var rotateStep = rotateSpeed * deltaTime;
            
            var maxDebugLength = 10f;
            var distanceToTarget = Mathf.Min(direction.magnitude, maxDebugLength);
            _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, desired, rotateStep);
            Debug.DrawRay(_agentTransform.position, _agentTransform.forward * distanceToTarget, Color.red, 0.1f);
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
            
            //Debug.Log("[QuaternionLookAtTarget] Accepted intent and UNPAUSED rotation.");
            _isPaused = false;
            _targetTransform = target;
            
            //Debug.Log($"[QuaternionLookAtTarget] Accepted intent: target={target.name}, settings={data}");
            return true;
        }
        
        public void ApplySettings(RotationData data)
        {
            if (data != null && _currentSettings == data) return;
            _currentSettings = data;
        }
        
        public void StartRotation()
        {
            _isPaused = false;
        }

        public void PauseRotation()
        {
            _isPaused = true;
        }

        public void CancelRotation()
        {
            _isPaused = false;
        }
        
        public bool IsFacingTarget()
        {
            if (!_targetTransform) return false;

            var toTarget = _targetTransform.position - _agentTransform.position;
            toTarget.y = 0;

            return IsWithinArrivalDistance(toTarget) || IsAngleAligned(toTarget);
        }
        
        private bool IsWithinArrivalDistance(Vector3 toTarget)
        {
            var sqrArrival = _currentSettings?.SqrArrivalDistanceThreshold ?? DefaultSqrArrivalDistanceThreshold;
            return toTarget.sqrMagnitude < sqrArrival;
        }

        private bool IsAngleAligned(Vector3 directionToTarget)
        {
            var angle = Vector3.Angle(_agentTransform.forward, directionToTarget.normalized);
            var angleThreshold = _currentSettings?.AngleThreshold ?? DefaultAngleThreshold;
            return angle <= angleThreshold;
        }
    }
}