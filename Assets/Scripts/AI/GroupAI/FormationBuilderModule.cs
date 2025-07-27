using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using AI.GroupAI.SquadAgent;
using UnityEngine;

namespace AI.GroupAI
{
    public class FormationBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(FormationBuilderModule);
        
        public void Build(BtContext context)
        {
            var agentProfiles = context.AgentProfiles;
            
            // Pick the default group key from agent config (fallback to first if not set)
            var formationKey = agentProfiles.CurrentFormationProfile;
            if (string.IsNullOrEmpty(formationKey))
            {
                if (agentProfiles.GroupFormationProfiles.Count == 0) return;
                formationKey = new List<string>(agentProfiles.GroupFormationProfiles.Keys)[0];
                Debug.LogError($"[{ScriptName}] No group key specified; using '{formationKey}'.");
            }
            
            // Retrieve the group entries (array of group behaviors)
            if (!agentProfiles.GroupFormationProfiles.TryGetValue(formationKey, out var formationEntries)
                || formationEntries == null || formationEntries.Count == 0)
            {
                Debug.LogError($"[{ScriptName}] No group entries for key '{formationKey}'.");
                return;
            }

            // Register all group behavior factories if not already registered (bootstrapping elsewhere is preferred)
            // Similar to BtNodeRegistrationList 
            FormationRegistry.Register(
                BlackboardKeys.Group.SquadAgent,
                (btContext, entry) => new SquadAgent.SquadAgent(btContext, entry)
            );
            
            foreach (var formationEntry in formationEntries)
            {
                if (FormationRegistry.TryResolve(formationEntry.BlackboardKey, context, formationEntry, out var formation))
                    context.Blackboard.Set(formationEntry.BlackboardKey, formation);
                else
                    Debug.LogError($"[{ScriptName}] No registered resolver for group key '{formationEntry.BlackboardKey}'.");
            }
            
            var squadAgent = context.Blackboard.Get<ISquadAgent>(BlackboardKeys.Group.SquadAgent);
            var squadManager = SquadManagerRegistry.GetOrCreateManager(formationKey);

            var mainEntry = formationEntries[0]; // Always at least one, else we'd have returned earlier

            // Direct assign (no mapping helper needed if enum/string matches registry)
            squadManager.FormationType = mainEntry.FormationType; // If this is string or enum, set directly
            // Only set parameters if they exist (i.e., this is the leader)
            if (mainEntry.FormationParameters != null && mainEntry.Role == SquadRoles.Leader)
                squadManager.SetFormationParameters(mainEntry.FormationParameters);
            
            squadManager.AddAgent(squadAgent);
        }
    }
}