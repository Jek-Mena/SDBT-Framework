using System.Collections.Generic;
using UnityEngine;

public class BtSequenceNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;
    private int _currentIndex;

    public BtSequenceNode(List<IBehaviorNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public BtStatus Tick(BtContext context)
    {
        while (_currentIndex < _children.Count)
        {
            Debug.Log($"[BT Sequence] Ticking child {_currentIndex}");
            var status = _children[_currentIndex].Tick(context);

            switch (status)
            {
                case BtStatus.Running:
                    return BtStatus.Running;
                case BtStatus.Failure:
                    _currentIndex = 0;
                    return BtStatus.Failure;
                case BtStatus.Success:
                default:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        return BtStatus.Success;
    }
}