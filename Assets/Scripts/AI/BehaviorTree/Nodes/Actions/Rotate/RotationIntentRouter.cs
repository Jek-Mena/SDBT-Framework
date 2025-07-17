using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Systems.StatusEffectSystem.Component;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public class RotationIntentRouter : IUsesStatusEffectManager, ISystemCleanable
    {
        private const string ScriptName = nameof(RotationIntentRouter);
        private readonly Dictionary<RotateToTargetNodeType, IRotationExecutor> _executors;
        private readonly StatusEffectManager _statusEffectManager;
        private IRotationExecutor _currentExecutor;
        private RotateToTargetNodeType _currentExecutorType;

        private string _activeExecutorId; // GUID session
        private string _lastOwnerId; // For debug overlay

        public RotationIntentRouter(BtContext context)
        {
            _executors = new Dictionary<RotateToTargetNodeType, IRotationExecutor>
            {
                { RotateToTargetNodeType.QuaternionLookAt, new QuaternionLookAtTarget(context.Agent.transform) }
                // Add more executors as needed here
            };

            _currentExecutorType = RotateToTargetNodeType.QuaternionLookAt;
            _currentExecutor = _executors[_currentExecutorType];

            Dispose();
            _statusEffectManager = context.Blackboard.StatusEffectManager;
            ;
            _statusEffectManager.DomainBlocked += OnDomainBlocked;
            _statusEffectManager.DomainUnblocked += OnDomainUnblocked;

            Debug.Log($"[{ScriptName}] {nameof(RotationIntentRouter)} initialized for {context.Agent.name}");
        }

        public void TakeOwnership(string newOwnerId)
        {
            Debug.Log($"[Domain][CLAIM] Rotation claimed by Session={newOwnerId} (was={_activeExecutorId})");
            if (_activeExecutorId != null && _activeExecutorId != newOwnerId)
                Debug.LogWarning(
                    $"[Domain][CLAIM][WARN] Rotation was owned by {_activeExecutorId}, now claiming for {newOwnerId}.");
            _currentExecutor.CancelRotation();
            _lastOwnerId = _activeExecutorId;
            _activeExecutorId = newOwnerId;
        }

        public void ReleaseOwnership(string sessionId)
        {
            Debug.Log($"[Domain][RELEASE] Rotation released by Session={sessionId} (current={_activeExecutorId})");
            if (_activeExecutorId == sessionId)
                _activeExecutorId = null;
            else
                Debug.LogWarning(
                    $"[Domain][RELEASE][WARN] Session={sessionId} tried to release, but owner is {_activeExecutorId}.");
        }

        public string GetActiveOwnerId() => _activeExecutorId;
        public string GetLastOwnerId() => _lastOwnerId;
        
        [System.Obsolete]
        public void ForceCancelAndReleaseOwnership()
        {
            CancelRotation();
            _lastOwnerId = _activeExecutorId;
            _activeExecutorId = null;
        }
        
        public void ReleaseSystem(BtContext context)
        {
            Debug.Log($"[{ScriptName}] CleanupSystem called.");
            _currentExecutor?.CancelRotation();
            _lastOwnerId = _activeExecutorId;
            _activeExecutorId = null; // Reset executor ID so no orphan BT can claim it
            Dispose(); // Unsubscribe from status manager

            // "Full Nuke": Clear all  executors 
            foreach (var executor in _executors.Values)
                executor.CancelRotation();

            Debug.Log($"[{ScriptName}] Cleanup complete.");
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
                Debug.LogError($"[{ScriptName}] Failed to switched executor. Executor not found for {type}");
                throw new Exception($"[{ScriptName}] Failed to switched executor. Executor not found for {type}");
            }
        }

        /// <summary>
        /// Only allows the active owner to command rotation.
        /// </summary>
        public bool TryIssueRotateIntent(Transform target, RotationData data, string executorId)
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

            // --- Only act if intent changes ---
            if (IsCurrentRotation(target, data)) return true;

            Debug.Log(
                $"[{ScriptName}] New rotation intent. Cancelling previous and rotating to {target} ({data.RotationType})");
            _currentExecutor.CancelRotation();
            _currentExecutor.ApplySettings(data);
            _currentExecutor.StartRotation();

            // Only return true if the executor successfully accepted the move intent.
            // If false, something is broken (unreachable, agent gone, etc) and the BT node will fail.
            return _currentExecutor.AcceptRotateIntent(target, data);
            // TODO see task below [07-16-2025]

            // Already rotating to this target with these params
        }

        public bool IsCurrentRotation(Transform target, RotationData data)
        {
            return _currentExecutor?.IsCurrentRotation(target, data) ?? false;
        }

        public void Tick(float deltaTime)
        {
            if (_currentExecutor is ITickableExecutor executor)
                executor.Tick(deltaTime);
        }

        public void OnDomainBlocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Rotation, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement/Rotation domain blocked, stopping executor.");
            _currentExecutor.PauseRotation();
        }

        public void OnDomainUnblocked(string domain)
        {
            if (!string.Equals(domain, DomainKeys.Rotation, StringComparison.OrdinalIgnoreCase)) return;
            Debug.Log($"[{ScriptName}] Movement/Rotation domain blocked, stopping executor.");
            _currentExecutor.StartRotation();
        }

        public void Dispose()
        {
            if (_statusEffectManager == null) return;
            _statusEffectManager.DomainBlocked -= OnDomainBlocked;
            _statusEffectManager.DomainUnblocked -= OnDomainUnblocked;
        }

        public void CancelRotation() => _currentExecutor.CancelRotation();
        public void PauseRotation() => _currentExecutor.PauseRotation();
        public void StartRotation() => _currentExecutor.StartRotation();
        public bool IsFacingTarget(Transform target) => _currentExecutor.IsFacingTarget(target);
        public RotationData GetCurrentSettings() => _currentExecutor.GetSettings();
    }
}
/*
# TODO: Upgrade Move/Rotate Intent API to Return Rich Result Object

**Goal:**  
Replace simple `bool` return values from movement and rotation intent routers (and executors) with a detailed result object, to provide richer error reporting and better system observability.

        ---

## Why?

    - **Clarity:** Distinguish *why* an intent failed (e.g., unreachable destination, missing agent, unauthorized call).
- **Debugging:** Expose precise failure reasons for overlays, logs, and QA.
- **Fallbacks:** Enable smarter AI behavior on specific failures (e.g., retry, fallback, skip turn).
- **Telemetry:** Collect actionable analytics on movement/rotation issues.

        ---

## Benefits

    - Fewer “silent failures” in agents and behavior trees.
- Easier to diagnose and fix edge cases (especially pathfinding or agent lifetime bugs).
- Better support for designers and testers via debug overlays.

        ---

## Migration Plan

    - [ ] Define a `MoveIntentResult` / `RotateIntentResult` class with fields:
- `Success` (bool)
    - `ErrorCode` (enum: e.g., AgentMissing, Unreachable, Unauthorized, etc.)
- `ErrorMessage` (string)
    - [ ] Update all relevant routers and executors to return the result object.
- [ ] Update BT nodes (`MoveToTargetNode`, `RotateToTargetNode`, etc.) to check result and react/log accordingly.
- [ ] Update any debug overlays, analytics, or logs to display failure details.
- [ ] Review/document common error scenarios for future maintainers.

        ---

## Sample API (for discussion)

    ```csharp
public class MoveIntentResult
{
    public bool Success;
    public MoveIntentErrorCode ErrorCode;
    public string ErrorMessage;
}
*/