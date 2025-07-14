// TODO [CleanUp]: Replace Vector3 with portable Position struct for engine decoupling
// When extracting Core to a shared library or testing context, replace Vector3 with Position struct
// and migrate all movement calls accordingly.

using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    [System.Obsolete("Use IMovementExecutor instead. FleeIntentExecutor is not yet properly implemented and will be refactored to use IMovementExecutor.")]
    public interface IMovementNode
    {
        void Initialize(MovementData data);
        void ApplySettings(MovementData data);
        bool TryMoveTo(Vector3 destination);
        bool IsAtDestination();
    }
}