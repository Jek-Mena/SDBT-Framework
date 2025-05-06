using UnityEngine;

public interface IMovementNode
{
    bool TryMoveTo(Vector3 destination);
    void Stop();
    bool IsAtDestination();
}