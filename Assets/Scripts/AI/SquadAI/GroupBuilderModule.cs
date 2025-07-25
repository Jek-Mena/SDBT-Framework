using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.SquadAI
{
    public class GroupBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(GroupBuilderModule);
        
        public void Build(BtContext context)
        {
            var agentProfiles = context.AgentProfiles;
            
            // Pick the default group key from agent config (fallback to first if not set)
            var groupKey = agentProfiles.CurrentGroupProfileKey;
            if (string.IsNullOrEmpty(groupKey))
            {
                if (agentProfiles.GroupBehaviorProfiles.Count == 0) return;
                groupKey = new List<string>(agentProfiles.GroupBehaviorProfiles.Keys)[0];
                Debug.LogWarning($"[{ScriptName}] No group key specified; using '{groupKey}'.");
            }
            
            // Retrieve the group entries (array of group behaviors)
            if (!agentProfiles.GroupBehaviorProfiles.TryGetValue(groupKey, out var groupEntries)
                || groupEntries == null || groupEntries.Count == 0)
            {
                Debug.LogWarning($"[{ScriptName}] No group entries for key '{groupKey}'.");
                return;
            }

            // Register all group behavior factories if not already registered (bootstrapping elsewhere is preferred)
            GroupBehaviorRegistry.Register( BlackboardKeys.Group.SquadAgent, (btContext, entry) => new SquadAgent(btContext, entry));

            // Attach each group behavior dynamically
            foreach (var groupEntry in groupEntries)
            {
                if (GroupBehaviorRegistry.TryResolve(groupEntry.BlackboardKey, context, groupEntry, out var groupBehavior))
                {
                    context.Blackboard.Set(groupEntry.BlackboardKey, groupBehavior);
                }
                else
                {
                    Debug.LogError($"[{ScriptName}] No registered resolver for group key '{groupEntry.BlackboardKey}'.");
                }
            }
            
            var squadAgent = context.Blackboard.Get<ISquadAgent>(BlackboardKeys.Group.SquadAgent);
            var squadManager = SquadManagerRegistry.GetOrCreateManager(groupKey);
            squadManager.AddAgent(squadAgent);
        }
    }
}