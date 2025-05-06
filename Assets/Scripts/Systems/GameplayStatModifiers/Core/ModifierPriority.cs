// NOTE: Priority assignment currently uses static values.
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
public static class ModifierPriority
{
    public static int Pause => 100;
    public static int Stun => 110;
    public const int AbsoluteOverride = 999;
    // Future: replace with dynamic resolver injection
}