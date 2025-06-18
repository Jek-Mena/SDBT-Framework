using System.Collections.Generic;

public static class TargetResolverRegistry
{
    private static readonly Dictionary<TargetingStyle, ITargetResolver> Resolvers = new()
    {
        { TargetingStyle.Single, new SingleTagTargetResolver() },
        { TargetingStyle.Closest, new ClosestTargetResolver() }
        // { TargetingStyle.Random, new RandomTargetResolver() },     // Implement as needed
        // { TargetingStyle.LowestHP, new LowestHpTargetResolver() }, // Implement as needed
        // { TargetingStyle.Farthest, new FarthestTargetResolver() }, // Implement as needed
    };
    
    /// <summary>
    /// Retrieves the resolver for the specified style. Throws if not registered.
    /// </summary>
    public static ITargetResolver Resolve(TargetingStyle style)
    {
        if (Resolvers.TryGetValue(style, out var resolver))
            return resolver;

        // Throw explicit exception instead of fallback
        throw new KeyNotFoundException($"No TargetResolver registered for style: {style}");
    }
    
    /// <summary>
    /// Retrieves the corresponding <see cref="ITargetResolver"/> for the specified <see cref="TargetingStyle"/>.
    /// If the specified style is not found, the resolver for <see cref="TargetingStyle.Closest"/> is returned as a fallback.
    /// </summary>
    /// <param name="style">The targeting style for which to retrieve the corresponding resolver.</param>
    /// <returns>The <see cref="ITargetResolver"/> associated with the specified targeting style, or the resolver for <see cref="TargetingStyle.Closest"/> if not found.</returns>
    public static ITargetResolver ResolveOrClosest(TargetingStyle style)
        => Resolvers.TryGetValue(style, out var resolver) ? resolver : Resolvers[TargetingStyle.Closest];

    // Optional: RegisterSchema at runtime (mod support, DLC, etc.)
    public static void Register(TargetingStyle style, ITargetResolver resolver) => Resolvers[style] = resolver;
}