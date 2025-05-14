public class MovementModifier : IModifier<MovementSettings>
{
    public ModifierMeta Meta { get; }
    public MovementSettings Settings { get; set; } = new();

    public MovementModifier(ModifierMeta meta, MovementSettings settings)
    {
        Meta = meta;
        Settings = settings;
    }

    public string Source => Meta.AppliedBy;
    public int Priority => Meta.Priority;
    public string EffectTag => Meta.Label;
    public float? Duration => Meta.Duration;
    public bool CanStack => Meta.IsExclusive;
    public int MaxStacks => Meta.MaxStacks;
    public ModifierBlendMode BlendMode => Meta.BlendMode;
    public MovementSettings Apply(MovementSettings original)
    {
        var result = original != null ? new MovementSettings(original) : new MovementSettings();

        if (BlendMode != ModifierBlendMode.Replace) return result;

        result.Speed = Settings.Speed ?? result.Speed;
        result.Acceleration = Settings.Acceleration ?? result.Acceleration;
        result.StoppingDistance = Settings.StoppingDistance ?? result.StoppingDistance;
        result.IsControlled |= Settings.IsControlled;

        // Extend for additive/multiplicative later

        return result;
    }
}