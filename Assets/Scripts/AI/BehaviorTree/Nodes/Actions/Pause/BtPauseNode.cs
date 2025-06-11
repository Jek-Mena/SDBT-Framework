using UnityEngine;

public class BtPauseNode : TimedExecutionNode
{
    private StatusEffectManager _effectManager;
    private StatusEffect _pauseEffect;
    private bool _applied;
    private readonly string[] _domains;

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
            return BtStatus.Failure;
        }
        
        EnsureTimerStarted();
        
        if (!_applied)
        {
            // Apply pause effect via orchestrator for movement block
            _pauseEffect = new StatusEffect()
            {
                Source = StatusEffectSourceKeys.TimedExecution.Pause,
                Duration = TimeData.Duration, // Use timer config
                TimeApplied = Time.time,
                Domains = _domains
            };
            _effectManager.ApplyEffect(_pauseEffect);
            _applied = true;
        }

        var status = CheckTimerStatus();

        if (status != BtStatus.Running && _applied)
        {
            _effectManager.RemoveEffects(_pauseEffect);
            _applied = false;
        }

        return status;
    }
}