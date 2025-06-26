using System;
using UnityEngine;

public class StatusEffectBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(StatusEffectBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
            
        var effect = agent.RequireComponent<StatusEffectManager>();
        if (!effect)
            throw new Exception($"[{scriptName}] {nameof(StatusEffectManager)} missing on {agent.name}");
        blackboard.StatusEffectManager = effect;
        Debug.Log($"[{scriptName}] Injected {nameof(StatusEffectManager)} for {agent.name}");
    }
}