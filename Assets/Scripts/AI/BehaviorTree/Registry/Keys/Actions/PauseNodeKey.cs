public sealed class PauseNodeKey : IBtNodeKey<PauseNodeFactory>
{
    public static string Alias => JsonLiterals.Behavior.TimedExecution.Pause;
    // This is purely structural glue.
    // It tells the compiler: “This factory is keyed by this unique type.”
    // It's "elegant" in its constraint but verbose in practice.
    // It exists only to bind the string to the factory in a type-safe way.
}