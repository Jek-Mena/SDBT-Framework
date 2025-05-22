using UnityEngine;

public interface ITargetResolver
{
    Transform ResolveTarget(GameObject self, TargetingData data);

}