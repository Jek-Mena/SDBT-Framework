using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public interface IRotationExecutor
    {
        RotateToTargetNodeType Type { get; }
        void ApplySettings(RotationData data);
        bool TryRotateTo(Vector3 targetPosition);
        void StartRotation();
        void PauseRotation();
        void CancelRotation();
        bool IsFacingTarget(Vector3 targetPosition);
        bool IsCurrentRotation(Vector3 targetPosition, RotationData data);
    }
}