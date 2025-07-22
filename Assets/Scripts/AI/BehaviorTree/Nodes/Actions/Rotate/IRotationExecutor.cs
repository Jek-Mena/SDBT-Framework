using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public interface IRotationExecutor
    {
        RotateToTargetNodeType Type { get; }
        void Tick(float deltaTime);
        void ApplySettings(RotationData data);
        bool AcceptRotateIntent(Transform targetTransform, RotationData data);
        void StartRotation();
        void PauseRotation();
        void CancelRotation();
        bool IsFacingTarget();
    }
}