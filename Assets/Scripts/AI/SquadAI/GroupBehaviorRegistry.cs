using System;
using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;

namespace AI.SquadAI
{
    public static class GroupBehaviorRegistry
    {
        private static readonly Dictionary<string, Func<BtContext, GroupBehaviorProfileEntry, IGroupBehavior>> Factories = new();

        public static void Register(string blackboardKey, Func<BtContext, GroupBehaviorProfileEntry, IGroupBehavior> factory)
            => Factories[blackboardKey] = factory;
        
        public static bool TryResolve(string blackboardKey, BtContext context, GroupBehaviorProfileEntry entry, out IGroupBehavior behavior)
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