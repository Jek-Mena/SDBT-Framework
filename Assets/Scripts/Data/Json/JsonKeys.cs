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

/// <summary>
/// JSON config keys used *inside* the "config" blocks of behavior tree or plugin nodes.
/// Centralized here to avoid magic strings like config["duration"] or config["speed"].
/// Keys expected *inside* the "config" block of nodes
/// </summary>
public static class JsonKeys
{
    /// <summary>
    /// Behavior tree-specific keys (repeater, decorator options, etc.)
    /// </summary>
    public static class Bt
    {
        public const string MaxRepeats = "maxRepeats";
    }

    /// <summary>
    /// Keys used in movement-related nodes (MoveTo, etc.)
    /// </summary>
    public static class Movement
    {
        public const string Speed = "speed";
        public const string AngularSpeed = "angularSpeed";
        public const string Acceleration = "acceleration";
        public const string StoppingDistance = "stoppingDistance";
        public const string UpdateThreshold = "updateThreshold";

        public const string OverrideSpeed = "overrideSpeed";
    }

    /// <summary>
    /// Impulse-style nodes that apply physics or directional bursts.
    /// </summary>
    public static class Impulse
    {
        public const string ImpulseStrength = "impulseStrength";
        public const string Tolerance = "tolerance";
        public const string StateTimeout = "stateTimeout";
    }

    /// <summary>
    /// Timer-based execution nodes (Pause, Timeout, etc.)
    /// </summary>
    public static class TimedExecution
    {
        public const string Key = "key";
        public const string Duration = "duration";
        public const string StartDelay = "startDelay";
        public const string Interruptible = "interruptible";
        public const string FailOnInterrupt = "failOnInterrupt";
        public const string ResetOnExit = "resetOnExit";
        public const string Mode = "mode";
    }

    /// <summary>
    /// Nodes or systems that spawn visual effects.
    /// </summary>
    public static class Visual
    {
        public const string PrefabPath = "visualPrefab";
    }

    /// <summary>
    /// Buff modifiers applied to entities.
    /// </summary>
    public static class Buff
    {
        public const string HealthMultiplier = "healthMultiplier";
    }

    /// <summary>
    /// Ability trigger or reference keys (used in injection or selection).
    /// </summary>
    public static class Ability
    {
        public const string Id = "abilityID";
    }
}

/// <summary>
/// Root-level fields used in JSON structure (outside "config").
/// Defines plugin identity, children lists, type tags, etc.
/// </summary>
public static class JsonFields
{
    public const string BtKey = "btKey";
    public const string Config = "config";       // The nested config object
    public const string Id = "id";               // Unique plugin or node ID
    public const string Plugin = "plugin";       // Plugin name (for injection system)
    public const string Params = "params";       // Plugin configuration block
    public const string Prefab = "prefab";       // Linked prefab (e.g., visuals)
    public const string Components = "components"; // ECS/MonoBridge-style component list
    public const string TreeId = "treeId";       // Behavior tree reference
    public const string Type = "type";           // Old legacy node key (deprecated)
    public const string Children = "children";   // Subtree/child node block
    public const string Root = "root";           // Entry point for BT definition

    /// <summary>
    /// Behavior tree-specific Fields (repeater, decorator options, etc.)
    /// Related to BT structure, but outside "config"
    /// </summary>
    public static class BtFields
    {
        public const string MaxRepeats = "maxRepeats";
    }
}