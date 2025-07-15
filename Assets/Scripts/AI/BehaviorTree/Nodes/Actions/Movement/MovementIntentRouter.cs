using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Systems.StatusEffectSystem.Component;
using UnityEngine;
using UnityEngine.AI;
using Utils.Component;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public class MovementIntentRouter: IUsesStatusEffectManager
    {
        private const string ScriptName = nameof(MovementIntentRouter);
        private readonly Dictionary<MoveToTargetNodeType, IMovementExecutor> _executors;
        private readonly StatusEffectManager _statusEffectManager;
        private IMovementExecutor _currentExecutor;
        private MoveToTargetNodeType _currentExecutorType;
        private int _activeExecutorId = -1;
        private bool _hasLastMove;

        public MovementIntentRouter(BtContext context)
        {
            var navMeshAgent = context.Agent.RequireComponent<NavMeshAgent>();
            
            _executors = new Dictionary<MoveToTargetNodeType, IMovementExecutor>
            {
                { MoveToTargetNodeType.NavMesh, new NavMeshMoveToTargetExecutor(navMeshAgent) },
                { MoveToTargetNodeType.Transform, new TransformMoveToTargetExecutor(context.Agent.transform)}
            };

            _currentExecutorType = MoveToTargetNodeType.NavMesh;
            _currentExecutor = _executors[_currentExecutorType];
            
            Dispose();
            _statusEffectManager = context.Blackboard.StatusEffectManager;;
            _statusEffectManager.DomainBlocked += OnDomainBlocked;
            _statusEffectManager.DomainUnblocked += OnDomainUnblocked;
            
            Debug.Log($"[{ScriptName}] {nameof(MovementIntentRouter)} initialized for {context.Agent.name}");
        }
        
        public void SetCurrentType(MoveToTargetNodeType type)
        {
            if(_currentExecutorType == type) return;
            
            // Always fully stop the previous executor!
            if (_currentExecutor != null)
            {
                _currentExecutor.CancelMovement();
                Debug.Log($"[{ScriptName}] Cancelled previous movement executor: {_currentExecutor.Type}");
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
        
        /// <summary>
        /// Called by BTNode <see cref="MoveToTargetNode"/>
        /// </summary>
        public bool TryIssueMoveIntent(Vector3 destination, MovementData data, int executorId)
        {
            if (_activeExecutorId != executorId)
            {
                Debug.LogError(
                    $"[{ScriptName}] ❌ Move intent from unauthorized owner. Ignoring. " +
                    $"ExecutorId={executorId}, Active={_activeExecutorId}. " +
                    $"This means the current BT session/context does not own movement. " +
                    $"(Did you forget to call TakeOwnership() on tree switch? Is context.BtSessionId being updated?)"
                );
                return false;
            }
         
            //Debug.Log($"[MovementOrchestrator] TryMoveTo: type={data.MovementType}, target={destination}");
            if (_currentExecutor == null)
            {
                Debug.LogError($"[MovementOrchestrator] Executor NOT FOUND for {data.MovementType}");
                return false;
            }
            
            SetCurrentType(data.MovementType);
            _currentExecutor.ApplySettings(data);
            
            // --- Only act if intent changes ---
            if (!IsCurrentMove(destination, data))
            {
                Debug.Log($"[MovementOrchestrator] New movement intent. " +
                          $"Cancelling previous and moving to {destination} ({data.MovementType})");
                _currentExecutor.CancelMovement();
                _currentExecutor.StartMovement();
                _currentExecutor.AcceptMoveIntent(destination, data);

                return true;
            }

            // Already moving to this target with these params
            return true;
        }
        
        public bool IsCurrentMove(Vector3 destination, MovementData data)
        {
            return _currentExecutor?.IsCurrentMove(destination, data) ?? false;
        }
        
        public void Tick(float deltaTime)
        {
            if (_currentExecutor is ITickableExecutor tickable)
                tickable.Tick(deltaTime);
        }
        
        public void TakeOwnership(int newOwnerId)
        {
            if (_activeExecutorId == newOwnerId) return;
            
            _currentExecutor.CancelMovement();
            _activeExecutorId = newOwnerId;
            Debug.Log($"[{ScriptName}] Movement intent owner switched to {newOwnerId}");
        }

        public void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement/Rotation domain blocked, stopping executor.");
            _currentExecutor.PauseMovement();
        }

        public void OnDomainUnblocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log("[Orchestrator] Movement domain unblocked, resuming executor.");
            _currentExecutor.StartMovement();
        }
        
        // Be a good citizen—unsubscribe when destroyed/disposed!
        public void Dispose()
        {
            if(_statusEffectManager == null) return;
            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }
        
        public void CancelMovement() => _currentExecutor.CancelMovement();
        public void PauseMovement() => _currentExecutor.PauseMovement();
        public void StartMovement() => _currentExecutor.StartMovement();
        public bool IsAtDestination() => _currentExecutor.IsAtDestination();
        public int GetActiveOwnerId() => _activeExecutorId;

        public void ForceCancelAndReleaseOwnership()
        {
            CancelMovement();
            _activeExecutorId = -1;
        }
    }
}