using System.Collections.Generic;
using UnityEngine;

public class BtSelectorNode : IBehaviorNode
{
    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Composite.Selector;

    private readonly List<IBehaviorNode> _children;
    public IEnumerable<IBehaviorNode> GetChildren => _children;

    private int _currentIndex;
    private const string ScriptName = nameof(BtSelectorNode);
    
    public BtSelectorNode(List<IBehaviorNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public BtStatus Tick(BtContext context)
    {
        if (!BtValidator.Require(context)
                .Children(_children)
                .Check(out var error)
           )
        {
            Debug.Log(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        while (_currentIndex < _children.Count)
        {
            Debug.Log($"[{ScriptName}] Ticking child {_currentIndex}");
            var status = _children[_currentIndex].Tick(context);

            switch (status)
            {
                case BtStatus.Success:
                    _lastStatus = BtStatus.Success;
                    _currentIndex = 0; // Reset for next evaluation
                    return _lastStatus;
                case BtStatus.Running:
                    _lastStatus = BtStatus.Running;
                    return _lastStatus;
                case BtStatus.Failure:
                default:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        _lastStatus = BtStatus.Failure;
        return _lastStatus;
    }
}