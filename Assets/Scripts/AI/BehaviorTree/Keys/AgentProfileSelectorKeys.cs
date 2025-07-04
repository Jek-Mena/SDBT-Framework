/// <summary>
/// Keys used for agent-level (global) profile selection.
/// 
/// - BLOCK (Plural): Dictionary of all profiles of a type for this agent.
/// - SELECTOR (Singular): Field that selects the active profile globally for the agent.
/// - DEFAULT: Fallback profile name (used for new agents, error recovery, etc).
///
/// These keys are used in agent JSON/configs and during runtime profile injection.
/// </summary>
public static class AgentProfileSelectorKeys
{
    // FEAR
    public const string FearProfiles = "FearProfiles";                       // BLOCK (dictionary of all fear profiles)
    public const string AgentCurrentFearProfile = "agentCurrentFearProfile"; // SELECTOR (which fear profile is active for this agent)
    public const string DefaultFearProfile = "DefaultFear";                  // DEFAULT VALUE
    
    // MOVEMENT
    public const string MovementProfiles = "MovementProfiles";                       // BLOCK
    public const string AgentCurrentMovementProfile = "agentCurrentMovementProfile"; // SELECTOR
    public const string DefaultMovementProfile = "DefaultMovement";                  // DEFAULT

    // TARGETING
    public const string TargetingProfiles = "TargetingProfiles";
    public const string AgentCurrentTargetingProfile = "agentCurrentTargetingProfile";
    public const string DefaultTargetingProfile = "DefaultTargeting";

    // ROTATION
    public const string RotationProfiles = "RotationProfiles";
    public const string AgentCurrentRotationProfile = "agentCurrentRotationProfile";
    public const string DefaultRotationProfile = "DefaultRotation";

    // HEALTH
    public const string HealthProfiles = "HealthProfiles";
    public const string AgentCurrentHealthProfile = "agentCurrentHealthProfile";
    public const string DefaultHealthProfile = "DefaultHealth";

    // SWITCHES (Other/Example)
    public const string SwitchProfiles = "SwitchProfiles";
    public const string AgentCurrentSwitchProfile = "agentCurrentSwitchProfile";
    public const string DefaultSwitchProfile = "DefaultSwitch";

    // TIMING
    public const string TimingProfiles = "TimingProfiles";
    public const string AgentCurrentTimingProfile = "agentCurrentTimingProfile";
    public const string DefaultTimingProfile = "DefaultTiming";
}