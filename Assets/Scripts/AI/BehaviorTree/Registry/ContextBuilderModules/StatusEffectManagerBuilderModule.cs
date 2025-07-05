using System;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class StatusEffectManagerBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(StatusEffectManagerBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
            
        var effect = agent.RequireComponent<StatusEffectManager>();
        if (!effect)
            throw new Exception($"[{scriptName}] {nameof(StatusEffectManager)} missing on {agent.name}");
        blackboard.StatusEffectManager = effect;
        Debug.Log($"[{scriptName}] Injected {nameof(StatusEffectManager)} for {agent.name}");
    }
}