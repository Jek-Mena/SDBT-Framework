/// <summary>
/// Keys used for behavior tree node-level (per-node) profile selection.
/// These are the JSON field names in BT node configs (usually lowerCamelCase).
///
/// These keys are ONLY used inside BT node "config" blocks, NOT in agent config/global selectors.
/// </summary>
public static class NodeProfileSelectorKeys
{
    // ---- MOVEMENT ----
    public const string MovementProfile = "movementProfile";

    // ---- ROTATION ----
    public const string RotationProfile = "rotationProfile";

    // ---- TIMING ----
    public const string TimingProfile = "timingProfile";

    // ---- TARGETING ----
    public const string TargetProfile = "targetProfile";

    // ---- FEAR (uncommon, but leave for extensibility) ----
    public const string FearProfile = "fearProfile";

    // ---- SWITCHES (to support per-node switching config) ----
    public const string SwitchProfile = "switchProfile";        
}