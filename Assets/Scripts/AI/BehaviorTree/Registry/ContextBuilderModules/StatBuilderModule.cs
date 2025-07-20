using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using Systems.StatusEffectSystem.Component;
using Utils.Component;

// [2025-07-06 ARCHITECTURE WARNING]
// The following loop dynamically adds custom stats to the blackboard using raw string keys:
// foreach (var kvp in modifiers.Custom) blackboard.Set(kvp.Key, kvp.Value);
// ---
// This is a potential source of "stringly-typed" bugs, key collisions, and typo errors.
// Before extending or refactoring this logic:
//  - Decide whether all custom stats should be namespaced (e.g., "CustomStat:Jump").
//  - Or, group all custom stats in a dictionary and store them under a single blackboard key (e.g., "Stats.Custom").
//  - NEVER rely on ad-hoc keys from designers or other systems unless they're strictly controlled.
// For now, do NOT refactor unless you have a clear, portfolio-worthy use case and API for custom stats.
// ---
namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class StatBuilderModule : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var scriptName = nameof(StatBuilderModule);
            var agent = context.Agent;
            var blackboard = context.Blackboard;
        
            var statusManager = blackboard.StatusEffectManager;
            var statSynchronizer = agent.RequireComponent<StatSynchronizer>(); // <<-- [2025-06-25] Red flag and needs to be abstract??? For checking 
        
            var modifiers = statusManager.AgentModifiers.Stats;
        
            blackboard.Set(BlackboardKeys.Multipliers.Movement, modifiers.Movement);
            blackboard.Set(BlackboardKeys.Multipliers.Attack, modifiers.Attack);
            blackboard.Set(BlackboardKeys.Multipliers.Armor, modifiers.Armor);

            //statSynchronizer.Initialize(context);
            //statSynchronizer.SetStatusEffectManager(statusManager);
        
            // Optional: loop custom stats if you use them
            if (modifiers.Custom.Count <= 0) return;
        
            foreach (var kvp in modifiers.Custom)
                blackboard.Set(kvp.Key, kvp.Value);
        }
    }
}