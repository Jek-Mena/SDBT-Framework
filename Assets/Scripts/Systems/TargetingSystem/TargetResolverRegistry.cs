using System.Collections.Generic;

public static class TargetResolverRegistry
{
    private static readonly Dictionary<TargetingStyle, ITargetResolver> _resolvers = new()
    {
        { TargetingStyle.Single, new SingleTagTargetResolver() },
        { TargetingStyle.Closest, new ClosestTargetResolver() }
        // { TargetingStyle.Random, new RandomTargetResolver() },     // Implement as needed
        // { TargetingStyle.LowestHP, new LowestHpTargetResolver() }, // Implement as needed
        // { TargetingStyle.Farthest, new FarthestTargetResolver() }, // Implement as needed
    };

    public static ITargetResolver Get(TargetingStyle style)
        => _resolvers.TryGetValue(style, out var resolver) ? resolver : _resolvers[TargetingStyle.Closest];

    // Optional: RegisterSchema at runtime (mod support, DLC, etc.)
    public static void Register(TargetingStyle style, ITargetResolver resolver) => _resolvers[style] = resolver;
}