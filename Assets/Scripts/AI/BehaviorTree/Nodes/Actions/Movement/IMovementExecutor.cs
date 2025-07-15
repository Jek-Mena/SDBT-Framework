using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public interface IMovementExecutor
    {
        MoveToTargetNodeType Type { get; }
        bool AcceptMoveIntent(Vector3 destination, MovementData data);
        void ApplySettings(MovementData data);
        void StartMovement();
        void PauseMovement();
        void CancelMovement();
        bool IsAtDestination();
        bool IsCurrentMove(Vector3 destination, MovementData data);
    }
}