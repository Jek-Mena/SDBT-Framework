//TODO: Duration system + expiration tick
//TODO: Merge logic per blend mode (ApplyStacked across modifiers)
//TODO: Visual debug tools to trace modifier influence live

public interface IModifier<T>
{
    string Source { get; }
    float? Duration { get; }
    int Priority { get; }
    bool CanStack { get; } // true = stackable, false = refresh
    int MaxStacks { get; } // 1 = no stack, >1 = stack limit, int.MaxValue = infinite
    string EffectTag { get; } // "Movement", "Cooldown", "Animation"
    ModifierBlendMode BlendMode { get; } // Defines how to apply
    T Apply(T original);

    // T = float -> modify speed, rate, time
    //
    // T = Vector3 -> modify direction
    //
    // T = MovementData -> full data profile
    //
    // T = CooldownProfile -> cooldowns
    //
    // T = ProjectileSettings -> projectile lifetime, speed
}