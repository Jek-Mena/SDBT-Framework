using System.Collections.Generic;

public interface ICompositeNode : IBehaviorNode
{
    void SetChildren(List<IBehaviorNode> children);
}