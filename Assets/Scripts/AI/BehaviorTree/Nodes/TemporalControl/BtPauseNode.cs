using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

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
                .Timers()
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
                Source = StatusEffectSourceKeys.TimedExecution.Pause,
                Duration = TimeData.Duration, // Use timer config
                TimeApplied = Time.time,
                Domains = _domains
            };
            _pauseEffect.SetCustomName(BtNodeDisplayName.TimedExecution.Pause);

            context.StatusEffectManager.ApplyEffect(_pauseEffect);
            _applied = true;
        }

        var status = CheckTimerStatus();

        if (status != BtStatus.Running && _applied)
        {
            context.StatusEffectManager.RemoveEffects(_pauseEffect);
            _applied = false;
        }

        _lastStatus = status;
        return _lastStatus;
    }
}