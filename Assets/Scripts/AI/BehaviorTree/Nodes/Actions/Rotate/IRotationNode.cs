using UnityEngine;

namespace AI.BehaviorTree.Nodes.Actions.Rotate
{
    public interface IRotationNode
    {
        void Initialize(RotationData data);
        void ApplySettings(RotationData data);
        bool TryRotateTo(Vector3 targetDesination);
        bool IsFacingTarget(Vector3 targetPosition);
    }
}