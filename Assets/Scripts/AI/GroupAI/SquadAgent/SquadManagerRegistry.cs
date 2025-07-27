using System.Collections.Generic;

namespace AI.GroupAI.SquadAgent
{
    public static class SquadManagerRegistry
    {
        private static readonly Dictionary<string, SquadManager> Managers = new();
        public static SquadManager GetOrCreateManager(string groupKey)
        {
            if (!Managers.TryGetValue(groupKey, out var manager))
            {
                manager = new SquadManager();
                Managers[groupKey] = manager;
            }
            return manager;
        }
        public static IEnumerable<SquadManager> AllManagers => Managers.Values;
    }
}