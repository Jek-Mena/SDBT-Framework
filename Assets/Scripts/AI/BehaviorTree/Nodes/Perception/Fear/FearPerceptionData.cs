public class FearPerceptionData
{
    public float DetectionRange;
    public float DecayDuration;
    public float MaxDuration;
    public float Threshold;
    /// <summary>
    /// How far the agent tries to move away from the threat (in world units). Higher = more cowardly.
    /// </summary>
    public float FleeDistance;
    public string CurveType; // TODO String or AnimationCurve (AC) but how does AC translate to JSON?
}
