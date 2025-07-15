using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    /// <summary>
    /// Handles runtime movement via transform logic.
    /// - Use <c>TryMoveTo()</c> to issue a new movement intent (not every frame).
    /// - Use <c>Tick()</c> to advance movement each frame based on deltaTime.
    /// This separation prevents redundant commands and keeps progression clean.
    /// </summary>
    public class TransformMoveToTargetExecutor : IMovementExecutor, ITickableExecutor
    {
        private const string ScriptName = nameof(TransformMoveToTargetExecutor);
        public MoveToTargetNodeType Type => MoveToTargetNodeType.Transform;
        
        private readonly Transform _transform;
        private MovementData _currentSettings;
        private Vector3 _targetDestination;
        private Vector3 _lastSetDestination;
        private float _arriveThreshold;
        private bool _isMoving;

        private const float DefaultMoveSpeed = 1f;
        private const float DefaultStoppingDistance = 0.1f;
        private const float DefaultUpdateThreshold = 0.1f;
        public Direction DefaultDirection => Direction.Forward;
        
        public TransformMoveToTargetExecutor(Transform transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// Issues a new movement command. Only call when intent changes.
        /// Do not call every frame.
        /// </summary>
        public bool AcceptMoveIntent(Vector3 destination, MovementData data)
        {
            if (!_transform)
            {
                Debug.LogError($"[{ScriptName}] Transform is null.");
                return false;
            }
            
            // Always accept new move intent (idempotent).
            _targetDestination = destination;
            _lastSetDestination = destination;
            _currentSettings = data;
            _isMoving = true;
            return true;
        }

        /// <summary>
        /// Advances movement by the current tick's deltaTime.
        /// Always called once per frame by the orchestrator/controller.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!_isMoving || !_transform ) return;
            
            var currentPosition = _transform.position;
            var targetDirection = (_targetDestination - currentPosition).normalized;
            
            // Adjust for special direction if needed (Forward, Backward, Left, Right)
            var finalDirection = ResolveDirection(targetDirection, _currentSettings.Direction, Vector3.up);

            // Move by speed and deltaTime
            var moveStep = (_currentSettings?.Speed ?? DefaultMoveSpeed)  * deltaTime;
            var newPosition = currentPosition + finalDirection * moveStep;
            
            // Clamp to not overshoot
            if (Vector3.Distance(currentPosition, _targetDestination) <= moveStep)
                newPosition = _targetDestination;
            
            _transform.position = newPosition;
            // TODO Check or decide if we should handle the prevention here the
            // IsAtDestination should handle but will be called here.
            // Similar concern to IsFaceTarget of QuaternionLookAtTarget
        }
        
        public void ApplySettings(MovementData data)
        {
            if (_currentSettings != null && _currentSettings == data) return;
            _currentSettings = data;
            
            if (data.StoppingDistance > 0f)
                _arriveThreshold = data.StoppingDistance;
            else
                _arriveThreshold = DefaultStoppingDistance; // fallback default
        }

        /// <summary>
        /// Start/resume moving toward the current target (if any).
        /// </summary>
        public void StartMovement()
        {
            // Only resume if we have a valid target and aren't already at destination
            if (!_transform) return;
            if (Vector3.Distance(_transform.position, _targetDestination) > _arriveThreshold)
                _isMoving = true;
        }

        /// <summary>
        /// Pauses movement at the current position, but remembers the target.
        /// </summary>
        public void PauseMovement()
        {
            _isMoving = false;
        }

        /// <summary>
        /// Cancels movement and forgets the current target.
        /// </summary>
        public void CancelMovement()
        {
            _isMoving = false;
            if (_transform)
                _targetDestination = _transform.position;
            else
                _targetDestination = Vector3.zero;
            // Optionally, clear _lastSetDestination as well if that makes sense for your intent logic
        }

        /// <summary>
        /// True if we are stopped or within _arriveThreshold of destination.
        /// </summary>
        public bool IsAtDestination()
        {
            if (!_transform) return false;
            return !_isMoving || Vector3.Distance(_transform.position, _targetDestination) <= _arriveThreshold;
        }

        public bool IsCurrentMove(Vector3 destination, MovementData data)
        {
            // 1. Are we already moving to a "close enough" destination?
            var destinationMatch = Vector3.Distance(_lastSetDestination, destination) < data.UpdateThreshold;
            // 2. Are the movement settings the same?
            var settingsMatch = _currentSettings != null && _currentSettings.Equals(data);
            // 3. Are we actively moving (optional, but often makes sense for intent)
            var isMoving = _isMoving;
            return destinationMatch && settingsMatch && isMoving;
        }

        // Converts the intended movement direction based on MovementData.Direction
        private Vector3 ResolveDirection(Vector3 baseDir, Direction moveDir, Vector3 upAxis)
        {
            switch (moveDir)
            {
                case Direction.Backward:
                    return -baseDir;
                case Direction.Left:
                    return Vector3.Cross(upAxis, baseDir).normalized;
                case Direction.Right:
                    return Vector3.Cross(baseDir, upAxis).normalized;
                case Direction.Up:
                    return upAxis;
                case Direction.Down:
                    return -upAxis;
                case Direction.ForwardLeft:
                    return (baseDir + Vector3.Cross(upAxis, baseDir)).normalized;
                case Direction.ForwardRight:
                    return (baseDir + Vector3.Cross(baseDir, upAxis)).normalized;
                case Direction.BackwardLeft:
                    return (-baseDir + Vector3.Cross(upAxis, baseDir)).normalized;
                case Direction.BackwardRight:
                    return (-baseDir + Vector3.Cross(baseDir, upAxis)).normalized;
                case Direction.Forward:
                default:
                    return baseDir;
            }
        }
    }
}