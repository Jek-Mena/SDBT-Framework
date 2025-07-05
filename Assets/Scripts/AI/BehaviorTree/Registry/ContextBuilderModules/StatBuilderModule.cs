using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class StatBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(StatBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
        
        var statusManager = agent.RequireComponent<StatusEffectManager>();
        var statSynchronizer = agent.RequireComponent<StatSynchronizer>(); // <<-- [2025-06-25] Red flag and needs to be abstract??? For checking 
        
        var modifiers = statusManager.agentModifiers.Stats;
        
        blackboard.Set(BlackboardKeys.Core.Multipliers.Movement, modifiers.Movement);
        blackboard.Set(BlackboardKeys.Core.Multipliers.Attack, modifiers.Attack);
        blackboard.Set(BlackboardKeys.Core.Multipliers.Armor, modifiers.Armor);

        statSynchronizer.Initialize(context);
        statSynchronizer.SetStatusEffectManager(statusManager);
        
        // Optional: loop custom stats if you use them
        if (modifiers.Custom.Count <= 0) return;
        
        foreach (var kvp in modifiers.Custom)
            blackboard.Set(kvp.Key, kvp.Value);
    }
}