using System.Collections.Generic;

/// <summary>
/// A decorator node that wraps any IBehaviorNode and calls OnExit() when it transitions
/// from Running to Success or Failure. Used to handle timer cleanup, state resets, etc.
/// Now supports overlay/debug with LastStatus, NodeName, GetChildren.
/// </summary>
public class BtLifecycleNode : IBehaviorNode
{
    private readonly IBehaviorNode _inner;
    private readonly IExitableBehavior _exitable;
    private BtStatus _lastStatus = BtStatus.Running;

    public BtLifecycleNode(IBehaviorNode inner)
    {
        _inner = inner;
        _exitable = inner as IExitableBehavior;
    }

    public BtStatus LastStatus => _lastStatus;
    public string NodeName => nameof(BtLifecycleNode);
    public IEnumerable<IBehaviorNode> GetChildren => new[] { _inner };

    public BtStatus Tick(BtContext context)
    {
        var status = _inner.Tick(context);

        if (status != BtStatus.Running && _lastStatus == BtStatus.Running)
        {
            _exitable?.OnExit();
        }

        _lastStatus = status;
        return _lastStatus;
    }
}