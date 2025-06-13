using System.Collections.Generic;
using UnityEngine;

public class BtSelectorNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;
    public BtSelectorNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtContext context)
    {
        if(!BtValidator.Require(context)
               .Children(_children)
               .Check(out var error)
           )
        {
            Debug.Log(error);
            return BtStatus.Failure;
        }
        
        foreach (var child in _children)
        {
            var status = child.Tick(context);
            if (status == BtStatus.Success)
                return BtStatus.Success;
            if (status == BtStatus.Running)
                return BtStatus.Running;
        }

        return BtStatus.Failure;
    }
}