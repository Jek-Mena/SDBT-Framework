using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Systems.StatusEffectSystem.Component;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class RotationIntentRouter: IUsesStatusEffectManager
    {
        private const string ScriptName = nameof(RotationIntentRouter);
        private readonly Dictionary<RotateToTargetNodeType, IRotationExecutor> _executors;
        private readonly StatusEffectManager _statusEffectManager;
        private IRotationExecutor _currentExecutor;
        private RotationData _lastRotationData;
        private RotateToTargetNodeType _currentExecutorType;
        private Vector3 _lastTarget;
        private int _activeExecutorId = -1;
        private bool _hasLastRotation;

        public RotationIntentRouter(BtContext context)
        {
            // Example: add all available executors for this agent
            _executors = new Dictionary<RotateToTargetNodeType, IRotationExecutor>
            {
                { RotateToTargetNodeType.QuaternionLookAt, new QuaternionLookAtTarget(context.Agent.transform) }
                // Add more executors as needed here
            };

            _currentExecutorType = RotateToTargetNodeType.QuaternionLookAt;
            _currentExecutor = _executors[_currentExecutorType];

            Dispose();
            _statusEffectManager = context.Blackboard.StatusEffectManager;;
            _statusEffectManager.DomainBlocked += OnDomainBlocked;
            _statusEffectManager.DomainUnblocked += OnDomainUnblocked;
            
            Debug.Log($"[{ScriptName}] {nameof(RotationIntentRouter)} initialized for {context.Agent.name}");
        }

        public void SetCurrentType(RotateToTargetNodeType type)
        {
            if (_currentExecutorType == type) return;

            // Stop the previous executor if switching types
            if (_currentExecutor != null)
            {
                _currentExecutor.CancelRotation();
                Debug.Log($"[{ScriptName}] Cancelled previous rotation executor: {_currentExecutor.Type}");
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
        /// Only allows the active owner to command rotation.
        /// </summary>
        public bool TryRotateTo(Vector3 target, RotationData data, int executorId)
        {
            if (_activeExecutorId != executorId)
            {
                Debug.LogError(
                    $"[{ScriptName}] ❌ Rotate intent from unauthorized owner. Ignoring. " +
                    $"ExecutorId={executorId}, Active={_activeExecutorId}. " +
                    $"This means the current BT session/context does not own rotation. " +
                    $"(Did you forget to call TakeOwnership() on tree switch? Is context.BtSessionId being updated?)"
                );
                return false;
            }
            
            SetCurrentType(data.RotationType);
            _currentExecutor.ApplySettings(data);
            
            if (_currentExecutor == null)
            {
                Debug.LogError($"[{ScriptName}] Executor NOT FOUND for {data.RotationType}");
                return false;
            }

            // --- Only act if intent changes ---
            if (!IsCurrentRotation(target, data))
            {
                Debug.Log($"[{ScriptName}] New rotation intent. Cancelling previous and rotating to {target} ({data.RotationType})");
                _currentExecutor.CancelRotation();
                var result = _currentExecutor.TryRotateTo(target);
                _currentExecutor.StartRotation();

                _lastTarget = target;
                _lastRotationData = data;
                _hasLastRotation = true;

                return result;
            }

            // Already rotating to this target with these params
            return true;
        }
        
        public bool IsCurrentRotation(Vector3 target, RotationData data)
        {
            return _currentExecutor?.IsCurrentRotation(target, data) ?? false;
        }
        
        public void TakeOwnership(int newOwnerId)
        {
            if (_activeExecutorId == newOwnerId) return;

            _currentExecutor.CancelRotation();
            _activeExecutorId = newOwnerId;
            Debug.Log($"[{ScriptName}] Rotation intent owner switched to {newOwnerId}");
        }

        public void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement/Rotation domain blocked, stopping executor.");
            _currentExecutor.PauseRotation();
        }

        public void OnDomainUnblocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Movement, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement/Rotation domain blocked, stopping executor.");
            _currentExecutor.StartRotation();
        }

        public void Dispose()
        {
            if(_statusEffectManager == null) return;
            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }
        
        public void CancelRotation() => _currentExecutor.CancelRotation();
        public void PauseRotation() => _currentExecutor.PauseRotation();
        public void StartRotation() => _currentExecutor.StartRotation();
        public bool IsFacingTarget(Vector3 target) => _currentExecutor.IsFacingTarget(target);
        public int GetActiveOwnerId() => _activeExecutorId;
        
        public void ForceCancelAndReleaseOwnership()
        {
            CancelRotation();
            _activeExecutorId = -1;
        }
    }
}