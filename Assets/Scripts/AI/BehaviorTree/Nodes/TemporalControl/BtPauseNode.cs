using System;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;
using Systems.StatusEffectSystem;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    public class BtPauseNode : IBehaviorNode
    {
        private readonly TimedExecutionComponent _timed;
        private readonly string[] _domains;
        private bool _applied;
        private StatusEffect _pauseEffect;
        
        public string DisplayName => string.IsNullOrEmpty(_timed.Data.Label) ? $"{BtNodeDisplayName.TimedExecution.Pause}" : $"{BtNodeDisplayName.TimedExecution.Pause} ({_timed.Data.Label})";
        public BtStatus LastStatus { get; private set; } = BtStatus.Idle;

        public BtPauseNode(TimedExecutionData timeData, string[] domains = null)
        {
            _timed = new TimedExecutionComponent(timeData);
            _domains = domains ?? new[] { BlockedDomain.Movement };
        }
        
        public void Initialize(BtContext context)
        {
            _timed.Initialize(context);
            _applied = false;
            _pauseEffect = null;
        }

        public BtStatus Tick(BtContext context)
        {
            if (!_applied)
            {
                _pauseEffect = new StatusEffect
                {
                    Domains = _domains,
                    Duration = _timed.Data.Duration,
                    Source = nameof(BtPauseNode),
                    TimeApplied = Time.time,
                };
                _pauseEffect.SetCustomName(BtNodeDisplayName.TimedExecution.Pause);
                
                context.Blackboard.StatusEffectManager.ApplyEffect(_pauseEffect);
                _applied = true;
                Debug.Log($"Creating pause effect for domains: {string.Join(",", _domains ?? Array.Empty<string>())}");
            }
            
            _timed.StartTimerIfNeeded();
            var timerStatus = _timed.GetTimerStatus();
            
            if ((timerStatus == BtStatus.Success || timerStatus == BtStatus.Failure) && _applied && _pauseEffect != null)
            {
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect);
                _applied = false;
                _pauseEffect = null;
            }
            
            LastStatus = timerStatus;
            return timerStatus;
        }

        public void Reset(BtContext context)
        {
            if (_applied && _pauseEffect != null)
            {
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect);
                _applied = false;
                _pauseEffect = null;
            }
            _timed.InterruptTimer();
            LastStatus = BtStatus.Idle;
        }

        public void OnExitNode(BtContext context)
        {
            if (_applied && _pauseEffect != null)
            {
                context.Blackboard.StatusEffectManager.RemoveEffects(_pauseEffect);
                _applied = false;
                _pauseEffect = null;
            }
            _timed.InterruptTimer();
            LastStatus = BtStatus.Idle;
        }
        
        public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();
    }
}