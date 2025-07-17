using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public interface IRotationExecutor
    {
        RotateToTargetNodeType Type { get; }
        void ApplySettings(RotationData data);
        bool AcceptRotateIntent(Transform targetTransform, RotationData data);
        void StartRotation();
        void PauseRotation();
        void CancelRotation();
        bool IsFacingTarget(Transform target);
        bool IsCurrentRotation(Transform target, RotationData data);
        RotationData GetSettings();
    }
}