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

    /// <summary>Navigation or pathfinding movement controller (e.g., NavMesh, grid, etc.)</summary>
    public IMovementNode MovementLogic;

    /// <summary>Modifier stack or provider for adjusting movement behavior (e.g., status effects)</summary>
    public ModifierProvider<MovementSettings> MovementModifiers;

    /// <summary>Impulse-based movement logic (e.g., knockbacks, pushes)</summary>
    public IImpulseNode ImpulseLogic;

    /// <summary>Timed execution logic for decorators or cooldown systems</summary>
    public ITimedExecutionNode TimedExecutionLogic;

    /// <summary>Runtime data associated with timing systems</summary>
    public TimedExecutionData TimerData;

    /// <summary>Reference to the entity's health system</summary>
    public HealthSystem Health;

    /// <summary>Target the AI is currently reacting to or pursuing</summary>
    public Transform Target; // To be replaced by Targeting System

    /// <summary>Whether the AI is currently stunned (movement/decision disabled)</summary>
    public bool IsStunned; // <<-- To be handled by ???

    /// <summary>Currently playing animation state (used for transitions, blocking, etc.)</summary>
    public string CurrentAnimationState;

    /// <summary>Direction vector used for impulse movement</summary>
    public Vector3 ImpulseDirection;

    // ───────────────
    // Dynamic Key-Value Context Store
    // ───────────────
    private readonly Dictionary<string, object> _data = new();

    /// <summary>
    /// Dynamically registers a runtime value with the blackboard.
    /// Useful for injecting settings, services, or tools without altering the main schema.
    /// </summary>
    public void Set<T>(string key, T value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// Retrieves a previously stored dynamic value.
    /// Logs a warning if the key is missing.
    /// </summary>
    public T Get<T>(string key)
    {
        if (_data.TryGetValue(key, out var value))
            return (T)value;

        Debug.LogWarning($"[Blackboard] Missing key '{key}' of type {typeof(T).Name}");
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