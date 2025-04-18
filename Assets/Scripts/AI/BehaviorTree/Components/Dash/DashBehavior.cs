using UnityEngine;

public class DashBehavior : IBehaviorNode
{
    private readonly IDashBehavior _dash;
    private readonly Blackboard _blackboard;
    private readonly Transform _target;
    private readonly Transform _targetLastPosition;

    public DashBehavior(Blackboard bb)
    {
        _dash = bb.Dash;
        _target = bb.Target;
        _blackboard = bb;
    }

    public BtStatus Tick(BtController controller)
    {
        if (_dash == null || _target == null)
            return BtStatus.Failure;

        if (_dash.IsDashing())
            return _dash.IsDashComplete() ? BtStatus.Success : BtStatus.Running;

        return _dash.TryDashTo(_target.position) ? BtStatus.Running : BtStatus.Failure;
    }
}