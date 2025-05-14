public enum ModifierBlendMode
{
    /// <summary>
    /// Replace previous stacks (default).
    /// </summary>
    Replace,
    
    /// <summary>
    /// Add values across stacks (e.g., poison damage, speed).
    /// </summary>
    Additive,

    /// <summary>
    /// Multiply values across stacks (e.g., poison damage, speed).
    /// </summary>
    Multiplicative,

    /// <summary>
    /// Use the highest value across all stacks (e.g., duration or priority).
    /// </summary>
    HighestOnly
}