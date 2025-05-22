public class TargetingData
{
    public string TargetTag = "Player";       // Tag to search for
    public TargetingStyle Style = TargetingStyle.Closest; // Enum: Closest, LowestHP, HighestHP, Random, etc.
    public float MaxRange = 100f;             // Max range for targeting (optional)
    public bool AllowNull = false;            // Should fallback to null be allowed?
    // ...add more targeting rules as you expand (e.g., onlyAlive, priorityList, etc.)
}