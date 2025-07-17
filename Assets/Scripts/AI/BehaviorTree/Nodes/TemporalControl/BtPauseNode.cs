using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.TemporalControl.Base;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    public class BtPauseNode : TimedExecutionNode
    {
        private StatusEffect _pauseEffect;
        private bool _applied;
        private readonly string[] _domains;
        public override string DisplayName => string.IsNullOrEmpty(Label) ? $"{BtNodeDisplayName.TimedExecution.Pause}" : $"{BtNodeDisplayName.TimedExecution.Pause} ({Label})";

        public BtPauseNode(TimedExecutionData timeData, string[] domains = null) : base(timeData)
        {
            _domains = domains ?? new[] { BlockedDomain.Movement };
        }

        public override BtStatus Tick(BtContext context)
        {
            if (!BtValidator.Require(context)
                    .TimeExecutionManager()
                    .Effects()
                    .Check(out var error)
               )
            {
                Debug.Log(error);
                _lastStatus = BtStatus.Failure;
                return _lastStatus;
            }

            EnsureTimerStarted();

            if (!_applied)
            {
                // Apply pause effect via orchestrator for movement block
                _pauseEffect = new StatusEffect
                {
                    Source = nameof(BtPauseNode),
                    Duration = TimeData.Duration, // Use timer config
                    TimeApplied = Time.time,
                    Domains = _domains
                };
                _pauseEffect.SetCustomName(BtNodeDisplayName.TimedExecution.Pause);
                
                context.Blackboard.StatusEffectManager.ApplyEffect(_pauseEffect);
                _applied = true;
            }

            var status = CheckTimerStatus();

            if (status != BtStatus.Running && _applied)
            {
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect);
                _applied = false;
            }

            _lastStatus = status;
            return _lastStatus;
        }
        
        public override void Reset(BtContext context)
        {
            // Remove pause effect if it's still active
            if (_applied && _pauseEffect != null)
            {
                // Defensive: If context needed, ensure you can access it, or pass in if required
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect); // (if you have context)
                _applied = false;
                _pauseEffect = null;
                Debug.Log($"[BtPauseNode] Removing pause effect for {Label} on reset/exit.");
            }
            base.Reset(context);  // Resets _lastStatus and TimerStarted
        }
        
        public override void OnExitNode(BtContext context)
        {
            // Remove pause effect if still active
            if (_applied && _pauseEffect != null)
            {
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect);
                _applied = false;
                _pauseEffect = null;
                Debug.Log($"[BtPauseNode] Removing pause effect for {Label} on exit.");
            }
            // Also interrupt the timer
            Timer?.Interrupt(Label);
            TimerStarted = false;
            _lastStatus = BtStatus.Idle;
        }
    }
}