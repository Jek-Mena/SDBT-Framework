using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class BtRepeaterNode : IBehaviorNode
{
    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Decorators.Repeater;
    public IEnumerable<IBehaviorNode> GetChildren => new[] { _child };

    private readonly IBehaviorNode _child;
    private readonly int _maxRepeats;
    private int _repeatCount;

    public BtRepeaterNode(IBehaviorNode child, int maxRepeats = -1)
    {
        _child = child;
        _maxRepeats = maxRepeats;
        _repeatCount = 0;
    }

    public BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .RequireChild(_child)
                .Check(out var error))
        {
            Debug.LogError(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        var status = _child.Tick(context);

        if (status == BtStatus.Success || status == BtStatus.Failure)
        {
            _repeatCount++;
            if (_maxRepeats > 0 && _repeatCount >= _maxRepeats)
            {
                _repeatCount = 0; // Reset to allow reuse
                _lastStatus = BtStatus.Success;
                return _lastStatus;
            }
        }

        _lastStatus = BtStatus.Running;
        return _lastStatus;
    }
}