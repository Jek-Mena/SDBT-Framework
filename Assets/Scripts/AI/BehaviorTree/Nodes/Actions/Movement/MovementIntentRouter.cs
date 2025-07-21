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
    public class MovementIntentRouter: IUsesStatusEffectManager, ISystemCleanable
    {
        private const string ScriptName = nameof(MovementIntentRouter);
        private readonly Dictionary<MoveToTargetNodeType, IMovementExecutor> _executors;
        private readonly StatusEffectManager _statusEffectManager;
        private IMovementExecutor _currentExecutor;
        private MoveToTargetNodeType _currentExecutorType;
        private string _activeExecutorId; // GUID session
        private string _lastOwnerId; // For debug overlay

        public MovementIntentRouter(BtContext context)
        {
            _executors = new Dictionary<MoveToTargetNodeType, IMovementExecutor>
            {
                { MoveToTargetNodeType.NavMesh, new NavMeshMoveToTargetExecutor(context.Agent.RequireComponent<NavMeshAgent>()) },
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
        
        public void TakeOwnership(string newOwnerId)
        {
            if (_activeExecutorId != null && _activeExecutorId != newOwnerId)
                Debug.LogWarning($"[Domain][CLAIM][WARN] Movement was owned by {_activeExecutorId}, now claiming for {newOwnerId}.");
            //Debug.Log($"[{ScriptName}][Domain][CLAIM] Movement claimed by Session={newOwnerId} (was={_activeExecutorId})");
            
            _currentExecutor.CancelMovement();
            _lastOwnerId = _activeExecutorId;
            _activeExecutorId = newOwnerId;
        }
        
        public string GetActiveOwnerId() => _activeExecutorId;
        public string GetLastOwnerId() => _lastOwnerId;
        
        public void ReleaseSystem(BtContext context)
        {
            //Debug.Log($"[{ScriptName}] CleanupSystem called.");
            _currentExecutor?.CancelMovement();
            _lastOwnerId = _activeExecutorId;
            _activeExecutorId = null; // Reset executor ID so no orphan BT can claim it
            Dispose(); // Unsubscribe from status manager
            
            // "Full Nuke": Clear all  executors 
            foreach (var executor in _executors.Values)
                executor.CancelMovement();

            //Debug.Log($"[{ScriptName}] Cleanup complete.");
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
        public bool TryIssueMoveIntent(Vector3 destination, MovementData data, string executorId)
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
            
            if (_statusEffectManager.IsBlocked(DomainKeys.Movement))
            {
                Debug.Log($"[{ScriptName}] ❌ Move intent denied: Movement domain is blocked.");
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
            if (IsCurrentMove(destination, data)) return true;
            
            //Debug.Log($"[MovementOrchestrator] New movement intent. " +
            //          $"Cancelling previous and moving to {destination} ({data.MovementType})");
            _currentExecutor.CancelMovement();
            _currentExecutor.StartMovement();
            
            // Only return true if the executor successfully accepted the move intent.
            // If false, something is broken (unreachable, agent gone, etc) and the BT node will fail.
            return _currentExecutor.AcceptMoveIntent(destination, data);;

            // Already moving to this target with these params
        }
        
        public bool IsCurrentMove(Vector3 destination, MovementData data)
        {
            return _currentExecutor?.IsCurrentMove(destination, data) ?? false;
        }
        
        public void Tick(float deltaTime)
        {
            if (_statusEffectManager.IsBlocked(DomainKeys.Movement))
            {
                //Debug.Log($"[{ScriptName}] ❌ Move intent denied: Movement domain is blocked.");
                return;
            }
            
            if (_currentExecutor is ITickableExecutor executor)
                executor.Tick(deltaTime);
        }
        
        public void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement domain blocked, stopping executor.");
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
    }
}