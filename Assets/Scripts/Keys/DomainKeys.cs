/// <summary>
/// DomainKeys defines behavior-blocking and coordination domains used in PauseNode,
/// LockBehavior, StatusEffectManager, and similar systems.
/// These domains are referenced in config fields such as \"domains\", \"blockedDomains\", etc.
///
/// Example:
/// \"domains\": [\"Domain/Movement\"]
/// </summary>
public static class DomainKeys
{
    public const string Movement = "Domain/Movement";
    public const string Targeting = "Domain/Targeting";
    public const string Combat = "Domain/Combat";
    public const string AI = "Domain/AI";
    public const string Navigation = "Domain/Navigation";
    public const string Animation = "Domain/Animation";
    public const string TimedExecution = "Domain/TimedExecution";
    // Optional: Alias for default pause/stun behavior
    public const string Default = Movement;
}