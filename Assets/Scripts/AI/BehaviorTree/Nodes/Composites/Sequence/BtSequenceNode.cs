using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class BtSequenceNode : IBehaviorNode
{
    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Composite.Sequence;

    private readonly List<IBehaviorNode> _children;
    public IEnumerable<IBehaviorNode> GetChildren => _children;
    
    private int _currentIndex;
    private const string ScriptName = nameof(BtSequenceNode);

    public BtSequenceNode(List<IBehaviorNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public void Reset(BtContext context)
    {
        foreach (var child in _children)
            child.Reset(context);
        _currentIndex = 0;
        _lastStatus = BtStatus.Idle;
    }
    
    public BtStatus Tick(BtContext context)
    {
        if(!BtValidator.Require(context)
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
                case BtStatus.Running:
                    _lastStatus = BtStatus.Running;
                    return _lastStatus;
                case BtStatus.Failure:
                    _lastStatus = BtStatus.Failure;
                    _currentIndex = 0;
                    return _lastStatus;
                case BtStatus.Success:
                default:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        _lastStatus = BtStatus.Success;
        return _lastStatus;
    }
}