using System;
using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;
using AI.GroupAI.SquadAgent;

namespace AI.GroupAI
{
    public static class FormationRegistry
    {
        private static readonly Dictionary<string, Func<BtContext, FormationProfileEntry, IGroupBehavior>> Factories = new();

        public static void Register(string blackboardKey, Func<BtContext, FormationProfileEntry, IGroupBehavior> factory)
            => Factories[blackboardKey] = factory;
        
        public static bool TryResolve(string blackboardKey, BtContext context, FormationProfileEntry entry, out IGroupBehavior behavior)
        {
            if (Factories.TryGetValue(blackboardKey, out var factory))
            {
                behavior = factory(context, entry);
                return true;
            }
            behavior = null;
            return false;
        }
    }
}