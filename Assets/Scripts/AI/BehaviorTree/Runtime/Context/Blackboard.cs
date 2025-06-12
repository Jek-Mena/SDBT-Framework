using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime container for AI-specific data used across behavior tree nodes.
/// Holds both strongly-typed fields for common systems and a dynamic key-value store for flexible extensions.
/// </summary>
public class Blackboard
{
    // ───────────────
    // Explicit Shared Context (common AI subsystems)
    // ───────────────

    // --- Targeting System ---
    // Add near your other explicit fields
    public Dictionary<string, TargetingData> TargetingProfiles { get; set; } = new();

    public TargetingData GetTargetingProfile(string key)
    {
        if (TargetingProfiles != null && TargetingProfiles.TryGetValue(key, out var data))
            return data;
        throw new System.Exception($"[Blackboard] Targeting profile '{key}' not found.");
    }
    
    /// <summary>Runtime data associated with targeting systems</summary>
    [System.Obsolete("Use TargetingProfiles for all new code. This will be removed once migration is complete.")]
    public TargetingData TargetingData { get; set; }
    /// <summary>Strategy object that dynamically selects and returns the current target Transform based on TargetingData—supports hot-swapping for advanced AI (e.g., tower defense targeting rules).</summary>
    public ITargetResolver TargetResolver { get; set; }
    /// <summary>The actual Transform to target (set by DynamicTargetContextBuilder)</summary>
    public Transform Target; // To be replaced by Targeting System

    
    // --- Movement System ---
    
    /// <summary>Navigation or pathfinding movement controller (e.g., NavMesh, grid, etc.)</summary>
    public IMovementNode MovementLogic { get; set; }
    /// <summary>Impulse-based movement logic (e.g., knockbacks, pushes)</summary>
    public IImpulseNode ImpulseLogic;

    // --- Rotation System
    
    /// <summary>Rotation controller</summary>
    public IRotationNode RotationLogic { get; set; }
    
    // --- Timed Execution System ---
    
    /// <summary>Timed execution logic for decorators or cooldown systems</summary>
    public TimerExecutionMono TimeExecutionManager { get; set; }
    /// <summary>Runtime data associated with timing systems</summary>
    public TimedExecutionData TimerData { get; set; }

    // --- Health System ---
    
    /// <summary>Reference to the entity's health system</summary>
    public HealthSystem Health;

    // --- Miscellaneous --- // TODO for sorting

    /// <summary>Whether the AI is currently stunned (movement/decision disabled)</summary>
    public bool IsStunned; // <<-- To be handled by ???

    /// <summary>Currently playing animation state (used for transitions, blocking, etc.)</summary>
    public string CurrentAnimationState;

    /// <summary>Direction vector used for impulse movement</summary>
    public Vector3 ImpulseDirection;

    // --- Update Phase Executor
    public UpdatePhaseExecutor UpdatePhaseExecutor { get; set; }

    // --- Status Effect Manager
    public StatusEffectManager StatusEffectManager { get; set; }

    // ───────────────
    // Dynamic Key-Value Context Store
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
    /// Dumps the dynamic dictionary for debugging or inspection.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object>> DumpDynamic() => _data;

    // TODO: Add [DebuggableBlackboard] attribute support for runtime GUI/Inspector visibility
}