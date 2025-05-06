// NOTE: Priority assignment currently uses static values from ModifierPriority.
// If dynamic priority resolution is needed in the future (e.g. context-sensitive effect layering),
// replace ModifierPriority static values with delegate-backed or interface-based IPriorityResolver.
// See below:
//
// Step 1: Define IPriorityResolver
//   public interface IPriorityResolver
//   {
//       int GetPriority(string source, string effectTag);
//   }
//
// Step 2: Update ModifierPriority to use a resolver:
//   public static class ModifierPriority
//   {
//       private static IPriorityResolver _resolver;
//       public static void SetResolver(IPriorityResolver resolver) => _resolver = resolver;
//       public static int Pause => _resolver?.GetPriority("Pause", "Movement") ?? 100;
//   }
//
// This approach enables clean fallback + full dynamic scaling without affecting behavior node usage.

public class ModifierProvider<T> : ISettingsProvider<T>
{
    private readonly ModifierStack<T> _stack = new();

    public void Add(IModifier<T> modifier) => _stack.Add(modifier);
    public void Remove(string source) => _stack.RemoveAllFromSource(source);

    public T GetEffectiveSettings()
    {
        // Fallback to an empty instance if default is null (for reference types)
        T baseValue = default;

        if (baseValue == null && typeof(T) == typeof(MovementSettings))
            baseValue = (T)(object)new MovementSettings();

        return _stack.Apply(baseValue);
    }
}