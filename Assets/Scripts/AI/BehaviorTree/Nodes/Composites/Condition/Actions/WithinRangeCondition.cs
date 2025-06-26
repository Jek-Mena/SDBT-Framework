using UnityEngine;

// NOTE: This node is not currently in use. 
// Upgraded for consistency and overlay/debugging.

public class WithinRangeCondition : BtConditionNode
{
    private readonly string _targetKey;
    private readonly float _maxDistance;

    public WithinRangeCondition(string targetKey, float maxDistance)
    {
        _targetKey = targetKey;
        _maxDistance = maxDistance;
    }

    public override BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .Blackboard()
                .KeyExists<GameObject>(_targetKey)
                .Check(out var error))
        {
            Debug.LogError(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        var target = context.Blackboard.Get<GameObject>(_targetKey);
        var distance = Vector3.Distance(context.Agent.transform.position, target.transform.position);
        var inRange = distance <= _maxDistance;

        if (!inRange)
        {
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        // Tick child if present, otherwise return Success (condition met)
        _lastStatus = _child?.Tick(context) ?? BtStatus.Success;
        return _lastStatus;
    }
}