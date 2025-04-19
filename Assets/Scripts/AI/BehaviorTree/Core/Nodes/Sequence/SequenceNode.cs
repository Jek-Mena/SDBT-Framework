using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class SequenceNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;
    private int _currentIndex;

    public SequenceNode(List<IBehaviorNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public BtStatus Tick(BtController controller)
    {
        while (_currentIndex < _children.Count)
        {
            var status = _children[_currentIndex].Tick(controller);

            if (status == BtStatus.Running)
                return BtStatus.Running;

            if (status == BtStatus.Failure)
            {
                _currentIndex = 0;
                return BtStatus.Failure;
            }

            _currentIndex++;
        }

        _currentIndex = 0;
        return BtStatus.Success;
    }
}


[Plugin(PluginKey.BtSequenceNode)]
public class SequenceNodeFactory : IBehaviorNodeFactory
{
    public PluginKey Type => PluginKey.BtTree_BasicChase; // Change if you want it distinct

    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard)
    {
        if (config[JsonKeys.BehaviorTree.Children] is not JArray childrenArray)
            throw new System.Exception($"SequenceNode requires {JsonKeys.BehaviorTree.Children} array.");

        var children = new List<IBehaviorNode>();
        foreach (var childToken in childrenArray)
        {
            children.Add(BtFactory.BuildTree(childToken, blackboard));
        }

        return new SequenceNode(children);
    }
}
