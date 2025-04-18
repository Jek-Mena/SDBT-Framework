using UnityEngine;

public interface IContextBuilder
{
    Blackboard Build(GameObject entity);
}