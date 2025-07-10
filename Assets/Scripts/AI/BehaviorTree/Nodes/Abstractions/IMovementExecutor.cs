using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Abstractions
{
    public interface IMovementExecutor
    {
        MovementNodeType Type { get; }
        bool TryMoveTo(Vector3 destination);
        void ApplySettings(MovementData data);
        void StartMovement();
        void PauseMovement();
        void CancelMovement();
        bool IsAtDestination();
    }
}