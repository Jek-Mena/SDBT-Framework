// PluginKey maps to the plugin *type* identifier in the JSON schema.
// It must match the string used as the first item in the JSON component entry array:
//
// Example:
// [Plugin(PluginKey.BtInjectAbilityNode)]
// → Matches JSON: ["BtInjectAbilityNode", { "abilityId": "DashIfNear" }]
//
// DO NOT confuse this with internal config fields like "abilityId" — those belong in JsonKeys.

// TODO: Gradual migration plan for PluginKey scalability:
// - Step 1: Prefix existing PluginKey entries by domain (e.g., BtInjectAbilityNode → Bt_InjectAbilityNode, BtNode_ImpulseRigidbody → Movement_DashRigidbody)
//   This keeps organization clean without breaking existing [Plugin(PluginKey.X)] usage.
// - Step 2: When ready, consider splitting PluginKey into domain-specific enums (BtPluginKey, MovementPluginKey, etc.)
//   PRO: Scales better as plugin count grows; clearer ownership by system.
//   CON: Breaks current PluginAttribute(PluginKey) signature.
// - Step 3: To support modular enums, refactor PluginAttribute to take either:
//     (A) a string key → [Plugin("Bt.InjectAbilityNode")]
//     (B) an IPluginKey interface implemented by all domain enums
// - Step 4: Update PluginRegistry to store plugins in a Dictionary<string, IEntityComponentPlugin>
//   and resolve using the unified string key from the attribute.
// - NOTE: Keeping the flat PluginKey enum with prefixed keys is safe and scalable short-term,
//   and defers complexity until enum size or domain entanglement becomes a problem.

using UnityEngine.Rendering;

public enum PluginKey
{
    // Behavior Tree: Node Loaders & Injectors
    BtLoadTree,
    BtSequenceNode,
    BtInjectAbilityNode,

    // Behavior Tree Condition-based

    // === Behavior Tree Nodes ===
    BtNode_MovementNavMesh,     // Move (and also rotate) to target using NavMesh
    BtNode_ImpulseRigidbody,    // Move using physics impulse

    // === Behavior Tree Trees (subtree loaders) ===
    BtTree_DashOnly,
    BtTree_BasicChase,

    // Behavior Tree: Time-Based
    BtNode_Pause,

    // Status/Stats
    StatusBuff,
    StatusPoison,   // future
    StatusBurn,     // future

    // Visuals
    VisualOverride,
    VisualEffect,   // optional split for pure VFX

    // Skills
    SkillProjectile,
    SkillMelee,

    // Audio
    AudioOverride,  // e.g. play grunt/growl sounds

    // Debug/Test
    DebugLog
}

public enum FXPrefabKey
{
    [ResourcePath("FX/GlowAura")] GlowAura
}

public static class NodeName
{
    public const string MoveTo = "MoveTo";
    public const string Impulse = "Impulse";
    public const string Pause = "Pause";
}