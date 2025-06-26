using System;
using System.Collections.Generic;

// NOTE: This base class is NOT currently used in the live tree. 
// Upgraded for future extensibility and debug overlay support.

public abstract class BtConditionNode : IBehaviorNode
{
    protected IBehaviorNode _child;
    protected BtStatus _lastStatus = BtStatus.Idle;

    public void SetChild(IBehaviorNode child)
    {
        _child = child;
    }

    public BtStatus LastStatus => _lastStatus;

    public virtual string DisplayName => GetType().Name;

    public IEnumerable<IBehaviorNode> GetChildren => _child != null ? new[] { _child } : Array.Empty<IBehaviorNode>();

    public BtStatus Tick(BtController controller)
    {
        throw new NotImplementedException();
        //var context = new BtContext(controller);
        //return Tick(context);
    }

    // REMEMBER: All inheriting nodes must set _lastStatus before every return!
    public abstract BtStatus Tick(BtContext context);
}