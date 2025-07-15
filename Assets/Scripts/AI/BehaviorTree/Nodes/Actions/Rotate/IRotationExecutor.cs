using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public interface IRotationExecutor
    {
        RotateToTargetNodeType Type { get; }
        void ApplySettings(RotationData data);
        bool AcceptRotateIntent(Vector3 targetPosition, RotationData data);
        void StartRotation();
        void PauseRotation();
        void CancelRotation();
        bool IsFacingTarget(Vector3 targetPosition);
        bool IsCurrentRotation(Vector3 targetPosition, RotationData data);
    }
}