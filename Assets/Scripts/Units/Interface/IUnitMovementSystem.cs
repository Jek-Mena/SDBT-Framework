
using UnityEngine;

public interface IUnitMovementSystem
{
    void MoveTo(Vector3 destination);
    void Stop();
    bool IsAtDestination();
}