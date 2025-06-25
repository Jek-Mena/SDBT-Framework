using UnityEngine;

public class StatContextBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(StatContextBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
        
        var statusManager = agent.GetComponent<StatusEffectManager>();
        if (!statusManager)
        {
            Debug.LogError($"[{scriptName}] StatusEffectManager not found.");
            return;
        }
        
        var statSynchronizer = agent.GetComponent<StatSynchronizer>(); // <<-- [2025-06-25] Red flag and needs to be abstract??? For checking 
        if (!statSynchronizer)
        {
            Debug.LogError($"[{scriptName}] StatSynchronizer not found.");
            return;
        }
        
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