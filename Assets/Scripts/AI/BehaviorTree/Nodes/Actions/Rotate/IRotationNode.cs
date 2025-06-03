using UnityEngine;

public interface IRotationNode
{
    void ApplySettings(RotationData data);
    bool TryRotateTo(Vector3 targetDesination);
    void StopRotation();
    void StartRotation();
    bool IsFacingTarget(Vector3 targetPosition);
}