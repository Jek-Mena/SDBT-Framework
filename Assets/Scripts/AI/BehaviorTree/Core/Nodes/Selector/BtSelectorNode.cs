using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class BtSelectorNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;
    public BtSelectorNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtController controller)
    {
        foreach (var child in _children)
        {
            var status = child.Tick(controller);
            if (status == BtStatus.Success)
                return BtStatus.Success;
            if (status == BtStatus.Running)
                return BtStatus.Running;
        }

        return BtStatus.Failure;
    }
}

[BtNode(BtNodeName.ControlFlow.Selector)]
public class SelectorFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard, Func<JToken, IBehaviorNode> recurse)
    {
        var childrenArray = config[JsonFields.Children] as JArray;
        if (childrenArray == null || childrenArray.Count == 0)
            throw new System.Exception($"Selector node requires a {JsonFields.Children} array.");

        var children = childrenArray
            .Select(recurse)
            .ToList();

        return new BtSelectorNode(children);
    }
}