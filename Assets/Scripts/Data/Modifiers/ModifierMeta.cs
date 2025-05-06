public readonly struct ModifierMeta // Immutable metadata for modifiers
{
    public string Source { get; } // e.g., "PauseNode", "Stun", "SlowAura"
    public int Priority { get; } // Higher wins
    public string EffectTag { get; } // Used for filtering/sorting if needed
    public ModifierBlendMode BlendMode { get; }
    public float? Duration { get; }
    public bool CanStack { get; }
    public int MaxStacks { get; }

    public ModifierMeta(
        string source,
        int priority,
        string effectTag,
        ModifierBlendMode blendMode,
        float? duration = null,
        bool canStack = false,
        int maxStacks = 1)
    {
        Source = source;
        Priority = priority;
        EffectTag = effectTag;
        BlendMode = blendMode;
        Duration = duration;
        CanStack = canStack;
        MaxStacks = maxStacks;
    }
}