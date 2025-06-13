using UnityEngine;

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
            return BtStatus.Failure;
        }
        
        var target =context.Blackboard.Get<GameObject>(_targetKey);
        var distance = Vector3.Distance(context.Agent.transform.position, target.transform.position);
        var inRange = distance <= _maxDistance;

        return !inRange
            ? BtStatus.Failure
            : _child?.Tick(context) ?? BtStatus.Success;
    }
}