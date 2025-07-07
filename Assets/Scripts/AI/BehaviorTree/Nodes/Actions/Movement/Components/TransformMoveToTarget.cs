using System;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement.Components
{
    public class TransformMoveToTarget : MonoBehaviour, IMovementNode, IUsesStatusEffectManager
    {
        private const string ScriptName = nameof(TransformMoveToTarget);
        
        [SerializeField] private bool canBeBlocked = true;
        private bool _isBlocked;
        
        private MovementData _movementData;
        
        private StatusEffectManager _statusEffectManager;
        private bool _hasDestination;
        private Vector3 _lastSetDestination;

        public void Initialize(MovementData data)
        {
            _movementData = data;
            Debug.Log($"[{ScriptName}] Has been initialized.");
        }

        public void ApplySettings(MovementData data)
        {
            Debug.Log($"[{ScriptName}] Settings not yet implemented. Settings not applied to {name}");
        }

        public bool TryMoveTo(Vector3 destination)
        {
            if (!canBeBlocked || _isBlocked) return false;
            
            if (!_statusEffectManager)
            {
                Debug.LogError($"[{ScriptName}] {gameObject.name} {nameof(StatusEffectManager)} not found. Movement blocking will not be applied.");
                return false;
            }
            
            // [2025-07-07] TODO Might implement isMovementBlockable in DataMovement
            if(_statusEffectManager.IsBlocked(BlockedDomain.Movement))
                return false;
            
            var currentPosition = transform.position;
            var targetDirection = (destination - currentPosition).normalized;
            
            // Adjust for special direction if needed (Forward, Backward, Left, Right)
            var finalDirection = ResolveDirection(targetDirection, _movementData.Direction, Vector3.up);
            
            // Move by speed and deltaTime
            var moveStep = _movementData.Speed * Time.deltaTime;
            var newPosition = currentPosition + finalDirection * moveStep;
            
            // Clamp to not overshoot
            if (Vector3.Distance(currentPosition, destination) <= moveStep)
                newPosition = destination;
            
            transform.position = newPosition;
            return true;
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

        public bool IsAtDestination()
        {
            throw new System.NotImplementedException();
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

        public void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            
            if(!canBeBlocked) return;
            _isBlocked = true;
        }

        public void OnDomainUnblocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            
            if(!canBeBlocked) return;
            _isBlocked = false;
        }

        private void OnDestroy()
        {
            if(!_statusEffectManager) return;

            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }
    }
}