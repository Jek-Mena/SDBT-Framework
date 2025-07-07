// TODO [CleanUp]: Replace Vector3 with portable Position struct for engine decoupling
// When extracting Core to a shared library or testing context, replace Vector3 with Position struct
// and migrate all movement calls accordingly.

using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public interface IMovementNode
    {
        void Initialize(MovementData data);
        void ApplySettings(MovementData data);
        bool TryMoveTo(Vector3 destination);
        bool IsAtDestination();
    }
}