using UnityEngine;

[System.Serializable]
public class FleeIntentData : IBehaviorIntentData
{
    public enum FleeMode
    {
        FromSource,
        ToSafePoint,
        FromPresence,
        ToGroup,
        RandomScatter,
    }

    public FleeMode Mode;
    public Vector3? SourcePosition;
    public GameObject ThreatEntity;
    public Vector3? TargetPoint;
    public float FearStrength;
    public AnimationCurve ModulationCurve; // Optional, for future agent personalities

    public FleeIntentData(
        FleeMode mode,
        Vector3? sourcePosition = null,
        GameObject threatEntity = null,
        Vector3? targetPoint = null,
        float fearStrength = 0f,
        AnimationCurve modulationCurve = null
    )
    {
        Mode = mode;
        SourcePosition = sourcePosition;
        ThreatEntity = threatEntity;
        TargetPoint = targetPoint;
        FearStrength = fearStrength;
        ModulationCurve = modulationCurve;
    }
}