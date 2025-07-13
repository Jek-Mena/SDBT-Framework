using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Components.NavMesh;
using AI.BehaviorTree.Nodes.Actions.Movement.Components.TransformMovement;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Systems.StatusEffectSystem.Component;
using UnityEngine;
using UnityEngine.AI;
using Utils.Component;

namespace AI.BehaviorTree.Nodes.Actions.Movement.Components
{
    public class MovementOrchestrator : MonoBehaviour
    {
        private const string ScriptName = nameof(MovementOrchestrator);
        
        private Dictionary<MovementNodeType, IMovementExecutor> _executors;
        private IMovementExecutor _currentExecutor;
        private MovementNodeType _currentExecutorType;

        [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private bool useNavMesh = true;
        
        private BtContext _context;
        private StatusEffectManager _statusEffectManager;
        
        
        private Vector3 _lastDestination = Vector3.positiveInfinity;
        private MovementData _lastMoveData;
        private bool _hasLastMove = false;
        
        public void Initialize(BtContext context)
        {
            _context = context;
            agent = context.Agent.RequireComponent<NavMeshAgent>();
            
            _executors = new Dictionary<MovementNodeType, IMovementExecutor>
            {
                { MovementNodeType.NavMesh, new NavMeshMoveToTargetExecutor(agent) },
                { MovementNodeType.Transform, new TransformMoveToTargetExecutor(_context.Agent.transform)}
            };

            _currentExecutorType = MovementNodeType.NavMesh;
            _currentExecutor = _executors[_currentExecutorType];

            _context = context;
            Dispose(); // Unsubscribe from the previous status effect manager
            _statusEffectManager = _context.Blackboard.StatusEffectManager;;
            _statusEffectManager.DomainBlocked += OnDomainBlocked;
            _statusEffectManager.DomainUnblocked += OnDomainUnblocked;
        }
        
        public void SetCurrentType(MovementNodeType type)
        {
            if(_currentExecutorType == type) return;
            
            // Always fully stop the previous executor!
            if (_currentExecutor != null)
            {
                _currentExecutor.CancelMovement();
                Debug.Log($"[{ScriptName}] Cancelled previous executor: {_currentExecutor.Type}");
            }

            if (_executors.TryGetValue(type, out var executor))
            {
                _currentExecutor = executor;
                _currentExecutorType = type;
                Debug.Log($"[{ScriptName}] Switched executor to: {type}");
            }
            else
            {
                Debug.LogError($"[{ScriptName}] Executor not found for {type}");
            }
        }
        
        public bool IsCurrentMove(Vector3 destination, MovementData data)
        {
            return _hasLastMove
                   && Vector3.Distance(_lastDestination, destination) < data.UpdateThreshold
                   && data.Equals(_lastMoveData);
        }
        
        /// <summary>
        /// Called by BTNode <see cref="MoveToTargetNode"/>
        /// </summary>
        public bool TryMoveTo(Vector3 destination, MovementData data)
        {
            SetCurrentType(data.MovementType);
            
            _currentExecutor.ApplySettings(data);
            
            //Debug.Log($"[MovementOrchestrator] TryMoveTo: type={data.MovementType}, target={destination}");
            if (_currentExecutor == null)
            {
                Debug.LogError($"[MovementOrchestrator] Executor NOT FOUND for {data.MovementType}");
                return false;
            }
            
            // --- Only act if intent changes ---
            if (!IsCurrentMove(destination, data))
            {
                Debug.Log($"[MovementOrchestrator] New movement intent. Cancelling previous and moving to {destination} ({data.MovementType})");
                _currentExecutor.CancelMovement();
                var result = _currentExecutor.TryMoveTo(destination);
                _currentExecutor.StartMovement(); // Explicitly "go"

                _lastDestination = destination;
                _lastMoveData = data;
                _hasLastMove = true;

                return result;
            }

            // Already moving to this target with these params
            return true;
        }
        
        public void Tick(float deltaTime)
        {
            if (_currentExecutor is ITickableExecutor tickable)
                tickable.Tick(deltaTime);
        }
        
        private void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement domain blocked, stopping executor.");
            _currentExecutor?.PauseMovement();
        }

        private void OnDomainUnblocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log("[Orchestrator] Movement domain unblocked, resuming executor.");
            _currentExecutor?.StartMovement();
        }
        
        // Be a good citizen—unsubscribe when destroyed/disposed!
        public void Dispose()
        {
            if(!_statusEffectManager) return;
            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }
        
        public void CancelMovement() => _currentExecutor?.CancelMovement();
        public void PauseMovement() => _currentExecutor?.PauseMovement();
        public void StartMovement() => _currentExecutor?.StartMovement();
        public bool IsAtDestination() => _currentExecutor.IsAtDestination();
    }
}