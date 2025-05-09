// TODO [CleanUp]: Replace Vector3 with portable Position struct for engine decoupling
// When extracting Core to a shared library or testing context, replace Vector3 with Position struct
// and migrate all movement calls accordingly.
using UnityEngine;

public interface IMovementNode
{
    bool TryMoveTo(Vector3 destination);
    void Stop();
    bool IsAtDestination();
}