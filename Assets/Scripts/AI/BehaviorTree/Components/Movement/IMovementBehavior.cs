using UnityEngine;

public interface IMovementBehavior
{
    bool TryMoveTo(Vector3 destination);
    void Stop();
    bool IsAtDestination();
}