// JsonKeys maps to property keys *inside* each JSON plugin config block.
// These are used to safely access typed config parameters in each plugin.
//
// Example:
// ["BtInjectAbilityNode", { "abilityId": "DashIfNear" }]
// → JsonKeys.Ability.Id == "abilityId"
//
// This avoids magic strings like config["abilityId"] in every plugin.

// TODO: Modularize JsonKeys by splitting into domain-specific static classes (e.g., JsonKeysMovement, JsonKeysBt).
// Ensure naming consistency with PluginKey domains.
// Optional: Add attributes for future tooling support (e.g., [PluginConfigKey("impulseStrength")]). <<-- Already implemented, just don't forget to implement it.

public static class JsonKeys
{
    public static class Bt
    {
        public const string MaxRepeats = "maxRepeats";
    }

    public static class Movement
    {
        public const string Speed = "speed";
        public const string Acceleration = "acceleration";
        public const string StoppingDistance = "stoppingDistance";
        public const string UpdateThreshold = "updateThreshold";
    }

    public static class Impulse
    {
        public const string ImpulseStrength = "impulseStrength";
        public const string Tolerance = "tolerance";
        public const string StateTimeout = "stateTimeout";
    }

    public static class TimedExecution
    {
        // Pause, Channelling, Idle, etc.
        public const string Duration = "duration";
        public const string StartDelay = "startDelay";
        public const string Interruptible = "interruptible";
        public const string FailOnInterrupt = "failOnInterrupt";
        public const string ResetOnExit = "resetOnExit";
        public const string Mode = "mode";
    }

    public static class Visual
    {
        public const string PrefabPath = "visualPrefab";
    }

    public static class Buff
    {
        public const string HealthMultiplier = "healthMultiplier";
    }

    public static class Ability
    {
        public const string Id = "abilityID";
    }
}

public static class JsonFields
{
    public const string Id = "id";
    public const string Plugin = "plugin";
    public const string Params = "params";
    public const string Prefab = "prefab";
    public const string Components = "components";
    public const string TreeId = "treeId";
    public const string Type = "type";
    public const string Children = "children";
    public const string Root = "root";
 }

public static class JsonLiterals
{
    public static class Behavior
    {
        public const string BasicChase = "basicChase";
        public const string DashOnly = "dashOnly";
    }
}
