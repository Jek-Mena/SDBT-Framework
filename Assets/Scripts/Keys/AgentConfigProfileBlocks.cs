/// <summary>
/// [ARCHITECTURE NOTE - 2025-07-04]
/// Profile block keys are always PLURAL and refer to a DICTIONARY of all available profiles for that type.
/// - Used as JSON config block keys, blackboard property names, and dictionary keys.
/// - Example: CoreKeys.Profiles.Movement = "movementProfiles"
/// - Usage: config["profiles"]["movementProfiles"], blackboard.MovementProfiles, etc.
/// - Never use the plural form for single profile selection in node configs or lookups.
/// </summary>
public static class AgentConfigProfileBlocks
{
    public const string Switches = "SwitchProfiles";
    public const string Health = "HealthProfiles";
    public const string Targeting = "TargetingProfiles";
    public const string Movement = "MovementProfiles";
    public const string Rotation = "RotationProfiles";
    public const string Timing = "TimingProfiles";
    public const string FearPerception = "FearProfiles";
}

public static class AgentProfileFields
{
    public const string CurrentFearProfile = "agentCurrentFearProfile";
}

public static class AgentDefaultProfileValues
{
    public const string Fear = "DefaultFear";
}