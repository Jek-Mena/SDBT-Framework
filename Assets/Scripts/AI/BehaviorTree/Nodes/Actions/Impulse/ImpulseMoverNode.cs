// NOTE: ImpulseMoverNode is NOT currently in use and properly implemented.
// Upgraded for debug overlay/traversal consistency.

using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;

public class ImpulseMoverNode : IBehaviorNode
{
    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Movement.ImpulseMover;
    public void Reset(BtContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnExitNode(BtContext context)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<IBehaviorNode> GetChildren => System.Array.Empty<IBehaviorNode>();

    public void Initialize(BtContext context)
    {
        throw new System.NotImplementedException();
    }

    public BtStatus Tick(BtContext context)
    {
        var impulseLogic = context.Services.ImpulseLogic;

        if (impulseLogic == null)
        {
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        if (impulseLogic.IsImpulsing())
        {
            _lastStatus = impulseLogic.IsImpulseComplete() ? BtStatus.Success : BtStatus.Running;
            return _lastStatus;
        }

        _lastStatus = impulseLogic.TryImpulse() ? BtStatus.Running : BtStatus.Failure;
        return _lastStatus;
    }
}