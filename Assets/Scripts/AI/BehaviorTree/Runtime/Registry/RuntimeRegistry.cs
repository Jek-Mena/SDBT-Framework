using System.Runtime.CompilerServices;
using System.Threading;

namespace AI.BehaviorTree.Runtime.Registry
{
    static class RuntimeIdRegistry
    {
        private static int _next; // starts at 0; thread-safe with Interlocked
        private static readonly ConditionalWeakTable<object, IdBox> Map = new();

        private sealed class IdBox { public readonly int Id; public IdBox(int id) => Id = id; }

        public static int GetId(object obj)
        {
            // If it’s a Unity object, just use Unity’s ID.
            if (obj is UnityEngine.Object uo) return uo.GetInstanceID();

            // Allocates one tiny box per unique object, once.
            return Map.GetValue(obj, _ => new IdBox(Interlocked.Increment(ref _next))).Id;
        }
    }
}