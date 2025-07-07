using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes.Actions.Movement;
using AI.BehaviorTree.Nodes.Actions.Rotate;
using AI.BehaviorTree.Switching;
using UnityEngine;

/// <summary>
/// Runtime container for AI-specific data used across behavior tree nodes.
/// Holds both strongly-typed fields for common systems and a dynamic key-value store for flexible extensions.
///
/// [2025-06-18] Migrated to Profile System: profile data is accessed via ProfileDictionaries only.
/// 
/// [2025-06-17 ARCHITECTURE NOTE]
/// All core runtime data for AI entities must flow through Blackboard.
/// - Fields with [Obsolete] will be removed by [set-date].
/// - Dynamic data (_data/Set/Get/TryGet) is only for optional or extension systems, never core.
/// - Each field must have a single owner: document the context builder module or system responsible.
/// - No field may be mutated by random scripts, only via context modules, the pipeline, or authorized plugins.
/// - If adding a new field, update DumpContents and document owner.
/// </summary>
public class Blackboard
{
    private const string ScriptName = nameof(Blackboard);

    /// <summary>The actual Transform to target (set by DynamicTargetContextBuilder)</summary>
    public Transform Target; // To be replaced by Targeting System

    /// <summary>Navigation or pathfinding movement controller (e.g., NavMesh, grid, etc.)</summary>
    public IMovementNode MovementLogic { get; set; }

    /// <summary>Impulse-based movement logic (e.g., knockbacks, pushes)</summary>
    public IImpulseNode ImpulseLogic { get; set; }

    /// <summary>Rotation controller</summary>
    /// 0
    public IRotationNode RotationLogic { get; set; }

    /// <summary>Timed execution logic for decorators or cooldown systems</summary>
    public TimeExecutionManager TimeExecutionManager { get; set; }

    // --- Miscellaneous --- // TODO for sorting or deletion

    /// <summary>Direction vector used for impulse movement</summary>
    [System.Obsolete] public Vector3 ImpulseDirection;

    // --- Update Phase Executor
    public UpdatePhaseExecutor UpdatePhaseExecutor { get; set; }

    // --- Status Effect Manager
    public StatusEffectManager StatusEffectManager { get; set; }

    // ───────────────
    // Dynamic Key-Value Context Store
    // Only for non-core, optional extensions; never use for primary context fields.
    // ───────────────

    private readonly Dictionary<string, object> _data = new();

    /// <summary>
    /// Dynamically registers a runtime value with the blackboard.
    /// Useful for injecting settings, services, or tools without altering the main schema.
    /// </summary>
    public void Set<T>(string key, T value) => _data[key] = value;

    /// <summary>
    /// Retrieves a previously stored dynamic value.
    /// Logs a warning if the key is missing.
    /// </summary>
    public T Get<T>(string key)
    {
        if (_data.TryGetValue(key, out var value))
            return (T)value;

        Debug.LogError($"[Blackboard] Missing key '{key}' of type {typeof(T).Name}");
        return default;
    }

    /// <summary>
    /// Tries to retrieve a value safely with type checking.
    /// Returns true if the value was found and matched the requested type.
    /// </summary>
    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var raw) && raw is T cast)
        {
            value = cast;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Removes a previously stored dynamic value. Returns true if removed; false if not present.
    /// </summary>
    public bool Remove(string key)
    {
        return _data.Remove(key);
    }

    // ───────────────
    // End Dynamic Key-Value Context Store
    // ───────────────

    /// <summary>
    /// Dumps the dynamic dictionary for debugging or inspection.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object>> DumpDynamic() => _data;
}
