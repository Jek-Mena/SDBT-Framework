using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// Encapsulates metadata about a runtime modifier (e.g., slow, pause, buff).
/// Intended for debugging, analytics, or logic layering (e.g., resolving stackable effects).
/// </summary>
public readonly struct ModifierMeta // Immutable metadata for modifiers
{
    // === Identity ===


    /// <summary>
    /// The key or alies of the plugin or node that applied this modifier.
    /// Should match one of your centralized keys (e.g., <see cref="PluginKeys.TimedExecution.Pause"/> or <see cref="Timeout"/>).
    /// </summary>
    public string AppliedBy { get; } // e.g., "PauseNode", "Stun", "SlowAura"

    /// <summary>
    /// Optional grouping tag (e.g., "Movement", "CrowdControl", "Root").
    /// Used for bulk filtering or removals.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Optional label to identify a specific use case (e.g., "StandStill").
    /// </summary>
    public string Label { get; }


    // == Stack Behavior


    /// <summary>
    /// How multiple stacks interact (e.g., Additive, Replace, HighestOnly).
    /// </summary>
    public ModifierBlendMode BlendMode { get; }

    /// <summary>
    /// How many times this modifier can be stacked simultaneously.
    /// Default is 1. Use higher for stacking effects like poison or buffs.
    /// </summary>
    public int MaxStacks { get; }


    // === Lifecycle ===


    /// <summary>
    /// When this modifier was applied (in seconds since game start).
    /// Used for ordering, debugging, or temporal logic.
    /// </summary>
    public float TimeApplied { get; }

    /// <summary>
    /// How long this modifier is intended to last (in seconds).
    /// -1 or float.MaxValue means "indefinite."
    /// </summary>
    public float Duration { get; }

    
    // === Priority / Behavior ===



    /// <summary>
    /// Optional priority for conflict resolution.
    /// Higher value overrides lower-priority modifiers in the same Category.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// If true, this modifier should be exclusive within its category (e.g., only one movement override allowed).
    /// </summary>
    public bool IsExclusive { get; }

    
    // === Diagnostics / Debug ===


    /// <summary>
    /// Free-form text, logged or visualized in UIs. Never used in logic.
    /// </summary>
    public string Description { get; }

    public ModifierMeta(
        string appliedBy,
        string category,
        string label,
        ModifierBlendMode blendMode,
        int maxStacks,
        float timeApplied,
        float duration,
        int priority,
        bool isExclusive = false,
        string description = null
    )
    {
        if (string.IsNullOrWhiteSpace(appliedBy))
            throw new ArgumentException("Modifier must include an AppliedBy key/alias.");

        if (maxStacks < 1)
            throw new ArgumentOutOfRangeException(nameof(maxStacks), "MaxStacks must be >= 1.");

        AppliedBy = appliedBy;
        Category = category;
        Label = label;
        BlendMode = blendMode;
        MaxStacks = maxStacks;
        TimeApplied = timeApplied;
        Duration = duration;
        Priority = priority;
        IsExclusive = isExclusive;
        Description = description;
    }

    public override string ToString() =>
        $"[Modifier] {Category}/{Label} from '{AppliedBy}' x{MaxStacks} [{BlendMode}] " +
        $"for {Duration}s @ {TimeApplied}s | P:{Priority} | X:{IsExclusive}";

    public static ModifierMeta CreateNow(
        string appliedBy,
        string category,
        string label,
        ModifierBlendMode blendMode,
        int maxStacks,
        float duration,
        int priority = 0,
        bool isExclusive = false,
        string description = null
    )
    {
        if (string.IsNullOrWhiteSpace(appliedBy))
            throw new ArgumentException("Modifier must include an AppliedBy key/alias.");

        if (maxStacks < 1)
            throw new ArgumentOutOfRangeException(nameof(maxStacks), "MaxStacks must be >= 1.");

        return new ModifierMeta(
            appliedBy,
            category,
            label,
            blendMode,
            maxStacks,
            Time.time,
            duration,
            priority,
            isExclusive,
            description
        );
    }
}